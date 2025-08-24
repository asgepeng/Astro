using Astro.Data;
using Astro.Models;
using Astro.Server.Binaries;
using Astro.Server.Extensions;
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
    internal static class Suppliers
    {
        internal static void MapSupplierEndPoints(this WebApplication app)
        {
            app.MapGet("/data/suppliers", GetAllAsync).RequireAuthorization();
            app.MapGet("/data/suppliers/{id}", GetByIdAsync).RequireAuthorization();
            app.MapPost("/data/suppliers", CreateAsync).RequireAuthorization();
            app.MapPut("/data/suppliers", UpdateAsync).RequireAuthorization();
            app.MapDelete("/data/suppliers/{id}", DeleteAsync).RequireAuthorization();
        }

        private static async Task<IResult> GetAllAsync(IDBClient db, HttpContext context)
        {
            var commandText = """
                SELECT c.contactid, 
                       c.name, 
                       COALESCE(ca.streetaddress || ' ' || v.name || ' ' || d.name || ' ' || cty.name || ', ' || ca.zipcode, '') AS address,
                       COALESCE(p.phonenumber, '') as phonenumber, 
                       creator.fullname, 
                       c.createddate
                FROM contacts c
                LEFT JOIN addresses ca 
                       ON c.contactid = ca.contactid AND ca.isprimary = true
                LEFT JOIN villages v 
                       ON ca.villageid = v.villageid
                LEFT JOIN districts d 
                       ON v.districtid = d.districtid
                LEFT JOIN cities cty 
                       ON d.cityid = cty.cityid
                LEFT JOIN phones p 
                       ON c.contactid = p.contactid AND p.isprimary = true
                INNER JOIN employees creator 
                       ON c.creatorid = creator.employeeid
                WHERE c.isdeleted = false 
                  AND c.contacttype = 0;
                """;
            return Results.File(await db.ExecuteBinaryTableAsync(commandText), "application/octet-stream");
        }
        private static async Task<IResult> GetByIdAsync(short id, IDBClient db, HttpContext context)
        {
            var commandText = """
                SELECT contactid, name
                FROM contacts
                WHERE contactid = @contactId
                AND isdeleted = false
                """;
            using (var writer = new Streams.Writer())
            {
                bool contactExists = false;
                await db.ExecuteReaderAsync(async reader =>
                {
                    contactExists = reader.HasRows;
                    writer.WriteBoolean(contactExists);
                    if (await reader.ReadAsync())
                    {
                        writer.WriteInt16(reader.GetInt16(0));
                        writer.WriteString(reader.GetString(1));
                    }
                }, commandText, db.CreateParameter("contactId", id, DbType.Int16));
                if (!contactExists) return Results.File(writer.ToArray(), "application/octet-stream");

                commandText = """
                SELECT a.addressid, a.streetaddress, v.villageid, v.name AS villagename, d.districtid, d.name AS districtname,
                c.cityid, c.name AS cityname, s.stateid, s.name AS statename, a.addresstype, a.isprimary, a.zipcode
                FROM addresses AS a
                	INNER JOIN villages AS v ON a.villageid = v.villageid
                	INNER JOIN districts AS d ON v.districtid = d.districtid
                	INNER JOIN cities AS c on d.cityid = c.cityid
                	INNER JOIN states AS s ON c.stateid = s.stateid	
                WHERE a.contactid = @contactId
                """;
                var iPos = writer.ReserveInt32();
                var iCount = 0;
                await db.ExecuteReaderAsync(async reader =>
                {
                    while (await reader.ReadAsync())
                    {
                        //address id: int, streetaddress: string
                        writer.WriteInt32(reader.GetInt32(0));
                        writer.WriteString(reader.GetString(1));

                        //village id: int64, villagename : string
                        writer.WriteInt64(reader.GetInt64(2));
                        writer.WriteString(reader.GetString(3));

                        //district id: int, districtname : string
                        writer.WriteInt32(reader.GetInt32(4));
                        writer.WriteString(reader.GetString(5));

                        //city id: int, cityname : string
                        writer.WriteInt32(reader.GetInt32(6));
                        writer.WriteString(reader.GetString(7));

                        //state id: short, statename : string
                        writer.WriteInt16(reader.GetInt16(8));
                        writer.WriteString(reader.GetString(9));

                        //address type, isprimary, zipcode
                        writer.WriteInt16(reader.GetInt16(10));
                        writer.WriteBoolean(reader.GetBoolean(11));
                        writer.WriteString(reader.GetString(12));
                        iCount++;
                    }
                }, commandText, db.CreateParameter("contactId", id, DbType.Int16));
                writer.WriteInt32(iCount, iPos);

                commandText = """
                    select phoneid, phonenumber, phonetype, isprimary
                    from phones as p
                    where contactid = @contactId
                    """;

                iCount = 0;
                iPos = writer.ReserveInt32();
                await db.ExecuteReaderAsync(async reader =>
                {
                    while (await reader.ReadAsync())
                    {
                        writer.WriteInt32(reader.GetInt32(0));
                        writer.WriteString(reader.GetString(1));
                        writer.WriteInt16(reader.GetInt16(2));
                        writer.WriteBoolean(reader.GetBoolean(3));
                        iCount++;
                    }
                }, commandText, db.CreateParameter("contactId", id, DbType.Int16));
                writer.WriteInt32(iCount, iPos);

                iCount = 0;
                iPos = writer.ReserveInt32();
                commandText = """
                    select emailid, emailaddress, emailtype, isprimary
                    from emails
                    where contactid = @contactId
                    """;
                await db.ExecuteReaderAsync(async reader =>
                {
                    while (await reader.ReadAsync())
                    {
                        writer.WriteInt32(reader.GetInt32(0));
                        writer.WriteString(reader.GetString(1));
                        writer.WriteInt16(reader.GetInt16(2));
                        writer.WriteBoolean(reader.GetBoolean(3));
                        iCount++;
                    }
                }, commandText, db.CreateParameter("contactId", id, DbType.Int16));
                writer.WriteInt32(iCount, iPos);

                return Results.File(writer.ToArray(), "application/octet-stream");
            }
        }
        private static async Task<IResult> CreateAsync(Contact contact, IDBClient db, HttpContext context)
        {
            var commandText = """
                INSERT INTO contacts (name, contacttype, creatorid)
                VALUES (@contactName, @contactType, @creatorId)
                RETURNING contactid
                """;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("contactName", contact.Name.Trim(), DbType.String),
                db.CreateParameter("contactType", (short)0, DbType.Int16),
                db.CreateParameter("creatorId", Extensions.Application.GetUserID(context), DbType.Int16)
            };
            var contactId = await db.ExecuteScalarAsync<short>(commandText, parameters);
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
                    insert into addresses (contactid, streetaddress, villageid, zipcode, addresstype, isprimary)
                    values
                    """);
                for (int i = 0; i < contact.Addresses.Count; i++)
                {
                    if (i > 0) sb.Append(",");
                    sb.Append("(@contactId, @a").Append(i).Append(",").Append("@c").Append(i).Append(",@t").Append(i).Append(",@p").Append(i).Append(")");
                    parameters2.Add(db.CreateParameter("a" + i, contact.Addresses[i].StreetAddress.Trim(), DbType.String));
                    parameters2.Add(db.CreateParameter("c" + i, contact.Addresses[i].Village, DbType.Int64));
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
                for (short i=0; i < contact.Phones.Count; i++)
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
        private static async Task<IResult> UpdateAsync(Contact contact, IDBClient db, HttpContext context)
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
                db.CreateParameter("editor", Extensions.Application.GetUserID(context), DbType.Int16),
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
                for (int i=0; i < contact.Emails.Count; i++)
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
        private static async Task<IResult> DeleteAsync(short id, IDBClient db, HttpContext context)
        {
            var commandText = "update contacts set is_deleted = true, editor_id=@editorId, edited_date=current_timestamp where contact_id = @contactId and is_deleted = false;";
            var parameters = new DbParameter[]
            {
                db.CreateParameter("contactId", id, DbType.Int16),
                db.CreateParameter("editorId", Extensions.Application.GetUserID(context), DbType.Int16)
            };
            var success = await db.ExecuteNonQueryAsync(commandText, parameters);
            return success
                ? Results.Ok(CommonResult.Ok("Supplier successfully deleted."))
                : Results.Problem("An error occured while deleting the supplier. Please try again later.");
        }
    }
}
