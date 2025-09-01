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
    internal static class Customers
    {
        internal static void MapCustomerEndPoints(this WebApplication app)
        {
            app.MapGet("/data/customers/{id}", GetByIdAsync).RequireAuthorization();
            app.MapPost("/data/customers", async Task<IResult>(IDBClient db, HttpContext context) =>
            {
                using (var stream = await context.Request.GetMemoryStreamAsync())
                using (var reader = new Streams.Reader(stream))
                {
                    var requestType = reader.ReadByte();
                    if (requestType == 0x00) return await GetDataTableAsync(reader, db, context);
                    else if (requestType == 0x01) return await CreateAsync(reader, db, context);
                    else return Results.BadRequest();
                }
            }).RequireAuthorization();
            app.MapPut("/data/customers", UpdateAsync).RequireAuthorization();
            app.MapDelete("/data/customers/{id}", DeleteAsync).RequireAuthorization();
        }
        private static async Task<IResult> GetDataTableAsync(Streams.Reader reader, IDBClient db, HttpContext context)
        {
            var listingType = reader.ReadByte();
            if (listingType == 0x00)
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
                  AND c.contacttype = 1;
                """;
                return Results.File(await db.ExecuteBinaryTableAsync(commandText), "application/octet-stream");
            }
            else if (listingType == 0x01)
            {
                return Results.BadRequest();
            }
            else return Results.BadRequest();
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
                    SELECT emailid, emailaddress, emailtype, isprimary
                    FROM emails
                    WHERE contactid = @contactId
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
        private static async Task<IResult> CreateAsync(Streams.Reader reader, IDBClient db, HttpContext context)
        {
            var commandText = """
                INSERT INTO contacts ("name", contacttype, creatorid)
                VALUES (@name, @type, @creator)
                RETURNING contactid
                """;
            var contactId = reader.ReadInt32();
            if (contactId > 0) return Results.BadRequest();

            var parameters = new DbParameter[]
            {
                db.CreateParameter("name", reader.ReadString(), DbType.AnsiString),
                db.CreateParameter("type", (short)0, DbType.Int16),
                db.CreateParameter("creator", context.GetUserID(), DbType.Int16)
            };
            contactId = await db.ExecuteScalarAsync<short>(commandText, parameters);
            if (contactId == default(short)) return Results.Problem("An error occured while creating the contact. Please try again later.");

            var sb = new StringBuilder();
            var listParameter = new List<DbParameter>()
            {
                db.CreateParameter("contactId", contactId,DbType.Int16)
            };
            var addressCount = reader.ReadInt32();
            if (addressCount > 0)
            {
                sb.Append("""
                    INSERT INTO addresses 
                    (contactid, streetaddress, villageid, zipcode, addresstype, isprimary)
                    VALUES 
                    """);
                for (int i = 0; i < addressCount; i++)
                {
                    if (i > 0) sb.Append(",");
                    sb.Append("(@contactId, @a").Append(i).Append(",").Append("@v").Append(i).Append(",@z").Append(i).Append(",@t").Append(i).Append(",@p").Append(i).Append(")");
                    listParameter.Add(db.CreateParameter("a" + i, reader.ReadString(), DbType.AnsiString));
                    listParameter.Add(db.CreateParameter("v" + i, reader.ReadInt64(), DbType.Int64));
                    listParameter.Add(db.CreateParameter("z" + i, reader.ReadString(), DbType.AnsiString));
                    listParameter.Add(db.CreateParameter("t" + i, reader.ReadInt16(), DbType.Int16));
                    listParameter.Add(db.CreateParameter("p" + i, reader.ReadBoolean(), DbType.Boolean));
                }
                sb.AppendLine(";");
            }
            var phoneCount = reader.ReadInt32();
            if (phoneCount > 0)
            {
                sb.Append("""
                    INSERT INTO phones (contactid, phonenumber, phonetype, isprimary)
                    VALUES
                    """);
                for (short i = 0; i < phoneCount; i++)
                {
                    if (i > 0) sb.Append(",");
                    sb.Append("(@contactId, @pn").Append(i).Append(", @pt").Append(i).Append(", @pp").Append(i).Append(")");
                    listParameter.Add(db.CreateParameter("pn" + i, reader.ReadString(), DbType.String));
                    listParameter.Add(db.CreateParameter("pt" + i, reader.ReadInt16(), DbType.Int16));
                    listParameter.Add(db.CreateParameter("pp" + i, reader.ReadBoolean(), DbType.Boolean));
                }
                sb.AppendLine(";");
            }
            var emailCount = reader.ReadInt32();
            if (emailCount > 0)
            {
                sb.Append("""
                    INSERT INTO emails (contactid, emailaddress, emailtype, isprimary)
                    values
                    """);
                for (short i = 0; i < emailCount; i++)
                {
                    if (i > 0) sb.Append(",");
                    sb.Append("(@contactId, @ea").Append(i).Append(",@et").Append(i).Append(",@ep").Append(i).Append(")");
                    listParameter.Add(db.CreateParameter("ea" + i, reader.ReadString().Trim(), DbType.String));
                    listParameter.Add(db.CreateParameter("et" + i, reader.ReadInt16(), DbType.Int16));
                    listParameter.Add(db.CreateParameter("ep" + i, reader.ReadBoolean(), DbType.Boolean));
                }
                sb.AppendLine(";");
            }
            if (addressCount == 0 && phoneCount == 0 && emailCount == 0)
            {
                return Results.Ok(CommonResult.Ok("Supplier successfully created"));
            }
            var success = await db.ExecuteNonQueryAsync(commandText, listParameter.ToArray());
            return success ? Results.Ok(CommonResult.Ok("Supplier successfully created")) : Results.Problem("ERROR 2: An error occured while creating the supplier. Please try again later.");
        }
        private static async Task<IResult> UpdateAsync(IDBClient db, HttpContext context)
        {
            using (var stream = await context.Request.GetMemoryStreamAsync())
            using (var reader = new Streams.Reader(stream))
            {
                if (reader.ReadByte() != 0x01) return Results.BadRequest();

                var contactId = reader.ReadInt32();
                var commandText = """
                UPDATE contacts
                SET name = @contactName, editeddate = current_timestamp, editorid = @editor
                WHERE contactid = @contactId and isdeleted = false
                """;
                var parameters = new DbParameter[]
                {
                    db.CreateParameter("contactId", contactId, DbType.Int32),
                    db.CreateParameter("contactName", reader.ReadString(), DbType.AnsiString),
                    db.CreateParameter("editor", context.GetUserID(), DbType.Int16),
                };
                var success = await db.ExecuteNonQueryAsync(commandText, parameters);
                if (!success) return Results.Problem("An error occured while updating the supplier. Please try again later.");

                var sb = new StringBuilder();
                var listParameter = new List<DbParameter>()
                {
                    db.CreateParameter("contactId", contactId,DbType.Int16)
                };
                sb.AppendLine("""
                    DELETE FROM addresses WHERE contactid = @contactId;
                    DELETE FROM phones WHERE contactid = @contactId;
                    DELETE FROM emails WHERE contactid = @contactId;
                    """);
                var addressCount = reader.ReadInt32();
                if (addressCount > 0)
                {
                    sb.Append("""
                    INSERT INTO addresses 
                    (contactid, streetaddress, villageid, zipcode, addresstype, isprimary)
                    VALUES 
                    """);
                    for (int i = 0; i < addressCount; i++)
                    {
                        if (i > 0) sb.Append(",");
                        sb.Append("(@contactId, @a").Append(i).Append(",@v").Append(i).Append(",@z").Append(i).Append(",@t").Append(i).Append(",@p").Append(i).Append(")");
                        listParameter.Add(db.CreateParameter("a" + i, reader.ReadString(), DbType.AnsiString));
                        listParameter.Add(db.CreateParameter("v" + i, reader.ReadInt64(), DbType.Int64));
                        listParameter.Add(db.CreateParameter("z" + i, reader.ReadString(), DbType.AnsiString));
                        listParameter.Add(db.CreateParameter("t" + i, reader.ReadInt16(), DbType.Int16));
                        listParameter.Add(db.CreateParameter("p" + i, reader.ReadBoolean(), DbType.Boolean));
                    }
                    sb.AppendLine(";");
                }
                var phoneCount = reader.ReadInt32();
                if (phoneCount > 0)
                {
                    sb.Append("""
                    INSERT INTO phones (contactid, phonenumber, phonetype, isprimary)
                    VALUES
                    """);
                    for (short i = 0; i < phoneCount; i++)
                    {
                        if (i > 0) sb.Append(",");
                        sb.Append("(@contactId, @pn").Append(i).Append(", @pt").Append(i).Append(",@pp").Append(i).Append(")");
                        listParameter.Add(db.CreateParameter("pn" + i, reader.ReadString(), DbType.String));
                        listParameter.Add(db.CreateParameter("pt" + i, reader.ReadInt16(), DbType.Int16));
                        listParameter.Add(db.CreateParameter("pp" + i, reader.ReadBoolean(), DbType.Boolean));
                    }
                    sb.AppendLine(";");
                }
                var emailCount = reader.ReadInt32();
                if (emailCount > 0)
                {
                    sb.Append("""
                    INSERT INTO emails (contactid, emailaddress, emailtype, isprimary)
                    values
                    """);
                    for (short i = 0; i < emailCount; i++)
                    {
                        if (i > 0) sb.Append(",");
                        sb.Append("(@contactId, @ea").Append(i).Append(",@et").Append(i).Append(",@ep").Append(i).Append(")");
                        listParameter.Add(db.CreateParameter("ea" + i, reader.ReadString().Trim(), DbType.String));
                        listParameter.Add(db.CreateParameter("et" + i, reader.ReadInt16(), DbType.Int16));
                        listParameter.Add(db.CreateParameter("ep" + i, reader.ReadBoolean(), DbType.Boolean));
                    }
                    sb.AppendLine(";");
                }
                success = await db.ExecuteNonQueryAsync(sb.ToString(), listParameter.ToArray());
                return success ? Results.Ok(CommonResult.Ok("Supplier successfully updated")) : Results.Problem("ERROR 2: An error occured while updating the supplier. Please try again later.");
            }
        }
        private static async Task<IResult> DeleteAsync(short id, IDBClient db, HttpContext context)
        {
            var commandText = "UPDATE contacts SET isdeleted = true, editorid=@editorId, editeddate=current_timestamp WHERE contactid = @contactId and isdeleted = false;";
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
