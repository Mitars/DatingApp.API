using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Dtos
{
    /// <summary>
    /// The class user used for registration.
    /// </summary>
    public class UserForLoginDto
    {
        /// <summary>
        /// Gets and sets the username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets and sets the password.
        /// </summary>
        public string Password { get; set; }
    }
}