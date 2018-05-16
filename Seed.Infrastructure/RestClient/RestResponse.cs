using Seed.Infrastructure.Result;
using System.Net;

namespace Seed.Infrastructure.RestClient
{
    /// <summary>
    /// Simplified REST response object including content and Http status code
    /// </summary>
    /// <typeparam name="TResponse"> REST resource type </typeparam>
    /// <typeparam name="TError"> Type used to parse error messages </typeparam>
    public class RestResponse<TResponse, TError> : Result<TResponse, TError>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RestResponse{TResponse, TError}"/> class.
        /// </summary>
        /// <param name="httpStatusCode"> Status code of the Http response </param>
        /// <param name="content"> Response object from Http response content </param>
        public RestResponse(HttpStatusCode httpStatusCode, TResponse content) : base(content)
        {
            HttpStatusCode = httpStatusCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RestResponse{TResponse, TError}"/> class.
        /// </summary>
        /// <param name="httpStatusCode"> Status code of the Http response </param>
        /// <param name="error"> Response object from Http response content </param>
        public RestResponse(HttpStatusCode httpStatusCode, TError error) : base(error)
        {
            HttpStatusCode = httpStatusCode;
        }

        /// <summary> Gets status code of the Http response</summary>
        public HttpStatusCode HttpStatusCode { get; }
    }
}
