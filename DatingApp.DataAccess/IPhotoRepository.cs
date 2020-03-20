using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DatingApp.DataAccess.Dtos;
using DatingApp.Shared;
using DatingApp.Shared.ErrorTypes;

namespace DatingApp.DataAccess
{
    /// <summary>
    /// The photo repository interface.
    /// Allows persisting of the photo.
    /// Fetching actual photo metadata is done through the <see cref="IPhotoMetadataRepository"/>.
    /// </summary>
    public interface IPhotoRepository
    {
        /// <summary>
        /// Adds a new photo.
        /// </summary>
        /// <param name="photoToUpload">The photo to create.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the created photo.
        /// </returns>
        Task<Result<CreatedPhoto, Error>> Add(PhotoToCreate photoToUpload);

        /// <summary>
        /// Deletes the photo.
        /// </summary>
        /// <param name="publicId">The photo public ID.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// </returns>
        Task<Result<None, Error>> Delete(string publicId);
    }
}