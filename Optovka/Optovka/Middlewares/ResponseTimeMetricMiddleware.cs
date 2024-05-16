using Prometheus;
using System.Diagnostics;

namespace Optovka
{
    public class ResponseTimeMetricMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly Histogram _requestDurationTimeHistogram = Metrics.CreateHistogram(
            "http_request_duration_time",
            "Reports request duration time",
            new HistogramConfiguration
            {
                Buckets = new[] {0.02, 0.05, 0.1, 0.15, 0.2, 0.3, 0.5, 1, 1.5, 3},
                LabelNames = new string[] { "status_code", "method", "path" }
            });

        public ResponseTimeMetricMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;
            var method = context.Request.Method.ToString();
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                await _next(context);
            }
            finally
            {
                stopWatch.Stop();
                var duration = stopWatch.Elapsed.TotalSeconds;
                var statusCode = context.Response.StatusCode.ToString();
                _requestDurationTimeHistogram.WithLabels(statusCode, method, path).Observe(duration);
            }
        }
    }
}
