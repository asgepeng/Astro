using Astro.Data;
using Astro.Models;
using Astro.Server.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Astro.Binaries;

namespace Astro.Server.Api
{
    internal static class Employees
    {
        internal static void MapEmployeeIendPoints(this WebApplication app)
        {
            app.MapGet("/data/employees/{id}", GetByIdAsync).RequireAuthorization();
            app.MapPost("/data/employees", async Task<IResult> (IDBClient db, HttpContext context) =>
            {
                using (var ms = await context.Request.GetMemoryStreamAsync())
                using (var reader = new BinaryDataReader(ms))
                {
                    var requestType = reader.ReadByte();
                    if (requestType == 0x00) return await GetDataTableAsync(reader, db, context);
                    else if (requestType == 0x01) return await CreateAsync(reader, db, context);
                    else return Results.BadRequest();
                }
            }).RequireAuthorization();
            app.MapPut("/data/employees", UpdateAsync).RequireAuthorization();
        }
        private static async Task<IResult> GetDataTableAsync(BinaryDataReader reader, IDBClient db, HttpContext context)
        {
            var listingType = reader.ReadByte();
            if (listingType == 0x00)
            {
                var commandText = """
                    SELECT e.employeeid, e.fullname, e.streetaddress, e.email, e.phone, r.name AS rolename, CASE e.creatorid WHEN 0 THEN 'System' ELSE c.fullname END AS creator, e.createddate
                    FROM employees AS e
                    INNER JOIN roles AS r ON e.roleid = r.roleid
                    LEFT JOIN employees AS c ON e.creatorid = c.employeeid
                    WHERE e.isdeleted = false AND e.employeeid > 1
                    """;
                return Results.File(await db.ExecuteBinaryTableAsync(commandText), "application/octet-stream");
            }
            else if (listingType == 0x01)
            {
                return Results.BadRequest();
            }
            else if (listingType == 0x02)
            {
                var commandText = """
                    SELECT employeeid, fullname
                    FROM employees
                    WHERE employeeid NOT IN ((SELECT employeeid FROM logins))
                    """;
                using (var writer = new BinaryDataWriter())
                {
                    var iPos = writer.ReserveInt32();
                    var iCount = 0;
                    await db.ExecuteReaderAsync(async r =>
                    {
                        while (await r.ReadAsync())
                        {
                            writer.WriteInt16(r.GetInt16(0));
                            writer.WriteString(r.GetString(1));
                            iCount++;
                        }
                    }, commandText);
                    writer.WriteInt32(iCount, iPos);
                    return Results.File(writer.ToArray(), "application/octet-stream");
                }
            }
            else if (listingType == 0x03)
            {
                var employeeId = reader.ReadInt16();
                var commandText = """
                    SELECT name
                    FROM employees
                    WHERE employeeid = @id
                    """;
                var employeeName = await db.ExecuteScalarAsync<string>(commandText, db.CreateParameter("id", employeeId, DbType.Int16));
                if (string.IsNullOrEmpty(employeeName)) employeeName = "";
                return Results.File(Encoding.UTF8.GetBytes(employeeName), "application/octet-stream");
            }
            else
            {
                return Results.BadRequest();
            }
        }
        private static async Task<IResult> CreateAsync(BinaryDataReader reader, IDBClient db, HttpContext context)
        {
            var parameters = new DbParameter[]
            {
                db.CreateParameter("fullname", reader.ReadString(), DbType.AnsiString),
                db.CreateParameter("placeofbirth", reader.ReadString(), DbType.AnsiString),
                db.CreateParameter("dateofbirth", reader.ReadDateTime(), DbType.DateTime),
                db.CreateParameter("sex", reader.ReadInt16(), DbType.Int16),
                db.CreateParameter("maritalstatus", reader.ReadInt16(), DbType.Int16),
                db.CreateParameter("email", reader.ReadString(), DbType.AnsiString),
                db.CreateParameter("phone", reader.ReadString(), DbType.AnsiString),
                db.CreateParameter("streetaddress", reader.ReadString(), DbType.AnsiString),
                db.CreateParameter("zipcode", reader.ReadString(), DbType.AnsiString),
                db.CreateParameter("villageid", reader.ReadInt64(), DbType.Int64),
                db.CreateParameter("roleid", reader.ReadInt16(), DbType.Int16),
                db.CreateParameter("isactive", reader.ReadBoolean(), DbType.Boolean),
                db.CreateParameter("creator", context.GetUserID(), DbType.Int16)
            };
            var commandText = """
                INSERT INTO employees 
                (
                    fullname, placeofbirth, dateofbirth, sex, maritalstatus, streetaddress,
                    villageid, zipcode, phone, email, hireddate, roleid, isactive, terminationdate, payrolldate,
                    payrollmethod, notes, creatorid
                ) 
                VALUES 
                (
                    @fullname, @placeofbirth, @dateofbirth, @sex, @maritalstatus, @streetaddress,
                    @villageid, @zipcode, @phone, @email, CURRENT_TIMESTAMP, @roleid, @isactive, NULL, 10,
                    1,'', @creator
                );
                """;
            var success = await db.ExecuteNonQueryAsync(commandText, parameters);
            return success ? Results.Ok(CommonResult.Ok("Employee is saved successfully")) : Results.Problem("An error occured while save employee, please try again later");
        }
        private static async Task<IResult> UpdateAsync(IDBClient db, HttpContext context)
        {
            using (var stream = await context.Request.GetMemoryStreamAsync())
            using (var reader = new BinaryDataReader(stream))
            {
                var parameters = new DbParameter[]
                {
                    db.CreateParameter("employeeid", reader.ReadInt16(), DbType.Int16),
                    db.CreateParameter("fullname", reader.ReadString(), DbType.AnsiString),
                    db.CreateParameter("placeofbirth", reader.ReadString(), DbType.AnsiString),
                    db.CreateParameter("dateofbirth", reader.ReadDateTime(), DbType.DateTime),
                    db.CreateParameter("sex", reader.ReadInt16(), DbType.Int16),
                    db.CreateParameter("maritalstatus", reader.ReadInt16(), DbType.Int16),
                    db.CreateParameter("email", reader.ReadString(), DbType.AnsiString),
                    db.CreateParameter("phone", reader.ReadString(), DbType.AnsiString),
                    db.CreateParameter("streetaddress", reader.ReadString(), DbType.AnsiString),
                    db.CreateParameter("zipcode", reader.ReadString(), DbType.AnsiString),
                    db.CreateParameter("villageid", reader.ReadInt64(), DbType.Int64),
                    db.CreateParameter("roleid", reader.ReadInt16(), DbType.Int16),
                    db.CreateParameter("isactive", reader.ReadBoolean(), DbType.Boolean),
                    db.CreateParameter("editor", context.GetUserID(), DbType.Int16)
                };
                var commandText = """
                    UPDATE employees
                    SET fullname = @fullname,
                        placeofbirth = @placeofbirth,
                        dateofbirth = @dateofbirth,
                        sex = @sex,
                        maritalstatus = @maritalstatus,
                        email = @email,
                        phone = @phone,
                        streetaddress = @streetaddress,
                        zipcode = @zipcode,
                        villageid = @villageid,
                        roleid = @roleid,
                        isactive = @isactive,
                        editorid = @editor
                    WHERE employeeid = @employeeid
                    """;
                return await db.ExecuteNonQueryAsync(commandText, parameters) ? Results.Ok(CommonResult.Ok("")) : Results.Problem("Kesalahan yang tidak diketahui terjadi saat memperbarui data pegawai, silakan coba lain waktu");
            }
        }
        private static async Task<IResult> GetByIdAsync(short id, IDBClient db, HttpContext context)
        {
            using (var writer = new BinaryDataWriter())
            {
                var commandText = """
                    SELECT e.employeeid, e.fullname, e.placeofbirth, e.dateofbirth, e.sex, e.maritalstatus, e.email, e.phone, 
                        e.streetaddress, e.zipcode, e.villageid, v.districtid, d.cityid, c.stateid, e.roleid, e.hireddate, e.isactive, 
                        e.payrolldate, e.notes
                    FROM employees AS e
                    LEFT JOIN villages AS v ON e.villageid = v.villageid
                    LEFT JOIN districts AS d ON v.districtid = d.districtid
                    LEFT JOIN cities AS c ON d.cityid = c.cityid
                    LEFT JOIN states AS s ON c.stateid = s.stateid
                    WHERE e.employeeid = @employeeid AND e.isdeleted = false
                    """;
                await db.ExecuteReaderAsync(async reader =>
                {
                    if (await reader.ReadAsync())
                    {
                        var exists = reader.HasRows;
                        writer.WriteBoolean(exists);                        // data found
                        if (exists)
                        {
                            writer.WriteInt16(reader.GetInt16(0));          // employee ID
                            writer.WriteString(reader.GetString(1));        // employee fullname
                            writer.WriteString(reader.GetString(2));        // place of birth
                            writer.WriteDateTime(reader.IsDBNull(3) ? 
                                null : reader.GetDateTime(3));              // date of birth
                            writer.WriteInt16(reader.GetInt16(4));          // sex
                            writer.WriteInt16(reader.GetInt16(5));          // marital status
                            writer.WriteString(reader.GetString(6));        // email address
                            writer.WriteString(reader.GetString(7));        // phone number
                            writer.WriteString(reader.GetString(8));        // street address
                            writer.WriteString(reader.GetString(9));        // zip code
                            writer.WriteInt64(reader.GetInt64(10));         // village ID
                            writer.WriteInt32(reader.GetInt32(11));         // district ID
                            writer.WriteInt32(reader.GetInt32(12));         // city ID
                            writer.WriteInt16(reader.GetInt16(13));         // state ID
                            writer.WriteInt16(reader.GetInt16(14));         // role ID
                            writer.WriteDateTime(reader.IsDBNull(15) ? 
                                null : reader.GetDateTime(15));             // hired date
                            writer.WriteBoolean(reader.GetBoolean(16));     // is active flag
                            writer.WriteDateTime(reader.IsDBNull(17) ? 
                                null : reader.GetDateTime(17));             // termination date
                            writer.WriteInt16(reader.GetInt16(18));         // payroll date, range from 1 to 30
                            writer.WriteString(reader.GetString(19));       // notes
                        }
                    }
                }, commandText, db.CreateParameter("employeeid", id, DbType.Int16));
                return Results.File(writer.ToArray(), "application/octet-stream");
            }
        }
    }
}
