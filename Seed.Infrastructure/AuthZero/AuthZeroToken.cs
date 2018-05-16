using System;

namespace Seed.Infrastructure.AuthZero
{
    /// <summary>
    /// Token provided by Auth0 Authentication API (POST https://{yourdomain}.auth0.com/oauth/token)
    /// </summary>
    public class AuthZeroToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthZeroToken"/> class.
        /// </summary>
        /// <param name="accessToken"> Access token to be included in the Authorization header as bearer </param>
        /// <param name="expiresInSeconds"> Token expiration in seconds </param>
        public AuthZeroToken(string accessToken, int expiresInSeconds)
        {
            AccessToken = accessToken;
            ExpirationDateTime = DateTimeOffset.UtcNow.AddSeconds(expiresInSeconds);
        }

        /// <summary> Access token to be included in the Authorization header as bearer </summary>
        public string AccessToken { get; }

        /// <summary>
        /// Approximate datetime when this token will expire.
        ///
        /// Approximate because we are calculating it as Now+ExpireIn, that's another reason to configure secondsBeforeTokenExpirationToRenew on <see cref="AuthZeroClient"/>
        /// </summary>
        public DateTimeOffset ExpirationDateTime { get; }
    }
}
