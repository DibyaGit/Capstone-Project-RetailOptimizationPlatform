using Microsoft.EntityFrameworkCore;
using RetailOptimizationPlatform.Core.Interfaces;
using RetailOptimizationPlatform.Data;
using RetailOptimizationPlatform.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 1. Add services to the container (Enables MVC).
builder.Services.AddControllersWithViews();

// 2. Register our Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Register our Repositories and Services (Dependency Injection)
// This tells the app: "Whenever someone asks for IInventoryRepository, give them the InventoryRepository"
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<RetailOptimizationPlatform.Core.Interfaces.IInventoryService, RetailOptimizationPlatform.Core.Services.InventoryService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<RetailOptimizationPlatform.Core.Interfaces.IAnalyticsService, RetailOptimizationPlatform.Core.Services.AnalyticsService>();

var app = builder.Build();

// Run database migrations and apply stored procedures / triggers on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        logger.LogInformation("Applying migrations on startup...");
        context.Database.Migrate();
        logger.LogInformation("Migrations applied successfully.");

        // Read and apply DatabaseSetup.sql
        var contentRoot = app.Environment.ContentRootPath;
        var scriptPath = Path.Combine(contentRoot, "..", "RetailOptimizationPlatform.Data", "Scripts", "DatabaseSetup.sql");
        if (!File.Exists(scriptPath))
        {
            scriptPath = Path.Combine(contentRoot, "DatabaseSetup.sql");
        }
        if (File.Exists(scriptPath))
        {
            logger.LogInformation("Applying DatabaseSetup.sql scripts...");
            var rawSql = File.ReadAllText(scriptPath);
            var batches = System.Text.RegularExpressions.Regex.Split(
                rawSql, 
                @"^\s*GO\s*$", 
                System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Multiline
            );
            foreach (var batch in batches)
            {
                if (!string.IsNullOrWhiteSpace(batch))
                {
                    // Skip USE statements as EF is already connected to the target database
                    if (batch.TrimStart().StartsWith("USE ", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    context.Database.ExecuteSqlRaw(batch);
                }
            }
            logger.LogInformation("DatabaseSetup.sql scripts executed successfully.");
        }
        else
        {
            logger.LogWarning("DatabaseSetup.sql script not found at path: {ScriptPath}", scriptPath);
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during database migration/initialization.");
    }
}

app.UseMiddleware<RetailOptimizationPlatform.Web.Middleware.GlobalExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();