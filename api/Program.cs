using Microsoft.EntityFrameworkCore;
using Orbital.Api.Data;
using Orbital.Api.Infrastructure;
using StackExchange.Redis;
using Microsoft.Extensions.Configuration;
using Hangfire;
using Hangfire.PostgreSql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseHangfireDashboard();
}

// Endpoint mapping phase
app.MapControllers();

app.Run();
