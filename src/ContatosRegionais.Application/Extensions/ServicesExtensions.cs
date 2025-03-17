using ContatosRegionais.Application.BackgroundServices;
using ContatosRegionais.Application.Interfaces;
using ContatosRegionais.Application.UseCases;
using ContatosRegionais.Domain.Entities;
using ContatosRegionais.Domain.Interfaces;
using ContatosRegionais.Infra.Data.Context;
using ContatosRegionais.Infra.Data.Repository;
using ContatosRegionais.Service.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Reflection;

namespace ContatosRegionais.Application.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IBaseRepository<Contato>, BaseRepository<Contato>>();
        services.AddScoped<IBaseService<Contato>, BaseService<Contato>>();
        services.AddScoped<IAuthenticationUseCase, AuthenticationUseCase>();

        services.AddHostedService<CpuMetricsCollector>();
        services.AddHostedService<MemoryMetricsCollector>();

        return services;
    }

    public static IServiceCollection AddDbConnection(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddDbContext<SqlServerDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        return services;
    }

    public static IServiceCollection AddDocs(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Cadastro de Contatos", Version = "v1.0" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description =
                "JWT Authorization Header - utilizado com Bearer Authentication.\r\n\r\n" +
                "Digite 'Bearer' [espaço] e então seu token no campo abaixo.\r\n\r\n" +
                "Exemplo (informar sem as aspas): 'Bearer 12345abcdef'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            c.IncludeXmlComments(xmlPath);
        });

        return services;
    }
}


