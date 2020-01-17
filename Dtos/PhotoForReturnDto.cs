using System;

namespace DatingApp.API.Dtos
{
    /// <summary>
    /// The photo to return data transfer object.
    /// </summary>
    public class PhotoForReturnDto
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the photo URL.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the photo description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the date the photo was added.
        /// </summary>
        public DateTime DateAdded { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Photo"> is the main photo.
        /// </summary>
        public bool IsMain { get; set; }
        
        /// <summary>
        /// Gets or sets the public ID.
        /// </summary>
        public string PublicId { get; set; }   
    }
}