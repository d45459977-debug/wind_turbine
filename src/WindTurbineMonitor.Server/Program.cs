using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WindTurbineMonitor.Infrastructure.Data;
using WindTurbineMonitor.Infrastructure.Repositories;
using WindTurbineMonitor.Server.Hubs;
using WindTurbineMonitor.Server.Services;
using WindTurbineMonitor.Core.Interfaces;
using WindTurbineMonitor.Core.Entities;
using WindTurbineMonitor.Core.Enums;
using WindTurbineMonitor.Infrastructure.Services;
using MudBlazor.Services;
using WindTurbineMonitor.Server.Components;

var builder = WebApplication.CreateBuilder(args);

// ===== Database Configuration =====
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection")),
        mysqlOptions =>
        {
            mysqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null);
            mysqlOptions.CommandTimeout(30);
        }));

// Also register as scoped for repositories
builder.Services.AddScoped<AppDbContext>(sp => sp.GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContext());

// ===== Repository Registration =====
builder.Services.AddScoped<ITurbineRepository, TurbineRepository>();

// ===== Sensor Service Registration =====
// Bisa diganti dengan OpcUaSensorService, ModbusSensorService, atau MqttSensorService
builder.Services.AddSingleton<ISensorDataService, SimulatorSensorService>();

// ===== Background Services =====
builder.Services.AddHostedService<TurbineDataBroadcastService>();

// ===== SignalR Configuration =====
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.KeepAliveInterval = TimeSpan.FromSeconds(10);
    options.HandshakeTimeout = TimeSpan.FromSeconds(15);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
});

// ===== Blazor Server Configuration =====
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ===== MudBlazor Configuration =====
builder.Services.AddMudServices();

// ===== SignalR Client Service =====
builder.Services.AddScoped<WindTurbineMonitor.Server.Services.TurbineSignalRService>();

// ===== HTTP Client (untuk API calls jika diperlukan) =====
builder.Services.AddHttpClient();

// ===== CORS Configuration =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ===== Application Insights / Logging (Optional) =====
builder.Services.AddLogging(options =>
{
    options.AddConsole();
    options.AddDebug();
    options.SetMinimumLevel(LogLevel.Information);
});

if (builder.Environment.IsDevelopment())
{
    builder.Logging.SetMinimumLevel(LogLevel.Debug);
}

var app = builder.Build();

// ===== Database Migration & Seeding =====
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var dbContext = services.GetRequiredService<AppDbContext>();

    try
    {
        logger.LogInformation("Applying database migrations...");
        dbContext.Database.Migrate();

        // Seed initial data if needed
        logger.LogInformation("Seeding initial data...");
        await SeedData.Initialize(services);

        logger.LogInformation("Database initialization completed successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while initializing the database");
    }
}

// ===== Configure the HTTP Request Pipeline =====
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseStatusCodePagesWithReExecute("/not-found");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// ===== CORS =====
app.UseCors("AllowAll");

// ===== SignalR Hub Mapping =====
app.MapHub<TurbineHub>("/turbinehub");

// ===== Razor Components =====
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// ===== Run Application =====
app.Run();

// ===== Database Seeder =====
static class SeedData
{
    public static async Task Initialize(IServiceProvider services)
    {
        var context = services.GetRequiredService<AppDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();
        var sensorService = services.GetRequiredService<ISensorDataService>();

        // Check if data already exists
        if (context.Turbines.Any())
        {
            logger.LogInformation("Database already seeded with turbine data");
            return;
        }

        // Create sample turbines
        var turbines = new[]
        {
            new Turbine
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Turbine Alpha-01",
                Location = "Jakarta Coastal Wind Farm",
                Model = "WTG-2000",
                Capacity = 2.5m, // 2.5 MW
                Status = TurbineStatus.Offline,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Turbine
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Turbine Beta-02",
                Location = "Jakarta Coastal Wind Farm",
                Model = "WTG-2000",
                Capacity = 2.5m,
                Status = TurbineStatus.Running,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Turbine
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Turbine Gamma-03",
                Location = "Jakarta Coastal Wind Farm",
                Model = "WTG-3000",
                Capacity = 3.5m,
                Status = TurbineStatus.Running,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        await context.Turbines.AddRangeAsync(turbines);
        await context.SaveChangesAsync();

        // Initialize sensor service with turbines
        if (sensorService is SimulatorSensorService simulator)
        {
            foreach (var turbine in turbines)
            {
                simulator.InitializeTurbine(turbine.Id, turbine.Name);
            }
        }

        // Create initial events
        var events = new[]
        {
            new TurbineEvent
            {
                Id = Guid.NewGuid(),
                TurbineId = turbines[0].Id,
                EventType = "System",
                Message = "Turbine registered and initialized",
                Severity = EventSeverity.Success,
                CreatedAt = DateTime.UtcNow
            },
            new TurbineEvent
            {
                Id = Guid.NewGuid(),
                TurbineId = turbines[1].Id,
                EventType = "System",
                Message = "Turbine started successfully",
                Severity = EventSeverity.Success,
                CreatedAt = DateTime.UtcNow
            },
            new TurbineEvent
            {
                Id = Guid.NewGuid(),
                TurbineId = turbines[2].Id,
                EventType = "System",
                Message = "Turbine started successfully",
                Severity = EventSeverity.Success,
                CreatedAt = DateTime.UtcNow
            }
        };

        await context.TurbineEvents.AddRangeAsync(events);
        await context.SaveChangesAsync();

        // Create initial component statuses
        var componentTypes = new[]
        {
            ComponentType.Tower,
            ComponentType.Nacelle,
            ComponentType.RotorBlade,
            ComponentType.Gearbox,
            ComponentType.Generator
        };

        foreach (var turbine in turbines)
        {
            foreach (var componentType in componentTypes)
            {
                var componentStatus = new ComponentStatus
                {
                    Id = Guid.NewGuid(),
                    TurbineId = turbine.Id,
                    ComponentType = componentType,
                    ComponentName = GetComponentName(componentType),
                    Status = "Healthy",
                    HealthPercentage = 100,
                    LastUpdated = DateTime.UtcNow,
                    Description = "Component in excellent condition"
                };

                await context.ComponentStatuses.AddAsync(componentStatus);
            }
        }

        await context.SaveChangesAsync();

        logger.LogInformation("Successfully seeded {Count} turbines with initial data", turbines.Length);
    }

    private static string GetComponentName(ComponentType componentType)
    {
        return componentType switch
        {
            ComponentType.Tower => "Main Tower",
            ComponentType.Nacelle => "Nacelle",
            ComponentType.RotorBlade => "Rotor Blades",
            ComponentType.Gearbox => "Gearbox",
            ComponentType.Generator => "Generator",
            ComponentType.Brake => "Brake System",
            ComponentType.Pitch => "Pitch System",
            ComponentType.Yaw => "Yaw System",
            ComponentType.Transformer => "Transformer",
            ComponentType.Control => "Control System",
            _ => "Unknown Component"
        };
    }
}
