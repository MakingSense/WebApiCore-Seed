using Auth0.AuthenticationApi.Models;
using Auth0.Core;
using Auth0.Core.Exceptions;
using Auth0.ManagementApi;
using Seed.Infrastructure.RestClient;
using Seed.Infrastructure.Result;
using System;
using System.Threading.Tasks;

namespace Seed.Infrastructure.AuthZero
{
    public class AuthZeroClient : IAuthZeroClient
    {
        private readonly IRestClient _restClient;

        private readonly string _clientId;
        private readonly string _authZeroSecret;
        private readonly string _domain;

        private readonly int _secondsBeforeTokenExpirationToRenew;

        private AuthZeroToken _authZeroToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthZeroClient"/> class.
        /// </summary>
        /// <param name="restClient"> <see cref="IRestClient"/> instance to perform requests against Auth0 API </param>
        /// <param name="clientId"> Id for Auth0 Non-interactive client </param>
        /// <param name="secret"> Secret for Auth0 Non-interactive client</param>
        /// <param name="domain"> Domain to be used on Auth0 </param>
        /// <param name="secondsBeforeTokenExpirationToRenew">  Threshold in seconds to consider a token is 'about to expire' and force a refresh </param>
        public AuthZeroClient(IRestClient restClient, string clientId, string secret, string domain, int secondsBeforeTokenExpirationToRenew = 300)
        {
            _restClient = restClient;
            _clientId = clientId;
            _authZeroSecret = secret;
            _secondsBeforeTokenExpirationToRenew = secondsBeforeTokenExpirationToRenew;
            _domain = domain;
        }

        /// <inheritdoc/>
        public async Task<Result<IManagementApiClient, ErrorResult>> GetManagementApiClient()
        {
            var obtainTokenResult = await ObtainManagementApiToken();
            if (!obtainTokenResult.IsSuccessResult)
                return new Result<IManagementApiClient, ErrorResult>(obtainTokenResult.ErrorValue);

            var managementApiClient = new ManagementApiClient(obtainTokenResult.SuccessValue.AccessToken, _domain);
            return new Result<IManagementApiClient, ErrorResult>(managementApiClient);
        }

        /// <inheritdoc/>
        public async Task<Result<AuthZeroToken, ErrorResult>> ObtainManagementApiToken()
        {
            var newTokenRequired = _authZeroToken == null || DateTimeOffset.UtcNow > _authZeroToken.ExpirationDateTime.AddSeconds(-_secondsBeforeTokenExpirationToRenew);
            if (!newTokenRequired)
                return new Result<AuthZeroToken, ErrorResult>(_authZeroToken);

            var newTokenResult = await RetrieveNewToken();
            _authZeroToken = newTokenResult.IsSuccessResult ? newTokenResult.SuccessValue : null;
            return newTokenResult;
        }

        private async Task<Result<AuthZeroToken, ErrorResult>> RetrieveNewToken()
        {
            var bodyParameters = new
            {
                grant_type = "client_credentials",
                client_id = _clientId,
                client_secret = _authZeroSecret,
                audience = $"https://{_domain}/api/v2/"
            };

            try
            {
                var response = await _restClient.PostAsync<AccessToken, ApiError>("/oauth/token", bodyParameters);
                return response.IsSuccessResult ?
                    new Result<AuthZeroToken, ErrorResult>(new AuthZeroToken(response.SuccessValue.AccessToken, response.SuccessValue.ExpiresIn)) :
                    new Result<AuthZeroToken, ErrorResult>(new ErrorResult(response.ErrorValue.ErrorCode, response.ErrorValue.Message));
            }
            catch (ApiException e)
            {
                return new Result<AuthZeroToken, ErrorResult>(new ErrorResult(e.ApiError.ErrorCode, e.ApiError.Message));
            }
        }
    }
}
