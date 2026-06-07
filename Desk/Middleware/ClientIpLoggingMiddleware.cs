using Serilog.Context;
using System.Net;

namespace Desk.Middleware
{
    public class ClientIpLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public ClientIpLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using (LogContext.PushProperty(
                "ClientIp",
               context.Connection.RemoteIpAddress))

            {
                await _next(context);
            }
        }
    }
}
