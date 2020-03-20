using System.IO;

namespace DatingApp.DataAccess.Dtos
{
    /// <summary>
    /// The photo to create data transfer object.
    /// </summary>
    public class PhotoToCreate
    {
        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the photo stream.
        /// </summary>
        public Stream Stream { get; set; }
    }
}