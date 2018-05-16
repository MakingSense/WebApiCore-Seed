using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Seed.Api.Middleware
{
    /// <summary>
    /// Middleware to handle global User Actions
    /// </summary>
    public class UserMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserMiddleware"/> class.
        /// </summary>
        /// <param name="next"> Next action on the request processing pipeline </param>
        public UserMiddleware(RequestDelegate next)
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

            //Do Something with Context dDatabase CRUD



            await _next(context);

            //try
            //{
            //    await _next(context);
            //}
            //catch (Exception ex)
            //{
            //    await HandleExceptionAsync(context, ex);
            //}
        }

    }
}
