using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Astro.Server.Middlewares;
using Astro.Server.Api;
using Astro.Data;
using Astro.Server.Websites;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IDBClient>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var connectionString = config.GetConnectionString("Npgsql") ?? throw new InvalidOperationException("Connection string 'Default' not found.");
    return new PgDbClient(connectionString);
});
builder.Services.AddScoped<TokenValidator>();
builder.Services.AddAuthentication("Bearer")
     .AddJwtBearer(options =>
     {
         options.Events = new JwtBearerEvents
         {
             OnMessageReceived = async context =>
             {
                 var tokenValidator = context.HttpContext.RequestServices.GetRequiredService<TokenValidator>();
                 await tokenValidator.ValidateAsync(context);
                 await Task.CompletedTask;
             }
         };
     });
builder.Services.AddAuthorization();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();

app.MapAuthEndPoints();
app.MapUserEndPoints();
app.MapRoleEndPoints();
app.MapRegionEndPoints();
app.MapProductEndPoints();
app.MapCategoryEndPoints();
app.MapUnitEndPoints();

app.MapSupplierEndPoints();
app.MapCustomerEndPoints();
app.MapEmployeeIendPoints();

app.MapAccountEndPoints();
app.MapAccountProviderEndPoints();

app.MapDocumentEndPoints();
app.MapSqlQueryEndPoints();

app.MapPurchaseEndPoints();

//Websites
app.MapHomeEndPoints();

app.Run();