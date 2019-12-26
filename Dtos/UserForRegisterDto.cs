using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Dtos
{
    /// <summary>
    /// The class user used for registration.
    /// </summary>
    public class UserForRegisterDto
    {
        /// <summary>
        /// Gets and sets the username.
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// Gets and sets the password.
        /// </summary>
        [Required]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "You must specify a password between 4 and 8 characters")]
        public string Password { get; set; }
    }
}