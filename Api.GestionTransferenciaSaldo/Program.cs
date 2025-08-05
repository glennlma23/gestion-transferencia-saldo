using Api.GestionTransferenciaSaldo.Middlewares;
using Application.Interfaces;
using Application.Services;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "API de Gestión de Transferencias de Saldo",
        Version = "v1",
        Description = "API desarrollada para administrar billeteras digitales, incluyendo la creación, actualización, eliminación y consulta de saldos."
    });

    var apiXml = Path.Combine(AppContext.BaseDirectory, "Api.GestionTransferenciaSaldo.xml");
    options.IncludeXmlComments(apiXml);

    var appXml = Path.Combine(AppContext.BaseDirectory, "Application.xml");
    options.IncludeXmlComments(appXml);
});

// Configurar DbContext con SQL Server

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Inyección de dependencias: Repositorios

builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMovementRepository, MovementRepository>();

// Inyección de dependencias: Servicios

builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<IMovementService, MovementService>();


var app = builder.Build();

// Middleware para desarrollo (Swagger)

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Gestión de Transferencia v1");
    });
}

app.UseExceptionMiddleware();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }