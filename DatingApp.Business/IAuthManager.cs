using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DatingApp.Business.Dtos;
using DatingApp.Models;
using DatingApp.Shared;

namespace DatingApp.Business
{
    /// <summary>
    /// The auth manager interface.
    /// </summary>
    public interface IAuthManager
    {
        Task<Result<User, Error>> Login(UserForLoginDto userForLoginDto);

        Task<Result<User, Error>> Register(UserForRegisterDto userForRegisterDto);
        
        /// <summary>
        /// Generates the JSON Web Token using the <see cref="User"/>.
        /// </summary>
        /// <param name="user">The user used for generating the JWT.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The generated JWT.
        /// </returns>
        Task<Result<string, Error>> GenerateJwt(User user, string key);
    }
}