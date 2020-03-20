using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DatingApp.Business.Dtos;
using DatingApp.Models;
using DatingApp.Shared.ErrorTypes;

namespace DatingApp.Business
{
    /// <summary>
    /// The auth manager interface.
    /// </summary>
    public interface IAuthManager
    {
        /// <summary>
        /// Logs in the specified user.
        /// </summary>
        /// <param name="userForLoginDto">The user to login.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains logedin user.
        /// </returns>
        Task<Result<User, Error>> Login(UserForLoginDto userForLoginDto);

        /// <summary>
        /// Registers the specified user.
        /// </summary>
        /// <param name="userForRegisterDto">The user to register.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the registered user.
        /// </returns>
        Task<Result<User, Error>> Register(UserForRegisterDto userForRegisterDto);

        /// <summary>
        /// Generates the JSON Web Token using the <see cref="User"/>.
        /// </summary>
        /// <param name="user">The user used for generating the JWT.</param>
        /// <param name="key">The key used for encoding the JWT.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The generated JWT.
        /// </returns>
        Task<Result<string, Error>> GenerateJwt(User user, string key);
    }
}