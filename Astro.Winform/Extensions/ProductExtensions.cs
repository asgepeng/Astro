using Astro.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Astro.Data;

namespace Astro.Winform.Extensions
{
    internal static class ProductExtensions
    {
        internal static async Task<bool> IsBarcodeUsed(this Product product, IDBClient db)
        {
            var commandText = """
                SELECT 1 FROM products
                WHERE sku = @sku and productid != @productid 
                AND isdeleted = false;
                """;
            var parameters = new DbParameter[]
            {
                db.CreateParameter("productid", product.ID, DbType.Int16),
                db.CreateParameter("sku", product.Sku, DbType.String)
            };
            return await db.HasRecordsAsync(commandText, parameters);
        }
    }
}
