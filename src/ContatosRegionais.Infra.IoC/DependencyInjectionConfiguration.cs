using ContatosRegionais.Application.BackgroundServices;
using ContatosRegionais.Application.Interfaces;
using ContatosRegionais.Application.Mapping;
using ContatosRegionais.Application.UseCases;
using ContatosRegionais.Domain.Entities;
using ContatosRegionais.Domain.Interfaces;
using ContatosRegionais.Infra.Data.Context;
using ContatosRegionais.Infra.Data.Repository;
using ContatosRegionais.Service.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace ContatosRegionais.Infra.IoC
{
    public static class DependencyInjectionConfiguration
    {
        public static void AddMessagingInjections(this IServiceCollection services, IConfiguration configuration)
        {
            // Data
            services.AddDbContext<SqlServerDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Repositories
            services.AddScoped<IBaseRepository<Contato>, BaseRepository<Contato>>();
            services.AddScoped<IBaseService<Contato>, BaseService<Contato>>();

            //Services
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddHostedService<CpuMetricsCollector>();
            services.AddHostedService<MemoryMetricsCollector>();

            services.AddScoped<IAuthenticationUseCase, AuthenticationUseCase>();

            services.AddScoped<IContatoPublisher, ContatoPublisher>();
            const string serviceName = "ApiMessaging";

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitMqHost = configuration["RabbitMQ:RABBITMQ_HOST"];
                    var rabbitMqUser = configuration["RabbitMQ:RABBITMQ_USER"] ?? string.Empty;
                    var rabbitMqPassword = configuration["RabbitMQ:RABBITMQ_PASSWORD"] ?? string.Empty;

                    cfg.Host(rabbitMqHost, h =>
                    {
                        h.Username(rabbitMqUser);
                        h.Password(rabbitMqPassword);
                    });

                    // Configuração de Retry
                    cfg.UseMessageRetry(retryConfig =>
                    {
                        retryConfig.Interval(3, TimeSpan.FromSeconds(5)); // Tentar 3 vezes com intervalos de 5 segundos
                    });

                    // Configuração do Circuit Breaker
                    cfg.UseCircuitBreaker(cb =>
                    {
                        cb.TrackingPeriod = TimeSpan.FromMinutes(1);  // Período para acompanhar falhas
                        cb.TripThreshold = 15;  // Número máximo de falhas para abrir o circuito
                        cb.ActiveThreshold = 10; // Número máximo de falhas ativas antes de abrir o circuito
                        cb.ResetInterval = TimeSpan.FromMinutes(2);  // Intervalo para resetar o circuito
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });

            // Adicionar Health Checks
            services.AddHealthChecks()
                .AddCheck("API Health", () =>
                    Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("API está saudável"));


            services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()

            )
            .WithMetrics(metrics =>
            {
                metrics
                    .AddRuntimeInstrumentation()
                    .AddProcessInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddPrometheusExporter();  // Exposição para Prometheus
            });
        }
    }
}
