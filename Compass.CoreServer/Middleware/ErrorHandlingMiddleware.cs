using System;
using System.Net;
using System.Threading.Tasks;
using Compass.CoreServer.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Compass.CoreServer.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context /* other scoped dependencies */)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // TODO: Remove the stacktrace for production environment.
            var error = new ErrorResponse { Error = exception.Message, StackTrace = exception.StackTrace };
            var result = JsonConvert.SerializeObject(error);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsync(result);
        }

    }

}
