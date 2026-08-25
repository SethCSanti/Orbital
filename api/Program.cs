using Microsoft.EntityFrameworkCore;
using Orbital.Api.Data;
using Orbital.Api.Infrastructure;
using StackExchange.Redis;
using Hangfire;
using Hangfire.PostgreSql;
using Orbital.Api.Hubs;
using Orbital.Api.Jobs;
using Orbital.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddDbContext<OrbitalDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("OrbitalDb")));

builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(
        builder.Configuration.GetConnectionString("Redis")!));

builder.Services.AddSingleton<IRedisService, RedisService>();
builder.Services.AddOrbitalHttpClients(builder.Configuration);

builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(options =>
        options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("OrbitalDb"))));

builder.Services.AddHangfireServer();
builder.Services.AddSignalR();
builder.Services.AddScoped<IApodSyncJob, ApodSyncJob>();
builder.Services.AddScoped<IAsteroidSyncJob, AsteroidSyncJob>();
builder.Services.AddScoped<IExoplanetSyncJob, ExoplanetSyncJob>();
builder.Services.AddScoped<ITleSyncJob, TleSyncJob>();
builder.Services.AddHostedService<IssSyncJob>();
builder.Services.AddScoped<ILaunchSyncJob, LaunchSyncJob>();
builder.Services.AddScoped<IMissionSyncJob, MissionSyncJob>();
builder.Services.AddScoped<ISpaceStationSyncJob, SpaceStationSyncJob>();
builder.Services.AddScoped<IAstronautService, AstronautService>();
builder.Services.AddScoped<IApodService, ApodService>();
builder.Services.AddScoped<IAsteroidService, AsteroidService>();
builder.Services.AddScoped<ILaunchService, LaunchService>();
builder.Services.AddScoped<IRocketService, RocketService>();
builder.Services.AddScoped<IMissionService, MissionService>();
builder.Services.AddScoped<IExoplanetService, ExoplanetService>();
builder.Services.AddScoped<IIssService, IssService>();
builder.Services.AddScoped<ISpaceStationService, SpaceStationService>();
builder.Services.AddScoped<ISolarSystemService, SolarSystemService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddHttpLogging(options => { });

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{\"error\":\"An unexpected error occurred.\"}");
    });
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseHangfireDashboard();
}

app.UseCors("AllowFrontend");
app.UseHttpLogging();

// Endpoint mapping phase
app.MapControllers();
app.MapHub<PingHub>("/hubs/ping");
app.MapHub<IssHub>("/hubs/iss");
app.MapHub<LaunchHub>("/hubs/launches");

using (var scope = app.Services.CreateScope())
{
    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

    recurringJobManager.AddOrUpdate<IApodSyncJob>(
        "apod-sync",
        job => job.ExecuteAsync(),
        Cron.Daily(12)
    );

    recurringJobManager.AddOrUpdate<IAsteroidSyncJob>(
        "asteroid-sync",
        job => job.ExecuteAsync(),
        Cron.Daily(11)
    );

    recurringJobManager.AddOrUpdate<IExoplanetSyncJob>(
        "exoplanet-sync",
        job => job.ExecuteAsync(),
        Cron.Weekly(DayOfWeek.Monday, 10)
    );

    recurringJobManager.AddOrUpdate<ITleSyncJob>(
        "tle-sync",
        job => job.ExecuteAsync(),
        "0 */6 * * *"
    );

    recurringJobManager.AddOrUpdate<ILaunchSyncJob>(
        "launch-sync",
        job => job.ExecuteAsync(),
        "*/15 * * * *" // every 15 minutes — no Cron.* helper for sub-hourly intervals
    );

    recurringJobManager.AddOrUpdate<IMissionSyncJob>(
        "mission-sync",
        job => job.ExecuteAsync(),
        Cron.Daily(2)
    );

    recurringJobManager.AddOrUpdate<ISpaceStationSyncJob>(
        "spacestation-sync",
        job => job.ExecuteAsync(),
        Cron.Weekly(DayOfWeek.Sunday, 8)
    );
}

app.Run();
