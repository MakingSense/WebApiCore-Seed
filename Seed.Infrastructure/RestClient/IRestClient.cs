using System.Threading.Tasks;

namespace Seed.Infrastructure.RestClient
{
    /// <summary>
    /// Interface to provide an easy way to perform REST requests
    /// </summary>
    public interface IRestClient
    {
        /// <summary>
        /// Performs a POST request
        /// </summary>
        /// <typeparam name="TResource">Resource type for this REST request</typeparam>
        /// <typeparam name="TError"> Type used to parse error message </typeparam>
        /// <param name="relativeUrl">Relative URL to be used to perform this request</param>
        /// <param name="body">Payload for this POST request</param>
        /// <returns>A <see cref="RestResponse{TResource,TError}"/> object, including response's content and Http status code </returns>
        Task<RestResponse<TResource, TError>> PostAsync<TResource, TError>(string relativeUrl, object body);

        /// <summary>
        /// Performs a GET request
        /// </summary>
        /// <typeparam name="TResource">Resource type for this REST request</typeparam>
        /// <typeparam name="TError"> Type used to parse error message </typeparam>
        /// <param name="relativeUrl">Relative URL to be used to perform this request</param>
        /// <returns>A <see cref="RestResponse{TResource,TError}"/> object, including response's content and Http status code </returns>
        Task<RestResponse<TResource, TError>> GetAsync<TResource, TError>(string relativeUrl);
    }
}
