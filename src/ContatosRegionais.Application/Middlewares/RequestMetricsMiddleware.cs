using Microsoft.AspNetCore.Http;
using Prometheus;
using System.Diagnostics;

namespace ContatosRegionais.Application.Middlewares;

public class RequestMetricsMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    private static readonly Histogram _requestDuration = Metrics
        .CreateHistogram("api_request_duration_seconds", "Duração das requisições HTTP em segundos",
            new HistogramConfiguration
            {
                Buckets = Histogram.ExponentialBuckets(0.01, 2, 10), // Buckets: 10ms até ~10s
                LabelNames = ["method", "endpoint", "status_code"]
            });

    public async Task Invoke(HttpContext context)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            sw.Stop();
            _requestDuration
                .WithLabels(context.Request.Method, context.Request.Path, context.Response.StatusCode.ToString())
                .Observe(sw.Elapsed.TotalSeconds);
        }
    }
}
