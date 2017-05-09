using WebApiCoreSeed.Infrastructure.Result;

namespace WebApiCoreSeed.Infrastructure
{
    internal static class InfrastructureError
    {
        internal static readonly ErrorResult Auth0ManagementApiAuthenticationFailed = new ErrorResult("Auth0-ManagementApi-FailedAuth", "Failed to obtain an authenticated Management Api Client");
    }
}
