using System;
using System.IO;

namespace DatingApp.Business.Dtos
{
    /// <summary>
    /// The photo for creation data transfer object class.
    /// </summary>
    public class PhotoForCreationDto
    {
        /// <summary>
        /// Gets or sets the user ID for who the photo will be added.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        public Stream Stream { get; set; }
        
        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the photo description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the date the photo was added.
        /// </summary>
        public DateTime DateAdded { get; set; } = DateTime.Now;           
    }
}