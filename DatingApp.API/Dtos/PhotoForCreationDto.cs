using System;
using Microsoft.AspNetCore.Http;

namespace DatingApp.API.Dtos
{
    /// <summary>
    /// The photo for creation data transfer object class.
    /// </summary>
    public class PhotoForCreationDto
    {
        /// <summary>
        /// Gets or sets the photo URL.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        public IFormFile File { get; set; }

        /// <summary>
        /// Gets or sets the photo description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the date the photo was added.
        /// </summary>
        public DateTime DateAdded { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the public ID.
        /// </summary>
        public string PublicId { get; set; }                
    }
}