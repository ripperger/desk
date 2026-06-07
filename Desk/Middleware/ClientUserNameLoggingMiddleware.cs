using Serilog.Context;

namespace Desk.Middleware
{
    public class ClientUserNameLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public ClientUserNameLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var username = context.User?.Identity?.Name ?? "Anonymous";
            using (LogContext.PushProperty("UserName", username))
            {
                await _next(context);
            }
        }
    }
}
