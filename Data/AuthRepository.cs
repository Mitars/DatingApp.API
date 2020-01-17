using System;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    /// <summary>
    /// The authentication repository class.
    /// </summary>
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext dataContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthRepository"/> class.
        /// </summary>
        /// <param name="dataContext">The datacontext.</param>
        public AuthRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        /// <inheritdoc/>
        public async Task<User> Login(string username, string password)
        {
            var user = await this.dataContext.Users.Include(u => u.Photos).FirstOrDefaultAsync(user => user.Username == username);

            if (user?.PasswordHash.SequenceEqual(this.GeneratePasswordHash(password, user.PasswordSalt)) ?? false)
            {
                return user;
            }

            return null;                
        }

        /// <inheritdoc/>
        public async Task<User> Register(User user, string password)
        {
            (user.PasswordHash, user.PasswordSalt) = password.GeneratePasswordHashSalt();

            await this.dataContext.Users.AddAsync(user);
            await this.dataContext.SaveChangesAsync();

            return user;
        }

        /// <inheritdoc/>
        public async Task<bool> UserExists(string username)
        {
            return await this.dataContext.Users.AnyAsync(user => user.Username == username);
        }

        /// <summary>
        /// Generates the password hash using the specified plain text password and salt.
        /// </summary>
        /// <param name="password">The password used to generate the hash.</param>
        /// <param name="passwordSalt">The salt used to generate the hash.</param>
        /// <returns>The generated password hash.</returns>
        private byte[] GeneratePasswordHash(string password, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)) {
                return hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}