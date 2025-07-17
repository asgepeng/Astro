using Astro.Data;
using Astro.Helpers;
using Astro.Models;
using Astro.Server.Binaries;
using Astro.ViewModels;
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
    internal static class Customers
    {
        internal static void MapCustomerEndPoints(this WebApplication app)
        {
            app.MapGet("/data/customers", GetAllAsync).RequireAuthorization();
            app.MapGet("/data/customers/{id}", GetByIdAsync).RequireAuthorization();
            app.MapPost("/data/customers", CreateAsync).RequireAuthorization();
            app.MapPut("/data/customers", UpdateAsync).RequireAuthorization();
            app.MapDelete("/data/customers/{id}", DeleteAsync).RequireAuthorization();
        }

        private static async Task<IResult> GetAllAsync(IDatabase db, HttpContext context)
        {
            if (Application.IsWinformApp(context.Request)) return Results.File(await db.GetContactDataTable(1), "application/octet-stream");
            return Results.Ok();
        }
        private static async Task<IResult> GetByIdAsync(short id, IDatabase db, HttpContext context)
        {
            if (Application.IsWinformApp(context.Request)) return Results.File(await db.GetContact(id));

            var model = new Contact();
            var commandText = """
                select contact_id, contact_name
                from contacts
                where contact_id = @contactId
                and is_deleted = false
                """;
            await db.ExecuteReaderAsync(async reader =>
            {
                if (await reader.ReadAsync())
                {
                    model.Id = reader.GetInt32(0);
                    model.Name = reader.GetString(1);
                }
            }, commandText, db.CreateParameter("contactId", id, DbType.Int16));
            if (model.Id <= 0) return Results.NotFound("Supplier not found");

            commandText = """
                select a.address_id, a.street_address, a.city_id, c.city_name, c.state_id, s.state_name, a.address_type, a.is_primary, a.zip_code
                from addresses as a
                    inner join cities as c on a.city_id = c.city_id
                    inner join states as s on c.state_id = s.state_id
                where a.owner_id = @contactId
                """;
            await db.ExecuteReaderAsync(async reader =>
            {
                while (await reader.ReadAsync())
                {
                    model.Addresses.Add(new Address()
                    {
                        Id = reader.GetInt32(0),
                        StreetAddress = reader.GetString(1),
                        City = new City()
                        {
                            Id = reader.GetInt32(2),
                            Name = reader.GetString(3)
                        },
                        StateOrProvince = new Province()
                        {
                            Id = reader.GetInt16(4),
                            Name = reader.GetString(5)
                        },
                        Type = reader.GetInt16(6),
                        IsPrimary = reader.GetBoolean(7),
                        ZipCode = reader.GetString(8)
                    });
                }
            }, commandText, db.CreateParameter("contactId", id, DbType.Int16));

            commandText = """
                select phone_id, phone_number, phone_type, is_primary
                from phones as p
                where owner_id = @contactId
                """;
            await db.ExecuteReaderAsync(async reader =>
            {
                while (await reader.ReadAsync())
                {
                    model.Phones.Add(new Phone()
                    {
                        Id = reader.GetInt32(0),
                        Number = reader.GetString(1),
                        Type = reader.GetInt16(2),
                        IsPrimary = reader.GetBoolean(3)
                    });
                }
            }, commandText, db.CreateParameter("contactId", id, DbType.Int16));

            commandText = """
                    select email_id, email_address, email_type, is_primary
                    from emails
                    where owner_id = @contactId
                    """;
            await db.ExecuteReaderAsync(async reader =>
            {
                while (await reader.ReadAsync())
                {
                    model.Emails.Add(new Email()
                    {
                        Id = reader.GetInt32(0),
                        Address = reader.GetString(1),
                        Type = reader.GetInt16(2),
                        IsPrimary = reader.GetBoolean(3)
                    });
                }
            }, commandText, db.CreateParameter("contactId", id, DbType.Int16));
            return Results.Ok(model);
        }
        private static async Task<IResult> CreateAsync(Contact contact, IDatabase db, HttpContext context)
        {
            var commandText = """
                insert into contacts (contact_name, contact_type, creator_id)
                values (@contactName, @contactType, @creatorId)
                returning contact_id
                """;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("contactName", contact.Name.Trim(), DbType.String),
                db.CreateParameter("contactType", (short)0, DbType.Int16),
                db.CreateParameter("creatorId", Application.GetUserID(context), DbType.Int16)
            };
            var contactId = await db.ExecuteScalarInt16Async(commandText, parameters);
            if (contactId <= 0) return Results.Problem("An error occured while creating the contact. Please try again later.");

            if (contact.Addresses.Count == 0 && contact.Phones.Count == 0 && contact.Emails.Count == 0)
            {
                return Results.Ok(CommonResult.Ok("Supplier successfully created."));
            }

            var sb = new StringBuilder();
            var parameters2 = new List<DbParameter>()
            {
                db.CreateParameter("contactId", contactId,DbType.Int16)
            };
            if (contact.Addresses.Count > 0)
            {
                sb.Append("""
                    insert into addresses (owner_id, street_address, city_id, zip_code, address_type, is_primary)
                    values
                    """);
                for (int i = 0; i < contact.Addresses.Count; i++)
                {
                    if (i > 0) sb.Append(",");
                    sb.Append("(@contactId, @a").Append(i).Append(",").Append("@c").Append(i).Append(",@t").Append(i).Append(",@p").Append(i).Append(")");
                    parameters2.Add(db.CreateParameter("a" + i, contact.Addresses[i].StreetAddress.Trim(), DbType.String));
                    parameters2.Add(db.CreateParameter("c" + i, contact.Addresses[i].City, DbType.Int16));
                    parameters2.Add(db.CreateParameter("t" + i, contact.Addresses[i].Type, DbType.Int16));
                    parameters2.Add(db.CreateParameter("p" + i, contact.Addresses[i].IsPrimary, DbType.Boolean));
                }
                sb.AppendLine(";");
            }
            if (contact.Phones.Count > 0)
            {
                sb.Append("""
                    insert into phones (owner_id, phone_number, phone_type, is_primary)
                    values
                    """);
                for (short i = 0; i < contact.Phones.Count; i++)
                {
                    if (i > 0) sb.Append(",");
                    sb.Append("(@contactId, @pn").Append(i).Append(", @pt").Append(i).Append("@pp").Append(i).Append(")");
                    parameters2.Add(db.CreateParameter("pn" + i, contact.Phones[i].Number.Trim(), DbType.String));
                    parameters2.Add(db.CreateParameter("pt" + i, contact.Phones[i].Type, DbType.Int16));
                    parameters2.Add(db.CreateParameter("pp" + i, contact.Phones[i].IsPrimary, DbType.Boolean));
                }
                sb.AppendLine(";");
            }
            if (contact.Emails.Count > 0)
            {
                sb.Append("""
                    insert into emails (owner_id, email_address, email_type, is_primary)
                    values
                    """);
                for (short i = 0; i < contact.Emails.Count; i++)
                {
                    if (i > 0) sb.Append(",");
                    sb.Append("(@contactId, @ea").Append(i).Append(",@et").Append(i).Append(",@ep").Append(i).Append(")");
                    parameters2.Add(db.CreateParameter("ea" + i, contact.Emails[i].Address.Trim(), DbType.String));
                    parameters2.Add(db.CreateParameter("et" + i, contact.Emails[i].Type, DbType.Int16));
                    parameters2.Add(db.CreateParameter("ep" + i, contact.Emails[i].IsPrimary, DbType.Boolean));
                }
                sb.AppendLine(";");
            }
            var success = await db.ExecuteNonQueryAsync(commandText, parameters2.ToArray());

            return success ? Results.Ok(CommonResult.Ok("Supplier successfully created")) : Results.Problem("An error occured while creating the supplier. Please try again later.");
        }
        private static async Task<IResult> UpdateAsync(Contact contact, IDatabase db, HttpContext context)
        {
            var commandText = """
                update contacts
                set contact_name = @contactName, edited_date = current_timestamp, editor_id = @editor
                where contact_id = @contactId and is_deleted = false
                """;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("contactId", contact.Id, DbType.Int16),
                db.CreateParameter("contactName", contact.Name.Trim(), DbType.String),
                db.CreateParameter("editor", Application.GetUserID(context), DbType.Int16),
            };
            var success = await db.ExecuteNonQueryAsync(commandText, parameters);
            var sb = new StringBuilder();
            var parameter2 = new List<DbParameter>
            {
                db.CreateParameter("contactId", contact.Id, DbType.Int16),
            };
            sb.AppendLine("delete from addresses where owner_id=@contactId;");
            if (contact.Addresses.Count > 0)
            {
                sb.AppendLine("insert into addresses (owner_id, street_address, city_id, zip_code, address_type, is_primary) values ");
                for (int i = 0; i < contact.Addresses.Count; i++)
                {
                    if (i > 0) sb.Append(",");
                    sb.Append("(@contactId, @a").Append(i).Append(", @c").Append(i).Append(", @z").Append(i).Append(", @t").Append(i).Append(", @p").Append(i).Append(")");
                    parameter2.Add(db.CreateParameter("a" + i, contact.Addresses[i].StreetAddress.Trim(), DbType.String));
                    parameter2.Add(db.CreateParameter("c" + i, contact.Addresses[i].City.Id, DbType.Int16));
                    parameter2.Add(db.CreateParameter("z" + i, contact.Addresses[i].ZipCode.Trim(), DbType.String));
                    parameter2.Add(db.CreateParameter("t" + i, contact.Addresses[i].Type, DbType.Int16));
                    parameter2.Add(db.CreateParameter("p" + i, contact.Addresses[i].IsPrimary, DbType.Boolean));
                }
                sb.AppendLine(";");
            }
            sb.AppendLine("delete from phones where owner_id=@contactId;");
            if (contact.Phones.Count > 0)
            {
                sb.AppendLine("insert into phones (owner_id, phone_number, phone_type, is_primary) values ");
                for (int i = 0; i < contact.Phones.Count; i++)
                {
                    if (i > 0) sb.Append(",");
                    sb.Append("(@contactId, @pn").Append(i).Append(", @pt").Append(i).Append(", @pp").Append(i).Append(")");
                    parameter2.Add(db.CreateParameter("pn" + i, contact.Phones[i].Number.Trim(), DbType.String));
                    parameter2.Add(db.CreateParameter("pt" + i, contact.Phones[i].Type, DbType.Int16));
                    parameter2.Add(db.CreateParameter("pp" + i, contact.Phones[i].IsPrimary, DbType.Boolean));
                }
                sb.AppendLine(";");
            }
            sb.AppendLine("delete from emails where owner_id=@contactId;");
            if (contact.Emails.Count > 0)
            {
                sb.AppendLine("insert into emails (owner_id, email_address, email_type, is_primary) values ");
                for (int i = 0; i < contact.Emails.Count; i++)
                {
                    if (i > 0) sb.Append(",");
                    sb.Append("(@contactId, @ea").Append(i).Append(", @et").Append(i).Append(", @ep").Append(i).Append(")");
                    parameter2.Add(db.CreateParameter("ea" + i, contact.Emails[i].Address.Trim(), DbType.String));
                    parameter2.Add(db.CreateParameter("et" + i, contact.Emails[i].Type, DbType.Int16));
                    parameter2.Add(db.CreateParameter("ep" + i, contact.Emails[i].IsPrimary, DbType.Boolean));
                }
                sb.AppendLine(";");
            }
            success = await db.ExecuteNonQueryAsync(sb.ToString(), parameter2.ToArray());
            return success
                ? Results.Ok(CommonResult.Ok("Supplier successfully updated."))
                : Results.Problem("An error occured while updating the supplier. Please try again later.");

        }
        private static async Task<IResult> DeleteAsync(short id, IDatabase db, HttpContext context)
        {
            var commandText = "update contacts set is_deleted = true, editor_id=@editorId, edited_date=current_timestamp where contact_id = @contactId and is_deleted = false;";
            var parameters = new DbParameter[]
            {
                db.CreateParameter("contactId", id, DbType.Int16),
                db.CreateParameter("editorId", Application.GetUserID(context), DbType.Int16)
            };
            var success = await db.ExecuteNonQueryAsync(commandText, parameters);
            return success
                ? Results.Ok(CommonResult.Ok("Supplier successfully deleted."))
                : Results.Problem("An error occured while deleting the supplier. Please try again later.");
        }
    }
}
