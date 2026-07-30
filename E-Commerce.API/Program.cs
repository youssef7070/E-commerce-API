using E_Commerce.API.Extensions;
using E_Commerce.Application.Common;
using E_Commerce.Infrastructure;
using E_Commerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddLogging(logging => logging.AddConsole().SetMinimumLevel(LogLevel.Debug));

try
{
    builder.Services.AddInfrastructureServices(builder.Configuration);
}
catch (InvalidOperationException ex)
{
    var tempProvider = builder.Services.BuildServiceProvider();
    var logger = tempProvider.GetRequiredService<ILogger<Program>>();
    logger.LogWarning("Infrastructure initialization warning: {Message}", ex.Message);
}

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<PaymentGetwaySettings>(builder.Configuration.GetSection("Stripe"));


var app = builder.Build();

// Apply migrations and seed data (non-critical)
_ = Task.Run(async () =>
{
    await Task.Delay(1000); // Delay to ensure app is ready
    try
    {
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();

            logger.LogInformation("Starting database migration...");

            var dbContext = services.GetRequiredService<StoreDbContext>();

            try
            {
                logger.LogInformation("Applying migrations...");
                dbContext.Database.Migrate();
                logger.LogInformation("Migrations applied successfully.");

                logger.LogInformation("Starting data seeding...");
                await app.SeedDataBaseAsync();
                logger.LogInformation("Data seeding completed.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Database operation failed: {Message}", ex.Message);
            }
        }
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Background database initialization failed: {Message}", ex.Message);
    }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "Files")),
    RequestPath = "/Files"
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();