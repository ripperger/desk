using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Desk.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Handle the exception
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "text/html";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // Log the exception
            //Log.Error(exception, "An unhandled exception occurred while processing the request.");

            // Return a custom error response
            //return context.Response.WriteAsync("<h1>An unexpected error occurred.</h1>");
            context.Response.Redirect($"{context.Request.PathBase.Value}/Error/{context.Response.StatusCode}");
            return Task.CompletedTask;
        }
    }

}
