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
                using (var reader = new Streams.Reader(ms))
                {
                    var requestType = reader.ReadByte();
                    if (requestType == 0x00) return await GetDataTableAsync(reader, db, context);
                    else if (requestType == 0x01) return await CreateAsync(reader, db, context);
                    else return Results.BadRequest();
                }
            }).RequireAuthorization();
        }
        private static async Task<IResult> GetDataTableAsync(Streams.Reader reader, IDBClient db, HttpContext context)
        {
            var listingType = reader.ReadByte();
            if (listingType == 0x00)
            {
                var commandText = """
                    SELECT e.employeeid, e.fullname, e.streetaddress, e.email, e.phone, r.name AS rolename, CASE e.creatorid WHEN 0 THEN 'System' ELSE c.fullname END AS creator, e.createddate
                    FROM employees AS e
                    INNER JOIN roles AS r ON e.roleid = r.roleid
                    LEFT JOIN employees AS c ON e.creatorid = c.employeeid
                    WHERE e.isdeleted = false
                    """;
                return Results.File(await db.ExecuteBinaryTableAsync(commandText), "application/octet-stream");
            }
            else if (listingType == 0x01)
            {
                return Results.BadRequest();
            }
            else
            {
                return Results.BadRequest();
            }
        }
        private static async Task<IResult> CreateAsync(Streams.Reader reader, IDBClient db, HttpContext context)
        {
            var employee = Employee.Create(reader);
            var commandText = """
                INSERT INTO employees (
                    fullname,
                    placeofbirth,
                    dateofbirth,
                    sex,
                    maritalstatus,
                    streetaddress,
                    villageid,
                    zipcode,
                    phone,
                    email,
                    hireddate,
                    roleid,
                    isactive,
                    terminationdate,
                    payrolldate,
                    payrollmethod,
                    notes,
                    creatorid
                ) VALUES (
                    @fullname,
                    @placeofbirth,
                    @dateofbirth,
                    @sex,
                    @maritalstatus,
                    @streetaddress,
                    @villageid,
                    @zipcode,
                    @phone,
                    @email,
                    @hireddate,
                    @roleid,
                    @isactive,
                    @terminationdate,
                    @payrolldate,
                    @payrollmethod,
                    @notes,
                    @creator
                );
                """;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("fullname", employee.FullName, DbType.AnsiString),
                db.CreateParameter("placeofbirth", employee.PlaceOfBirth, DbType.DateTime),
                db.CreateParameter("dateofbirth", employee.DateOfBirth, DbType.DateTime),
                db.CreateParameter("sex", employee.Sex, DbType.Int16),
                db.CreateParameter("maritalstatus", employee.MaritalStatus, DbType.Int16),
                db.CreateParameter("streetaddress", employee.StreetAddress, DbType.AnsiString),
                db.CreateParameter("villageid", employee.StreetAddress, DbType.AnsiString),
                db.CreateParameter("zipcode", employee.ZipCode, DbType.AnsiString),
                db.CreateParameter("phone", employee.PhoneNumber, DbType.AnsiString),
                db.CreateParameter("email", employee.Email, DbType.AnsiString),
                db.CreateParameter("hireddate", employee.HiredDate, DbType.DateTime),
                db.CreateParameter("roleid", employee.RoleId, DbType.Int16),
                db.CreateParameter("isactive", employee.IsActive, DbType.Boolean),
                db.CreateParameter("terminationdate", employee.TerminationDate, DbType.DateTime),
                db.CreateParameter("payrolldate", employee.PayrollDate, DbType.Int16),
                db.CreateParameter("payrollmethos", employee.PayrollMethod, DbType.Int16),
                db.CreateParameter("notes", employee.Notes, DbType.AnsiString),
                db.CreateParameter("creator", context.GetUserID(), DbType.Int16)
            };
            var success = await db.ExecuteNonQueryAsync(commandText);
            return success ? Results.Ok(CommonResult.Ok("Employee is saved successfully")) : Results.Problem("An error occured while save employee, please try again later");
        }
        private static async Task<IResult> GetByIdAsync(short id, IDBClient db, HttpContext context)
        {
            using (var writer = new Streams.Writer())
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
