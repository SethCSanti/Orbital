using Microsoft.EntityFrameworkCore;
using Orbital.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddDbContext<OrbitalDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("OrbitalDb")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Endpoint mapping phase
app.MapControllers();

app.Run();
