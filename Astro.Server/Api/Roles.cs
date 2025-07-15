using Astro.Data;
using Astro.Helpers;
using Astro.Models;
using Astro.Server.Binaries;
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
            app.MapGet("/data/roles", GetAllAsync).RequireAuthorization();
            app.MapGet("/data/roles/{id}", GetByIdAsync).RequireAuthorization();
            app.MapPost("/data/roles", CreateAsync).RequireAuthorization();
            app.MapPut("/data/roles", UpdateAsync).RequireAuthorization();
            app.MapDelete("/data/roles/{id}", DeleteAsync).RequireAuthorization();
        }
        internal static async Task<IResult> GetAllAsync(IDatabase db, HttpContext context)
        {
            var isWinformApp = Application.IsWinformApp(context.Request);
            if (isWinformApp) return Results.File(await db.GetRoleDataTable(), "application/octet-stream");
            else
            {
                var commandText = """
                    select r.role_id, r.role_name, case when r.creator_id = 0 then 'System' else concat(c.user_firstname, ' ', c.user_lastname) end as creator, r.created_date
                    FROM roles as r
                    left join users as c on r.creator_id = c.user_id
                    """;
                var sb = new StringBuilder();
                sb.Append("<table class=\"table table-striped table-bordered\">\n")
                  .Append("<thead>\n")
                  .Append("<tr>\n")
                  .Append("<th>Role ID</th>\n")
                  .Append("<th>Role Name</th>\n")
                  .Append("<th>Creator</th>\n")
                  .Append("<th>Created Date</th>\n")
                  .Append("</tr>\n")
                  .Append("</thead>\n")
                  .Append("<tbody>\n");
                await db.ExecuteReaderAsync(async reader =>
                {
                    while (await reader.ReadAsync())
                    {
                        sb.Append("<tr>");
                        sb.Append("<td>").Append(reader.GetInt16(0)).Append("</td>");
                        sb.Append("<td>").Append(reader.GetString(1)).Append("</td>");
                        sb.Append("<td>").Append(reader.GetString(2)).Append("</td>");
                        sb.Append("<td>").Append(reader.GetDateTime(3).ToString("yyyy-MM-dd HH:mm:ss")).Append("</td>");
                        sb.Append("<td><a href=\"/roles/").Append(reader.GetInt16(0)).Append("\">Edit</a></td>");
                        sb.Append("<td><a href=\"/roles/delete/").Append(reader.GetInt16(0)).Append("\">Delete</a></td>");
                        sb.Append("</tr>");
                    }
                }, commandText);
                sb.Append("</tbody></table>");
                return Results.Content(sb.ToString(), "text/html");
            }
        }
        internal static async Task<IResult> GetByIdAsync(IDatabase db, short id, HttpContext context)
        {
            var isWinformApp = Application.IsWinformApp(context.Request);
            if (isWinformApp) return Results.File(await db.GetRole(id), "application/octet-stream");

            Role? role = null;
            var commandText = """
                select role_id, role_name
                from roles
                where role_id = @id;
                """;
            await db.ExecuteReaderAsync(async reader =>
            {
                if (await reader.ReadAsync())
                {
                    role = new Role()
                    {
                        Id = reader.GetInt16(0),
                        Name = reader.GetString(1)
                    };
                }
            }, commandText, db.CreateParameter("id", id, DbType.Int16));
            if (role is null) return Results.NotFound(CommonResult.Fail("role not found."));
            commandText = """
                select
                    rtm.menu_id, m.menu_title, rtm.allow_create, rtm.allow_read, rtm.allow_edit, rtm.allow_delete
                from role_to_menus as rtm
                    inner join menus as m on rtm.menu_id = m.menu_id
                where rtm.role_id = @id;
                """;

            await db.ExecuteReaderAsync(async reader =>
            {
                while (await reader.ReadAsync())
                {
                    var item = new Permission()
                    {
                        Id = reader.GetInt16(0),
                        Name = reader.GetString(1),
                        AllowCreate = reader.GetBoolean(2),
                        AllowRead = reader.GetBoolean(3),
                        AllowEdit = reader.GetBoolean(4),
                        AllowDelete = reader.GetBoolean(5)
                    };
                    role.Permissions.Add(item);
                }
            }, commandText, db.CreateParameter("@id", role.Id, DbType.Int16));
            return role != null ? Results.Ok(role) : Results.NotFound();
        }
        internal static async Task<IResult> CreateAsync(Role role, IDatabase db, HttpContext context)
        {
            var commandText = """
                insert into roles
                    (role_name, creator_id)
                values (@rolename, @creator)
                returning role_id
                """;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("rolename", role.Name, DbType.String),
                db.CreateParameter("creator", Application.GetUserID(context), DbType.String)
            };
            var roleId = await db.ExecuteScalarInt16Async(commandText, parameters);
            if (roleId == null) return Results.Ok(CommonResult.Fail("Failed to create role."));

            commandText = """
                insert into role_to_menus
                     (role_id, menu_id, allow_create, allow_read, allow_update, allow_delete)
                values 
                """;
            var rows = new List<string>();
            foreach (var item in role.Permissions)
            {
                var cells = new string[6];
                cells[0] = roleId.ToString()!;
                cells[1] = item.Id.ToString();
                cells[2] = item.AllowCreate ? "true": "false";
                cells[3] = item.AllowRead ? "true" : "false";
                cells[4] = item.AllowEdit ? "true" : "false";
                cells[5] = item.AllowDelete ? "true" : "false";
                rows.Add("(" + string.Join(",", cells) + ")");
            }
            commandText += string.Join(",", rows) + ";";
            var success = await db.ExecuteNonQueryAsync(commandText);
            return success ? Results.Ok(CommonResult.Ok("Role created successfully.")) : Results.Problem("Failed to create role permissions.");
        }
        internal static async Task<IResult> UpdateAsync(Role role, IDatabase db, HttpContext context)
        {
            var sb = new StringBuilder();
            sb.Append(""""
                update roles
                set 
                    role_name = @rolename, 
                    editor_id = @editor, 
                    edited_date = current_timestamp
                where role_id = @id;
                insert into role_to_menus
                    (role_id, menu_id, allow_create, allow_read, allow_update, allow_delete)
                values
                """");
            var parameters = new List<DbParameter>
            {
                db.CreateParameter("rolename", role.Name, DbType.String),
                db.CreateParameter("id", role.Id, DbType.Int16),
                db.CreateParameter("editor", Application.GetUserID(context), DbType.Int16)
            };
            for (int i = 0; i < role.Permissions.Count; i++)
            {
                var item = role.Permissions[i];

                if (i > 0) sb.Append(",");
                sb.Append("(@id,").Append(item.Id)
                    .Append(",@c").Append(i)
                    .Append(",@r").Append(i)
                    .Append(",@u").Append(i)
                    .Append(",@d").Append(i).AppendLine(")");

                parameters.Add(db.CreateParameter("c" + i, item.AllowCreate, DbType.Boolean));
                parameters.Add(db.CreateParameter("r" + i, item.AllowRead, DbType.Boolean));
                parameters.Add(db.CreateParameter("u" + i, item.AllowEdit, DbType.Boolean));
                parameters.Add(db.CreateParameter("d" + i, item.AllowDelete, DbType.Boolean));
            }
            sb.Append("""
                on conflict (role_id, menu_id) 
                do update 
                set allow_create = excluded.allow_create, 
                    allow_read = excluded.allow_read, 
                    allow_update = excluded.allow_update, 
                    allow_delete = excluded.allow_delete;
                """);
            var success = await db.ExecuteNonQueryAsync(sb.ToString(), parameters.ToArray());
            return success ? Results.Ok(CommonResult.Ok("Role updated successfully.")) : Results.Problem("Failed to update role permissions.");
        }
        internal static async Task<IResult> DeleteAsync(IDatabase db, short id)
        {
            var commandText = """
                delete from roles
                where role_id = @id;
                delete from role_to_menus
                where role_id = @id;
                """;
            var success = await db.ExecuteNonQueryAsync(commandText, db.CreateParameter("id", id, DbType.Int16));
            return success ? Results.Ok(CommonResult.Ok("Role deleted successfully.")) : Results.Problem("Failed to delete role.");
        }
    }
}
