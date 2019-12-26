using System.Threading.Tasks;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    /// <summary>
    /// The authentication repository interface.
    /// </summary>
    public interface IAuthRepository
    {
        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="user">The user to register.</param>
        /// <param name="password">The password.</param>
        /// <returns>The registered user.</returns>
         Task<User> Register(User user, string password);

         /// <summary>
         /// Logs in the user.
         /// </summary>
         /// <param name="username">The username.</param>
         /// <param name="password">The password.</param>
         /// <returns>The logged in user.</returns>
         Task<User> Login(string username, string password);

         /// <summary>
         /// Checks to see if the user already exists.
         /// </summary>
         /// <param name="username">The username.</param>
         /// <returns>True if the user already exists, otherwise false.</returns>
         Task<bool> UserExists(string username);
    }
}