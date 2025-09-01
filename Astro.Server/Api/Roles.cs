using Astro.Data;
using Astro.Models;
using Astro.Server.Binaries;
using Astro.Server.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Astro.Server.Api
{
    internal static class Roles
    {
        internal static void MapRoleEndPoints(this WebApplication app)
        {
            app.MapGet("/data/roles/{id}", GetByIdAsync).RequireAuthorization();
            app.MapPost("/data/roles", async Task<IResult>(IDBClient db, HttpContext context) =>
            {
                using (var ms = await context.Request.GetMemoryStreamAsync())
                using (var reader = new Streams.Reader(ms))
                {
                    var requestType = reader.ReadByte();
                    if (requestType == 0x00) return await GetDataTableAsync(reader, db);
                    if (requestType == 0x01) return await CreateAsync(reader, db, context);
                    else return Results.BadRequest();
                }
            }).RequireAuthorization();
            app.MapPut("/data/roles", UpdateAsync).RequireAuthorization();
            app.MapDelete("/data/roles/{id}", DeleteAsync).RequireAuthorization();
        }
        internal static async Task<IResult> GetDataTableAsync(Streams.Reader reader, IDBClient db)
        {
            var listingMode = reader.ReadByte();
            /*
             * 0x00 all data
             * 0x01 pagination model
             */
            if (listingMode == 0x00)
            {
                var commandText = """
                SELECT r.roleid, r.name, CASE WHEN r.creatorid = 0 THEN 'System' else c.fullname END AS creator, r.createddate
                FROM roles AS r
                LEFT JOIN employees AS c ON r.creatorid = c.employeeid
                """;
                return Results.File(await db.ExecuteBinaryTableAsync(commandText), "application/octet-stream");
            }
            else if (listingMode == 0x01)
            {
                return Results.BadRequest(); // not implemented right now
            }
            else return Results.BadRequest();
        }
        internal static async Task<IResult> GetByIdAsync(IDBClient db, short id, HttpContext context)
        {
            var commandText = """
            SElECT roleid, name
            FROM roles
            WHERE roleid = @id;
            """;
            using (var builder = new Streams.Writer())
            {
                await db.ExecuteReaderAsync(async reader =>
                {
                    builder.WriteBoolean(reader.HasRows);
                    if (await reader.ReadAsync())
                    {
                        builder.WriteInt16(reader.GetInt16(0));
                        builder.WriteString(reader.GetString(1));
                    }
                }, commandText, db.CreateParameter("id", id, DbType.Int16));
                commandText = """
                        select
                            m.menuid,
                            m.title,
                            coalesce(rtm.allowcreate, false) as allowcreate,
                            coalesce(rtm.allowread,   false) as allowread,
                            coalesce(rtm.allowupdate, false) as allowupdate,
                            coalesce(rtm.allowdelete, false) as allowdelete
                        from menus as m
                        left join rolemenus as rtm
                            on m.menuid = rtm.menuid and rtm.roleid = @id                        
                        """;
                var iCount = 0;
                var iPos = builder.ReserveInt32();
                await db.ExecuteReaderAsync(async reader =>
                {
                    while (await reader.ReadAsync())
                    {
                        builder.WriteInt16(reader.GetInt16(0));
                        builder.WriteString(reader.GetString(1));
                        builder.WriteBoolean(reader.GetBoolean(2));
                        builder.WriteBoolean(reader.GetBoolean(3));
                        builder.WriteBoolean(reader.GetBoolean(4));
                        builder.WriteBoolean(reader.GetBoolean(5));
                        iCount++;
                    }
                }, commandText, db.CreateParameter("id", id, DbType.Int16));

                builder.WriteInt32(iCount, iPos);
                return Results.File(builder.ToArray(), "application/octet-stream");
            }
        }
        internal static async Task<IResult> CreateAsync(Streams.Reader reader, IDBClient db, HttpContext context)
        {
            var rolename = reader.ReadString();
            var commandCheck = """
                    SELECT 1
                    FROM roles
                    WHERE name = @rolename
                    """;
            var exists = await db.HasRecordsAsync(commandCheck, db.CreateParameter("rolename", rolename, DbType.AnsiString));
            if (exists) return Results.Ok(CommonResult.Fail("Nama '" + rolename + "' sudah dipakai, gunakan nama lain"));

            var commandText = """
                INSERT INTO roles
                    (name, creatorid)
                VALUES (@rolename, @creator)
                RETURNING roleid
                """;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("rolename", rolename, DbType.AnsiString),
                db.CreateParameter("creator", context.GetUserID(), DbType.Int16)
            };
            var roleId = await db.ExecuteScalarAsync<short>(commandText, parameters);
            if (roleId == default(short)) return Results.Ok(CommonResult.Fail("Failed to create role."));

            commandText = """
                INSERT INTO rolemenus
                     (roleid, menuid, allowcreate, allowread, allowupdate, allowdelete)
                values 
                """;
            var rows = new List<string>();
            var roleCount = reader.ReadInt32();
            for (int i = 0; i < roleCount; i++)
            {
                var cells = new string[6];
                cells[0] = roleId.ToString();
                cells[1] = reader.ReadInt16().ToString();
                cells[2] = reader.ReadBoolean() ? "true" : "false";
                cells[3] = reader.ReadBoolean() ? "true" : "false";
                cells[4] = reader.ReadBoolean() ? "true" : "false";
                cells[5] = reader.ReadBoolean() ? "true" : "false";
                rows.Add("(" + string.Join(",", cells) + ")");
            }
            commandText += string.Join(",", rows) + ";";
            var success = await db.ExecuteNonQueryAsync(commandText);
            return success ? Results.Ok(CommonResult.Ok("Role created successfully.")) : Results.Problem("Failed to create role permissions.");
        }
        internal static async Task<IResult> UpdateAsync(IDBClient db, HttpContext context)
        {
            using (var ms = await context.Request.GetMemoryStreamAsync())
            using (var reader = new Streams.Reader(ms))
            {
                var sb = new StringBuilder();
                sb.Append(""""
                UPDATE roles
                SET 
                    name = @rolename, 
                    editorid = @editor, 
                    editeddate = current_timestamp
                WHERE roleid = @id;
                INSERT INTO rolemenus
                    (roleid, menuid, allowcreate, allowread, allowupdate, allowdelete)
                VALUES
                """");
                var roleId = reader.ReadInt16();
                var parameters = new List<DbParameter>
                {
                    db.CreateParameter("rolename", reader.ReadString(), DbType.String),
                    db.CreateParameter("id", roleId, DbType.Int16),
                    db.CreateParameter("editor", context.GetUserID(), DbType.Int16)
                };
                var permissionCount = reader.ReadInt32();
                for (int i = 0; i < permissionCount; i++)
                {
                    if (i > 0) sb.Append(",");
                    sb.Append("(@id,").Append(reader.ReadInt16())
                        .Append(",@c").Append(i)
                        .Append(",@r").Append(i)
                        .Append(",@u").Append(i)
                        .Append(",@d").Append(i).AppendLine(")");

                    parameters.Add(db.CreateParameter("c" + i, reader.ReadBoolean(), DbType.Boolean));
                    parameters.Add(db.CreateParameter("r" + i, reader.ReadBoolean(), DbType.Boolean));
                    parameters.Add(db.CreateParameter("u" + i, reader.ReadBoolean(), DbType.Boolean));
                    parameters.Add(db.CreateParameter("d" + i, reader.ReadBoolean(), DbType.Boolean));
                }
                sb.Append("""
                ON CONFLICT (roleid, menuid) 
                DO UPDATE 
                SET allowcreate = excluded.allowcreate, 
                    allowread = excluded.allowread, 
                    allowupdate = excluded.allowupdate, 
                    allowdelete = excluded.allowdelete;
                """);
                var success = await db.ExecuteNonQueryAsync(sb.ToString(), parameters.ToArray());
                return success ? Results.Ok(CommonResult.Ok("Role updated successfully.")) : Results.Problem("Failed to update role permissions.");
            }
        }
        internal static async Task<IResult> DeleteAsync(IDBClient db, short id)
        {
            var commandCheck = """
                SELECT COUNT(*) AS total FROM employees WHERE roleid = @roleid AND isactive = true
                """;
            var associated = await db.ExecuteScalarAsync<int>(commandCheck, db.CreateParameter("roleid", id, DbType.Int16));
            if (associated > 0)
            {
                return Results.Problem($"Role ini tidak bisa dihapus karena masih terikat dengan {associated} pegawai aktif");
            }
            var commandText = """
                DELETE from roles
                WHERE roleid = @id;
                DELETE from rolemenus
                where roleid = @id;
                """;
            var success = await db.ExecuteNonQueryAsync(commandText, db.CreateParameter("id", id, DbType.Int16));
            return success ? Results.Ok(CommonResult.Ok("Role deleted successfully.")) : Results.Problem("Failed to delete role.");
        }
    }
}
