using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DatingApp.Models;
using DatingApp.Shared;

namespace DatingApp.DataAccess
{
    /// <summary>
    /// The photo repository inferface.
    /// </summary>
    public interface IPhotoRepository
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
        /// Gets the main photo of the user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the photo.
        /// </returns>
        Task<Result<Photo, Error>> GetMainForUser(int userId);

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
    }
}