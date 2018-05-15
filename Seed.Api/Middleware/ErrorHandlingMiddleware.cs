using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Seed.Api.Models;
using Seed.Domain.Exceptions;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Seed.Api.Middleware
{
    /// <summary>
    /// Middleware responsible for catching unhandled exceptions and provide useful/meaningful responses to API consumers
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorHandlingMiddleware"/> class.
        /// </summary>
        /// <param name="next"> Next action on the request processing pipeline </param>
        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Performs the specific action of this middleware. Invoke is executed internally by AspNet Core framework
        /// </summary>
        /// <param name="context"> Http context for the request being processed </param>
        /// <returns> No object or value is returned by this method when it completes </returns>
        public async Task Invoke(HttpContext context)
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

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected

            if (exception is UnauthorizedException) code = HttpStatusCode.Unauthorized;
            // else if (exception is SomeOtherException) code = HttpStatusCode.RequestTimeout;
            // else if (exception is SomeOtherException2) code = HttpStatusCode.BadRequest;

            var result = JsonConvert.SerializeObject(new ErrorDto(exception.Message));
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
