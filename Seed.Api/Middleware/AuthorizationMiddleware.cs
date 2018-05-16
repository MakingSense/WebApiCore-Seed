using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Seed.Api.Models;
using System.Net;
using System.Threading.Tasks;

namespace Seed.Api.Middleware
{
    /// <summary>
    /// Middleware to handle global authorization policies
    /// </summary>
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationMiddleware"/> class.
        /// </summary>
        /// <param name="next"> Next action on the request processing pipeline </param>
        public AuthorizationMiddleware(RequestDelegate next)
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
            var isUserBlocked = context.User.HasClaim(c => c.Type == "UserStatus" && c.Value == "Blocked");
            if (isUserBlocked)
            {
                var message = JsonConvert.SerializeObject(new ErrorDto("Unauthorized"));
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync(message);
            }
            else
            {
                await _next(context);
            }
        }
    }
}
