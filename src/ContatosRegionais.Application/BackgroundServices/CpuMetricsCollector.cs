using Microsoft.Extensions.Hosting;
using Prometheus;
using System.Diagnostics;

namespace ContatosRegionais.Application.BackgroundServices;

public class CpuMetricsCollector : BackgroundService
{
    private readonly Gauge _cpuUsage = Metrics.CreateGauge("api_cpu_usage", "Uso de CPU da aplicação em porcentagem");

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Iniciando coleta de métrica Uso de CPU...");

        var process = Process.GetCurrentProcess();
        TimeSpan prevCpuTime = process.TotalProcessorTime;
        DateTime prevTime = DateTime.UtcNow;

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(5000, stoppingToken); // Atualiza a cada 5 segundos
            process.Refresh(); // Atualiza informações do processo

            TimeSpan newCpuTime = process.TotalProcessorTime;
            DateTime newTime = DateTime.UtcNow;

            double cpuUsedMs = (newCpuTime - prevCpuTime).TotalMilliseconds;
            double elapsedMs = (newTime - prevTime).TotalMilliseconds;
            double cpuUsage = cpuUsedMs / elapsedMs * 100 / Environment.ProcessorCount; // Normaliza pelo número de núcleos

            _cpuUsage.Set(cpuUsage);

            prevCpuTime = newCpuTime;
            prevTime = newTime;
        }
    }
}
