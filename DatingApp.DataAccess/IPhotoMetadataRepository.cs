using CSharpFunctionalExtensions;
using DatingApp.Models;
using DatingApp.Shared;
using DatingApp.Shared.ErrorTypes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatingApp.DataAccess
{
    /// <summary>
    /// The photo repository inferface.
    /// </summary>
    public interface IPhotoMetadataRepository
    {
        /// <summary>
        /// Gets the photo with the coresponding ID.
        /// </summary>
        /// <param name="id">The ID of the photo which to get.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the photo.
        /// </returns>
        Task<Result<Photo, Error>> Get(int id);

        /// <summary>
        /// Updates the main photo for the give user to the new specified photo.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="photoId">The ID of the photo which will the new main user photo.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the updated photo.
        /// </returns>
        Task<Result<Photo, Error>> UpdateMainForUser(int userId, int photoId);

        /// <summary>
        /// Adds a new photo.
        /// </summary>
        /// <typeparam name="photo">The photo to create.</typeparam>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the created photo.
        /// </returns>
        Task<Result<Photo, Error>> Add(Photo photo);

        /// <summary>
        /// Deletes a photo.
        /// </summary>
        /// <param name="photo">The photo to delete.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// </returns>
        Task<Result<None, Error>> Delete(Photo photo);

        Task<Result<Photo, Error>> Update(Photo photo);
        Task<Result<IEnumerable<Photo>, Error>> GetPhotosForModeration();
    }
}