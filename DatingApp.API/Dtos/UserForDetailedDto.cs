using System;
using System.Collections.Generic;

namespace DatingApp.API.Dtos
{
    /// <summary>
    /// The detailed user data transfer object class.
    /// </summary>
    public class UserForDetailedDto
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
        /// Gets or sets the user's main profile photo url.
        /// </summary>
        public string PhotoUrl { get; set; }
        
        /// <summary>
        /// Gets or sets the list of photos.
        /// </summary>
        public ICollection<PhotosForDetailedDto> Photos { get; set; }        
    }
}