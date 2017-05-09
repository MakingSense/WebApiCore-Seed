using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using WebApiCoreSeed.Data.Enums;

namespace WebApiCoreSeed.WebApi.Authorization
{
    public class AuthorizationPolicies : IAuthorizationPolicies
    {
        public Action<AuthorizationPolicyBuilder> AdminOnly { get; } = builder => builder.RequireRole(AuthorizationRoles.Admin.ToString());
    }
}
