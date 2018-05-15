using Microsoft.AspNetCore.Authorization;
using System;

namespace Seed.Api.Authorization
{
    /// <summary>
    /// Set of authorization policies defined for WebApiCoreSeed
    /// </summary>
    public interface IAuthorizationPolicies
    {
        /// <summary> Authorization policy for actions available only to admins </summary>
        Action<AuthorizationPolicyBuilder> AdminOnly { get; }
    }
}
