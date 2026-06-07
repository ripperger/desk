using Serilog;

namespace Desk.Middleware
{
    public class ExecutionTimeLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExecutionTimeLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Call the next middleware in the pipeline
            await _next(context);

            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            Log.Information("Request {Method} {Path} executed in {ElapsedMilliseconds}ms",
                context.Request.Method, context.Request.Path, elapsedMs);
        }
    }

}

