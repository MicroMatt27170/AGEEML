using Ageeml.Service.Data;
using Ageeml.Service.Extensions;
using Ageeml.Service.Middleware;
using Ageeml.Service.Options;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services
    .AddControllers()
    .AddODataApplication();

builder.Services.AddOptions<DatabaseOptions>()
    .Bind(builder.Configuration.GetSection(DatabaseOptions.SectionName));

var databaseOptions = builder.Configuration
    .GetSection(DatabaseOptions.SectionName)
    .Get<DatabaseOptions>() ?? new DatabaseOptions();

var provider = (databaseOptions.Provider ?? "sqlite").Trim().ToLowerInvariant();
var connectionString = string.IsNullOrWhiteSpace(databaseOptions.ConnectionString)
    ? new DatabaseOptions().ConnectionString
    : databaseOptions.ConnectionString;

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    switch (provider)
    {
        case "sqlite":
            options.UseSqlite(connectionString);
            break;
        case "mysql":
            if (string.IsNullOrWhiteSpace(databaseOptions.ConnectionString))
            {
                throw new InvalidOperationException("MySQL requires a non-empty connection string.");
            }
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            break;
        case "postgresql":
        case "postgres":
            if (string.IsNullOrWhiteSpace(databaseOptions.ConnectionString))
            {
                throw new InvalidOperationException("PostgreSQL requires a non-empty connection string.");
            }
            options.UseNpgsql(connectionString);
            break;
        default:
            throw new InvalidOperationException($"Unsupported database provider '{provider}'.");
    }
});

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference("/scalar");

app.UseHttpsRedirection();
app.UseMiddleware<GlobalExceptionMiddleware>();

app.MapGet("/", () => Results.Redirect("/scalar"));

app.MapControllers();

app.Run();
