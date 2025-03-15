using Microsoft.Extensions.Hosting;
using Prometheus;
using System.Diagnostics;

namespace ContatosRegionais.Application.BackgroundServices;

public class MemoryMetricsCollector : BackgroundService
{
    private readonly Gauge _memoryUsage = Metrics
        .CreateGauge("api_memory_usage_mb", "Uso de memória da aplicação em MB");

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Iniciando coleta de métrica Uso de memória...");

        while (!stoppingToken.IsCancellationRequested)
        {
            long memoryUsed = Process.GetCurrentProcess().WorkingSet64 / (1024 * 1024); // Convertendo para MB
            _memoryUsage.Set(memoryUsed);

            await Task.Delay(5000, stoppingToken); // Atualiza a cada 5s
        }
    }
}
