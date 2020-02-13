using System;
using System.ComponentModel.DataAnnotations;

namespace DatingApp.Business
{
    /// <summary>
    /// The class user used for registration.
    /// </summary>
    public class UserForRegisterDto
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="UserForRegisterDto"/> class.
        /// </summary>
        public UserForRegisterDto()
        {
            this.Created = DateTime.Now;
            this.LastActive = this.Created;
        }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [Required]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "You must specify a password between 4 and 8 characters")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the user's gender.
        /// </summary>
        [Required]
        public string Gender { get; set; }

        /// <summary>
        /// Gets or sets the user's date of birth.
        /// </summary>
        [Required]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the name by which the user goes by.
        /// </summary>
        [Required]
        public string KnownAs { get; set; }

        /// <summary>
        /// Gets or sets the city in which the user lives.
        /// </summary>
        [Required]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the country in which the user lives.
        /// </summary>
        [Required]
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the date the user account was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date when the user was last active.
        /// </summary>
        public DateTime LastActive { get; set; }
    }
}