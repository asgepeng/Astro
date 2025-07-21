using Astro.Data;
using Astro.Server.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Astro.Server.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        private readonly IDatabase db;
        public HomeController(IDatabase _db) => db = _db;
        [HttpGet]
        public IActionResult Index()
        {
            /*
            var sb = new StringBuilder();
            sb.Append("""
                <!DOCTYPE html>
                <html lang="en">
                <head>
                    <meta charset="utf-8" />
                    <title>Astro Server</title>
                    <meta name="viewport" content="width=device-width, initial-scale=1" />
                    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" />
                    <link rel="stylesheet" href="~/css/site.css" />
                </head>
                <body>
                    <header class="bg-dark text-white p-3 mb-4">
                        <div class="container d-flex justify-content-between align-items-center">
                            <h1 class="h4 mb-0">Astro Server</h1>
                            <nav>
                                <a href="/" class="text-white me-3">Home</a>
                                <a href="/products" class="text-white me-3">Products</a>
                                <a href="/account" class="text-white">Account</a>
                            </nav>
                        </div>
                    </header>

                    <main class="container">
                """);
            var commandText = """
                select p.product_id, p.product_name, p.product_sku, c.category_name,  p.stock, unt.unit_name, p.price, 
                   	concat(u.user_firstname, ' ', u.user_lastname) as creator, p.created_date
                from products as p
                    inner join categories as c on p.category_id = c.category_id
                    inner join units as unt on p.unit_id = unt.unit_id
                    inner join users as u on p.creator_id = u.user_id
                where p.is_deleted = false
                """;
            sb.Append("""
                <table class="table">
                <thead>
                <tr>
                    <th>ID</th>
                    <th>Product Name</th>
                    <th>SKU</th>
                    <th>Category</th>
                    <th>Stock</th>
                    <th>Unit</th>
                    <th>Price</th>
                    <th>Created By</th>
                    <th>Created Date</th>
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
            }, commandText);
            sb.Append("</tbody><.table>");
            sb.Append("""
                    </main>

                    <footer class="bg-light text-center text-muted py-3 mt-5 border-top">
                        <div class="container">
                            &copy; @DateTime.Now.Year - Astro Server
                        </div>
                    </footer>

                    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
                </body>
                </html>
                """);
            */
            var content = """
                <!DOCTYPE html>
                <html lang="en">
                <head>
                    <meta charset="UTF-8">
                    <title>Login</title>
                    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">
                    <style>
                        .table-header {
                            display: inline-flex;
                            align-items: center;
                            cursor: pointer;
                        }
                        .table-header small {
                            color: steelblue;
                        }
                    </style>
                </head>
                <body class="bg-light">
                <div class="container" id="main-container">
                    <div class="row justify-content-center align-items-center" style="height: 100vh;">
                        <div class="col-md-4">
                            <div class="card shadow-lg">
                                <div class="card-header text-center">
                                    <h4>Login</h4>
                                </div>
                                <div class="card-body">
                                    <!-- Tidak pakai <form> -->
                                    <div class="mb-3">
                                        <label for="username" class="form-label">Username</label>
                                        <input type="text" class="form-control" id="username" required autofocus>
                                    </div>
                                    <div class="mb-3">
                                        <label for="password" class="form-label">Password</label>
                                        <input type="password" class="form-control" id="password" required>
                                    </div>
                                    <div class="d-grid">
                                        <button id="loginButton" class="btn btn-primary">Login</button>
                                    </div>
                                </div>
                                <div class="card-footer text-muted text-center small">
                                    &copy; 2025 Your Company
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <script src="/js/login.js"></script>
                </body>
                </html>
                """;
            return Content(content, "text/html");           
        }
        [HttpPost]
        public IActionResult Index(IFormCollection form)
        {
            var username = form["username"].ToString();
            var password = form["password"].ToString();
            
            return Redirect("/web/data/products");
        }
        [Route("web/data/products")]
        public async Task<IActionResult> Products()
        {
            var sb = new StringBuilder();
            sb.Append("""
                <!DOCTYPE html>
                <html lang="en">
                <head>
                    <meta charset="UTF-8">
                    <title>Products</title>
                    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">
                </head>
                <body class="bg-light">
                    <div class="container">
                """);
            sb.AppendBreadCrumb("Product", "Home", "Master Data", "Product");
            await sb.AppendProductTableAsync(db, new Models.Pagination());
            sb.Append("</div><body></html>");
            return Content(sb.ToString(), "text/html");
        }
    }
}
