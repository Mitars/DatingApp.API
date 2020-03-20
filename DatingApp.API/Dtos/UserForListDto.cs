using System;

namespace DatingApp.API.Dtos
{
    /// <summary>
    /// The user list data transfer object class.
    /// </summary>
    public class UserForListDto
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
        /// Gets or sets the user's gender.
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// Gets or sets the user's age.
        /// </summary>
        public int Age { get; set; }

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
        /// Gets or sets the city in which the user lives.
        /// </summary>
        /// <value></value>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the country in which the user lives.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the user's main profile photo url.
        /// </summary>
        public string PhotoUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this user is liked by the user that made the request.
        /// </summary>
        public bool IsLiked { get; set; }
    }
}