using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Astro.Server.Middlewares;
using Astro.Data;
using Astro.Server;
using Astro.Server.Api;
using Astro.Server.Web;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IDatabase>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var connectionString = config.GetConnectionString("Npgsql") ?? throw new InvalidOperationException("Connection string 'Default' not found.");
    return new NpgsqlDatabase(connectionString);
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

builder.Services.AddWindowsService();
builder.Services.AddHostedService<Worker>();

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
app.MapAccountEndPoints();

app.MapHomeEndPoints();

app.Run();