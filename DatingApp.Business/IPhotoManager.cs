using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DatingApp.Business.Dtos;
using DatingApp.Models;
using DatingApp.Shared;
using DatingApp.Shared.ErrorTypes;

namespace DatingApp.Business
{
    /// <summary>
    /// The photo manager interface.
    /// </summary>
    public interface IPhotoManager
    {
        /// <summary>
        /// Gets the photo with the coresponding ID.
        /// </summary>
        /// <param name="id">The ID of the user which to get.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the photo.
        /// </returns>
        Task<Result<Photo, Error>> Get(int id);

        /// <summary>
        /// Adds a new photo.
        /// </summary>
        /// <param name="photoForCreationDto">The photo to create.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the created photo.
        /// </returns>
        Task<Result<Photo, Error>> Add(PhotoForCreationDto photoForCreationDto);

        /// <summary>
        /// Sets the photo as the main photo for the specified user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="id">The photo to set as main.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the photo that is set as main.
        /// </returns>
        Task<Result<Photo, Error>> SetAsMain(int userId, int id);

        /// <summary>
        /// Deletes the photo.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="id">The photo ID.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// </returns>
        Task<Result<None, Error>> Delete(int userId, int id);
    }
}