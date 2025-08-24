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
            app.MapGet("/data/employees", GetEmployeesAsync).RequireAuthorization();
            app.MapPost("/data/employees", HandlePostAsync).RequireAuthorization();
        }
        private static async Task<IResult> HandlePostAsync(IDBClient db, HttpContext context)
        {
            using (var stream = await context.Request.GetMemoryStreamAsync())
            using (var reader = new Streams.Reader(stream))
            {
                var type = reader.ReadByte();
                if (type == (byte)0)
                {
                    return await GetEmployeesAsync(db, context);
                }
                else if (type == (byte)1)
                {
                    var model = Employee.Create(reader);
                    return await CreateAsync(model, db, context);
                }
                else
                {
                    return Results.BadRequest();
                }
            }
        }
        private static async Task<IResult> GetEmployeesAsync(IDBClient db, HttpContext context)
        {
            var commandText = """
                SELECT e.fullname, e.streetaddress, e.email, e.phone, r.name AS rolename, CASE e.creatorid WHEN 0 THEN 'System' ELSE c.fullname END AS creator, e.createddate
                FROM employees AS e
                INNER JOIN roles AS r ON e.roleid = r.roleid
                LEFT JOIN employees AS c ON e.creatorid = c.employeeid
                WHERE e.isdeleted = false
                """;
            return Results.File(await db.ExecuteBinaryTableAsync(commandText), "application/octet-stream");
        }
        private static async Task<IResult> CreateAsync(Employee employee, IDBClient db, HttpContext context)
        {
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
    }
}
