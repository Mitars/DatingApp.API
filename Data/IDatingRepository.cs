using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    /// <summary>
    /// The dating repository inferface.
    /// </summary>
    public interface IDatingRepository
    {
        /// <summary>
        /// Adds a new <see cref="{T}"/> entity.
        /// </summary>
        /// <typeparam name="T">The entity which to create.</typeparam>
        void Add<T>(T entity) where T: class;

        /// <summary>
        /// Deletes the {T} entity.
        /// </summary>
        /// <typeparam name="T">The entity to delete.</typeparam>
        void Delete<T>(T entity) where T: class;

        /// <summary>
        /// Saves all commits.
        /// </summary>
        /// <returns></returns>
        Task<bool> SaveAll();

        /// <summary>
        /// Gets all the users.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the list of users.
        /// </returns>
        Task<IEnumerable<User>> GetUsers();

        /// <summary>
        /// Gets the user with the coresponding ID.
        /// </summary>
        /// <param name="Id">The ID of the user which to get.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the user.
        /// </returns>
        Task<User> GetUser(int Id);

        /// <summary>
        /// Gets the photo.
        /// </summary>
        /// <param name="id">The photo ID.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the photo.
        /// </returns>
        Task<Photo> GetPhoto(int id);

        /// <summary>
        /// Gets the main photo of the user.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the photo.
        /// </returns>
        Task<Photo> GetMainPhotoForUser(int userId);
    }
}