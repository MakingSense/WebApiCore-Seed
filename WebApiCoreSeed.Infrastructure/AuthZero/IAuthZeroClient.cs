using Auth0.ManagementApi;
using System.Threading.Tasks;
using WebApiCoreSeed.Infrastructure.Result;

namespace WebApiCoreSeed.Infrastructure.AuthZero
{
    /// <summary>
    /// Handles authentication to Auth0 API
    ///
    /// Auht0 Management API requires a token to be used, and that token expires every 24hs
    /// This interface provides valid tokens to execute requests against Management API
    /// </summary>
    public interface IAuthZeroClient
    {
        /// <summary> Management API client provided by Auth0 </summary>
        /// <returns> A client object to consume Auth0's Management Api </returns>
        Task<Result<IManagementApiClient, ErrorResult>> GetManagementApiClient();

        /// <summary> Obtains an access token to consume Auth0's Management Api  </summary>
        /// <returns> Access Token for Auth0's Management Api</returns>
        Task<Result<AuthZeroToken, ErrorResult>> ObtainManagementApiToken();
    }
}
