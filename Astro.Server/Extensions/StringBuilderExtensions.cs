using Astro.Data;
using Astro.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astro.Server.Extensions
{
    internal static class StringBuilderExtensions
    {
        internal static async Task AppendUserTableAsync(this StringBuilder sb, IDatabase db)
        {
            var commandText = """
                    select u.user_id, concat(u.user_firstname, ' ', u.user_lastname) AS fullname, u.email, r.role_name,
                    case when u.creator_id = 0 then 'System' else concat(c.user_firstname, ' ', c.user_lastname) end as creator, u.created_date
                    from users as u
                    inner join roles as r on u.role_id = r.role_id
                    left join users AS c ON u.creator_id = c.user_id
                    where u.is_deleted = false
                    """;
            sb.Append("<table class=\"data-table\"><thead><tr><th>User ID</th><th>Full Name</th><th>Email</th><th>Role</th><th>Creator</th><th>Created Date</th></tr></thead><tbody>");
            await db.ExecuteReaderAsync(async (DbDataReader reader) =>
            {
                while (await reader.ReadAsync())
                {
                    sb.Append("<tr>");
                    sb.AppendFormat("<td>{0}</td>", reader.GetInt16(0));
                    sb.Append("<td>").Append(System.Web.HttpUtility.HtmlEncode(reader.GetString(1))).Append("</td>");
                    sb.Append("<td>").Append(System.Web.HttpUtility.HtmlEncode(reader.GetString(2))).Append("</td>");
                    sb.Append("<td>").Append(System.Web.HttpUtility.HtmlEncode(reader.GetString(3))).Append("</td>");
                    sb.Append("<td>").Append(System.Web.HttpUtility.HtmlEncode(reader.GetString(4))).Append("</td>");
                    sb.Append("<td>").Append(reader.GetDateTime(5).ToString("dd/MM/yyyy HH:mm")).Append("</td>");
                    sb.Append("</tr>");
                }
            }, commandText);
            sb.Append("</tbody></table>");
        }
        internal static async Task AppendRoleTableAsync(this StringBuilder sb, IDatabase db)
        {
            var commandText = """
                    select r.role_id, r.role_name, case when r.creator_id = 0 then 'System' else concat(c.user_firstname, ' ', c.user_lastname) end as creator, r.created_date
                    FROM roles as r
                    left join users as c on r.creator_id = c.user_id
                    """;
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
        }
        internal static async Task AppendContactTableAsync(this StringBuilder sb, IDatabase db, short contactType)
        {
            var commandText = """
                SELECT c.category_id, c.category_name, c.created_date, CONCAT(u.user_firstname, ' ', u.user_lastname) AS created_by
                FROM categories c
                INNER JOIN users u ON c.creator_id = u.user_id
                WHERE c.is_deleted = false
                ORDER BY c.category_name
                """;

            sb.Append("<table class=\"table table-striped table-bordered\">\n")
              .Append("<thead>\n")
              .Append("<tr>\n")
              .Append("<th>Category ID</th>\n")
              .Append("<th>Category Name</th>\n")
              .Append("<th>Created Date</th>\n")
              .Append("<th>Created By</th>\n")
              .Append("<th colspan=\"2\">Actions</th>\n")
              .Append("</tr>\n")
              .Append("</thead>\n")
              .Append("<tbody>\n");

            await db.ExecuteReaderAsync(async reader =>
            {
                while (await reader.ReadAsync())
                {
                    sb.Append("<tr>");
                    sb.Append("<td>").Append(reader.GetInt16(0)).Append("</td>");                            // category_id
                    sb.Append("<td>").Append(reader.GetString(1)).Append("</td>");                          // category_name
                    sb.Append("<td>").Append(reader.GetDateTime(2).ToString("yyyy-MM-dd HH:mm:ss")).Append("</td>"); // created_date
                    sb.Append("<td>").Append(reader.GetString(3)).Append("</td>");                          // created_by
                    sb.Append("<td><a href=\"/roles/").Append(reader.GetInt16(0)).Append("\">Edit</a></td>");
                    sb.Append("<td><a href=\"/roles/delete/").Append(reader.GetInt16(0)).Append("\">Delete</a></td>");
                    sb.Append("</tr>");
                }
            }, commandText);

            sb.Append("</tbody></table>");
        }
        internal static async Task AppendCategoryTableAsync(this StringBuilder sb, IDatabase db)
        {

        }
        internal static async Task AppendUnitTableAsync(this StringBuilder sb, IDatabase db)
        {

        }
        internal static async Task AppendProductTableAsync(this StringBuilder sb, IDatabase db, Pagination pagination)
        {
            var countCommandText = """
                select count(*) as totalRecords
                from products as p
                    inner join categories as c on p.category_id = c.category_id
                    inner join units as unt on p.unit_id = unt.unit_id
                    inner join users as u on p.creator_id = u.user_id
                where p.is_deleted = false
                """;
            var totalRecords = await db.ExecuteScalarIntegerAsync(countCommandText);
            var columns = new string[]
            {
                "ID", "Product Name", "SKU", "Category", "Stock", "Unit", "Price", "Created By", "Created Date"
            };
            var sortColumns = new string[]
            {
                "p.product_id","p.product_name", "p.product_sku", "c.category_name", "p.stock", "unt.unit_name", "p.price", "u.user_firstname", "p.created_date"
            };
            var commandText = $"""
                select p.product_id, p.product_name, p.product_sku, c.category_name,  p.stock, unt.unit_name, p.price, 
                   	concat(u.user_firstname, ' ', u.user_lastname) as creator, p.created_date
                from products as p
                    inner join categories as c on p.category_id = c.category_id
                    inner join units as unt on p.unit_id = unt.unit_id
                    inner join users as u on p.creator_id = u.user_id
                where p.is_deleted = false
                ORDER by {sortColumns[pagination.OrderBy]} {(pagination.SortOrder == 0 ? "ASC" : "DESC")}
                LIMIT @pagesize OFFSET @offset
                """;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("pagesize", pagination.PageSize, System.Data.DbType.Int32),
                db.CreateParameter("offset", pagination.GetOffset(), System.Data.DbType.Int32)
            };
            
            sb.Append($"""
                <div class="responsive">
                <table class="table table-pagination" page="{pagination.Page}" page-size="{pagination.PageSize}" order-column="{pagination.OrderBy}" sort-order="{pagination.SortOrder}" search="{pagination.Search}" >
                <thead>
                <tr>
                """);
            for (int i = 0; i < columns.Length; i++)
            {
                sb.Append("<th><span class=\"table-header\">").Append(columns[i]);
                if (pagination.OrderBy == i)
                {
                    sb.Append("&nbsp;&nbsp;<small>");
                    sb.Append(pagination.SortOrder == 0 ? " &#9650" : " &#9660");
                    sb.Append("</small>");
                }
                sb.Append("</span></th>");
            }
            sb.Append("""
                </tr>
                </thead>
                <tbody>
                """);
            await db.ExecuteReaderAsync(async reader =>
            {
                while (await reader.ReadAsync())
                {
                    sb.Append("<tr>");
                    sb.Append("<td>").Append(reader.GetInt32(0)).Append("</td>"); // product_id
                    sb.Append("<td>").Append(reader.GetString(1)).Append("</td>"); // product_name
                    sb.Append("<td>").Append(reader.GetString(2)).Append("</td>"); // product_sku
                    sb.Append("<td>").Append(reader.GetString(3)).Append("</td>"); // category_name
                    sb.Append("<td>").Append(reader.GetInt32(4)).Append("</td>"); // stock
                    sb.Append("<td>").Append(reader.GetString(5)).Append("</td>"); // unit_name
                    sb.Append("<td>").Append(reader.GetDecimal(6).ToString("N0")).Append("</td>"); // price
                    sb.Append("<td>").Append(reader.GetString(7)).Append("</td>"); // creator
                    sb.Append("<td>").Append(reader.GetDateTime(8).ToString("yyyy-MM-dd HH:mm")).Append("</td>");
                    sb.Append("</tr>");
                }
            }, commandText, parameters);
            sb.Append("</tbody></table>");
            sb.Append("<span><strong>Total Records: </strong>");
            sb.Append(totalRecords.Value.ToString("N0")).Append("</span>");
            sb.Append("<nav aria-label=\"Page navigation\"><ul class=\"pagination\">");

            int iPage = 1;
            while (totalRecords > 0)
            {
                totalRecords -= pagination.PageSize;
                sb.Append("<li class=\"page-item");
                if (iPage == pagination.Page)
                {
                    sb.Append(" active\"><span class=\"page-link\">").Append(iPage).Append("<span></li>");
                }
                else
                {
                    sb.Append("\"><button class=\"page-link\">").Append(iPage).Append("</button></li>");
                }
                
                iPage++;
            }
            sb.Append("</ul></nav>");
            sb.Append("</div>");
        }
        internal static void AppendLayout(this StringBuilder sb)
        {

        }
        internal static void AppendBreadCrumb(this StringBuilder sb, string activeItem, params string[] items)
        {
            sb.Append("<nav aria-label=\"breadcrumb\"><ol class=\"breadcrumb\">");
            foreach (var item in items)
            {
                sb.Append("<li class=\"breadcrumb-item\"");
                if (activeItem == item)
                {
                    sb.Append(" active\">").Append("<span>").Append(item).Append("</span></li>");
                }
                else
                {
                    sb.Append("\"><a href=\"#\">").Append(item).Append("</a></li>");
                }
                
            }
            sb.Append("</ol></nav>");
        }
        internal static void AppendStyle(this StringBuilder sb, string rel, string href)
        {
            sb.Append("<link href=\"").Append(href).Append("\" rel=\"").Append(rel).Append("\"/>");
        }
        internal static void AppendScript(this StringBuilder sb, string src, string type = "text/javascript")
        {
            sb.Append("<script src=\"").Append(src).Append("\"");
            if (string.IsNullOrWhiteSpace(type))
            {
                sb.Append(" type=\"").Append(type).Append("\"");
            }
            sb.Append("/>");
        }
        internal static void AppendMetadata(this StringBuilder sb, string name, string content)
        {
            sb.Append("<meta name=\"").Append(name).Append("\" content=\"").Append(content).Append("/>");
        }
    }
}
