using ContatosRegionais.Application.Middlewares;
using ContatosRegionais.Infra.Data.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;
using System.Reflection;

namespace ContatosRegionais.Application.Extensions;

public static class ApplicationExtensions
{
    public static IApplicationBuilder ApplyMigrations(this WebApplication app)
    {
        Console.WriteLine("Iniciando Migrations...");

        using var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var context = serviceScope.ServiceProvider.GetService<SqlServerDbContext>();
        context!.Database.Migrate();

        Console.WriteLine("Migrations finalizada!");
        return app;
    }

    public static IApplicationBuilder UseMetrics(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<RequestMetricsMiddleware>(); // Adiciona o monitoramento de latência
        builder.UseMetricServer();
        builder.UseHttpMetrics(); // Coleta métricas de requisições HTTP automaticamente

        return builder;
    }

    public static Assembly GetAssembly()
    {
        return Assembly.GetExecutingAssembly();
    }
}