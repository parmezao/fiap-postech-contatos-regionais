using ContatosRegionais.Application.Consumers;
using ContatosRegionais.Domain.Entities;
using ContatosRegionais.Domain.Interfaces;
using ContatosRegionais.Infra.Data.Context;
using ContatosRegionais.Infra.Data.Repository;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Polly;

namespace ContatosRegionais.Application.Extensions;

public static class ConsumerExtensions
{
    public static void AddInjections(this IServiceCollection services, IConfiguration configuration)
    {
        //Data
        services.AddDbContext<SqlServerDbContext>(options => options
            .UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        //Repo
        services.AddScoped<IBaseRepository<Contato>, BaseRepository<Contato>>();

        const string serviceName = "InfraConsumer";

        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {

                var rabbitMqHost = configuration["RabbitMQ:RABBITMQ_HOST"];
                var rabbitMqUser = configuration["RabbitMQ:RABBITMQ_USER"] ?? string.Empty;
                var rabbitMqPassword = configuration["RabbitMQ:RABBITMQ_PASSWORD"] ?? string.Empty;

                //cfg.Host(new Uri("rabbitmq://localhost:5672/"), h =>
                cfg.Host(rabbitMqHost, h =>
                {
                    h.Username(rabbitMqUser);
                    h.Password(rabbitMqPassword);
                });

                // Configuração de fila específica para InsertContatoConsumer
                cfg.ReceiveEndpoint("insert-contact-queue", e =>
                {
                    e.ConfigureConsumer<InsertContatoConsumer>(context);
                });

                // Configuração de fila específica para UpdateContatoConsumer
                cfg.ReceiveEndpoint("update-contact-queue", e =>
                {
                    e.ConfigureConsumer<UpdateContatoConsumer>(context);
                });

                // Configuração de fila específica para DeleteContatoConsumer
                cfg.ReceiveEndpoint("delete-contact-queue", e =>
                {
                    e.ConfigureConsumer<DeleteContatoConsumer>(context);
                });

                cfg.ConfigureEndpoints(context);
            });

            x.AddConsumer<InsertContatoConsumer>();
            x.AddConsumer<UpdateContatoConsumer>();
            x.AddConsumer<DeleteContatoConsumer>();
        });



        // Configuração do OpenTelemetry
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


        // Configuração de Polly para RabbitMQ com retries
        services.AddSingleton<IAsyncPolicy>(policy =>
        {
            return Policy.Handle<Exception>()
                         .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        });

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
