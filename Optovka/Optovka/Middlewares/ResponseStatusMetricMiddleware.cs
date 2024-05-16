using Prometheus;

namespace Optovka
{
    public class ResponseStatusMetricMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly Counter _responseStatusCounter = Metrics.CreateCounter(
            "http_responses_total",
            "Count response status codes",
            new CounterConfiguration
            {
                LabelNames = new string[] { "status_code", "method", "path" }
            });

        public ResponseStatusMetricMiddleware(RequestDelegate next) 
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;

            await _next(context);

            var statusCode = context.Response.StatusCode.ToString();
            var method = context.Request.Method.ToString();
            _responseStatusCounter.WithLabels(statusCode, method, path).Inc();
        }
    }
}
