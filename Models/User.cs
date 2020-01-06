using System;
using System.Collections.Generic;

namespace DatingApp.API.Models
{
    /// <summary>
    /// The user class.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password hash.
        /// </summary>
        public byte[] PasswordHash { get; set; }

        /// <summary>
        /// Gets or sets the password salt.
        /// </summary>
        public byte[] PasswordSalt { get; set; }

        /// <summary>
        /// Gets or sets the user's gender.
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// Gets or sets the user's date of birth.
        /// </summary>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the name by which the user goes by.
        /// </summary>
        public string KnownAs { get; set; }

        /// <summary>
        /// Gets or sets the date the user account was created.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date when the user was last active.
        /// </summary>
        public DateTime LastActive { get; set; }

        /// <summary>
        /// Gets or sets the introduction text.
        /// </summary>
        public string Introduction { get; set; }

        /// <summary>
        /// Gets or sets the information for the kind of person the user is looking for.
        /// </summary>
        public string LookingFor { get; set; }

        /// <summary>
        /// Gets or sets the interests.
        /// </summary>
        public string Interests { get; set; }

        /// <summary>
        /// Gets or sets the city in which the user lives.
        /// </summary>
        /// <value></value>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the country in which the user lives.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the list of photos.
        /// </summary>
        public ICollection<Photo> Photos { get; set; }
    }
}