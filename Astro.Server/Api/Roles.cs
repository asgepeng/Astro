using Alaska.Data;
using Astro.Data;
using Astro.Models;
using Astro.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var isWinformApp = AppHelpers.IsWinformApp(context.Request);
            var commandText = """
                select r.role_id, r.role_name, case when r.creator_id = 0 then 'System' else concat(c.user_firstname, ' ', c.user_lastname) end as creator, r.created_date
                FROM roles as r
                left join users as c on r.creator_id = c.user_id
                """;
            if (isWinformApp)
            {
                using (var builder = new BinaryBuilder())
                {
                    await db.ExecuteReaderAsync(async reader =>
                    {
                        while (await reader.ReadAsync())
                        {
                            builder.WriteInt16(reader.GetInt16(0));
                            builder.WriteString(reader.GetString(1));
                            builder.WriteString(reader.GetString(2));
                            builder.WriteDateTime(reader.GetDateTime(3));
                        }
                    }, commandText);
                    return Results.File(builder.ToArray(), "application/octet-stream");
                }
            }
            return Results.Ok();
        }
        internal static async Task<IResult> GetByIdAsync(IDatabase db, short id)
        {
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
            }, commandText, new NpgsqlParameter("id", id));
            if (role is null) return Results.NotFound(CommonResult.Fail("role not found."));
            commandText = """
                select rtm.menu_id, m.menu_title, rtm.allow_create, rtm.allow_read, rtm.allow_edit, rtm.allow_delete
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
            }, commandText, new NpgsqlParameter("@id", role.Id));
            return role != null ? Results.Ok(role) : Results.NotFound();
        }
        internal static async Task<IResult> CreateAsync(Role role, IDatabase db, HttpContext context)
        {
            var commandText = """
                insert into roles
                    (role_name, creator_id)
                values (@rolename, @creator);
                returning role_id
                """;
            var roleId = await db.ExecuteScalarInt16Async(commandText, new NpgsqlParameter("rolename", role.Name), new NpgsqlParameter("creator", AppHelpers.GetUserID(context)));
            if (roleId == null) return Results.Problem("Failed to create role.");

            commandText = """
                insert into role_to_menus
                     (role_id, menu_id, allow_create, allow_read, allow_edit, allow_delete)
                values 
                """;
            var rows = new List<string>();
            foreach (var item in role.Permissions)
            {
                var cells = new string[6];
                cells[0] = roleId.ToString()!;
                cells[1] = item.Id.ToString();
                cells[2] = item.AllowCreate ? "true": "false";
                cells[3] = item.AllowCreate ? "true" : "false";
                cells[4] = item.AllowEdit ? "true" : "false";
                cells[5] = item.AllowDelete ? "true" : "false";
                rows.Add("(" + string.Join(",", cells));
            }
            commandText += string.Join(",", rows) + ";";
            var success = await db.ExecuteNonQueryAsync(commandText);
            return success ? Results.Ok(CommonResult.Ok("Role created successfully.")) : Results.Problem("Failed to create role permissions.");
        }
        internal static async Task<IResult> UpdateAsync(Role role, IDatabase db, string json)
        {
            var commandText = """
                update roles
                set role_name = @rolename
                where role_id = @id;
                """;
            foreach (var item in role.Permissions)
            {
                commandText += $"""
                    update role_to_menus
                    set allow_create = {(item.AllowCreate ? "true": "false")},
                        allow_read = {(item.AllowRead ? "true" : "false")},
                        allow_edit = {(item.AllowEdit ? "true" : "false")},
                        allow_delete = {(item.AllowDelete ? "true" : "false")}
                    where role_id = @id and menu_id = {item.Id.ToString()};
                    """;
            }
            var success = await db.ExecuteNonQueryAsync(commandText, new NpgsqlParameter[]
            {
                new NpgsqlParameter("rolename", role.Name),
                new NpgsqlParameter("id", role.Id)
            });
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
            var success = await db.ExecuteNonQueryAsync(commandText, new NpgsqlParameter("id", id));
            return success ? Results.Ok(CommonResult.Ok("Role deleted successfully.")) : Results.Problem("Failed to delete role.");
        }
    }
}
