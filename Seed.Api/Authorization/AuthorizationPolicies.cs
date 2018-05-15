using Microsoft.AspNetCore.Authorization;
using Seed.Data.Enums;
using System;

namespace Seed.Api.Authorization
{
    public class AuthorizationPolicies : IAuthorizationPolicies
    {
        public Action<AuthorizationPolicyBuilder> AdminOnly { get; } = builder => builder.RequireRole(AuthorizationRoles.Admin.ToString());
    }
}
