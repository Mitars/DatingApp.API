using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    /// <summary>
    /// The dating repository inferface.
    /// </summary>
    public interface IDatingRepository
    {
        /// <summary>
        /// Adds a new entity.
        /// </summary>
        /// <typeparam name="T">The entity which to create.</typeparam>
        void Add<T>(T entity) where T: class;

        /// <summary>
        /// Deletes the entity.
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
        /// <param name="userParams">The user params.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the paged list of users.
        /// </returns>
        Task<PagedList<User>> GetUsers(UserParams userParams);

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
        /// <param name="userId">The user ID.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the photo.
        /// </returns>
        Task<Photo> GetMainPhotoForUser(int userId);
    }
}