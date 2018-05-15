using Auth0.Core;
using System.Threading.Tasks;
using WebApiCoreSeed.Infrastructure.Result;

namespace WebApiCoreSeed.Infrastructure.AuthZero
{
    /// <summary>
    /// Service responsible for any interaction with Auth0 API
    /// </summary>
    public interface IAuthZeroService
    {
        /// <summary>
        /// Creates a new user on Auth0 service
        /// </summary>
        /// <param name="email"> The user's email address </param>
        /// <param name="password"> The user's desired password </param>
        /// <param name="roles"> Roles for this user </param>
        /// <returns> <see cref="Result{TSucces,TError}"/> user object as <see cref="Result{TSuccess,TError}.SuccessValue"/> or error information as <see cref="Result{TSuccess,TError}.ErrorValue"/> </returns>
        Task<Result<User, ErrorResult>> CreateUserAsync(string email, string password, string[] roles);
    }
}
