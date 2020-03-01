using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DatingApp.Models;
using DatingApp.Shared;
using DatingApp.Shared.ErrorTypes;

namespace DatingApp.Business
{
    /// <summary>
    /// The user manager interface.
    /// </summary>
    public interface IUserManager
    {
        /// <summary>
        /// Gets the user with the coresponding ID.
        /// </summary>
        /// <param name="id">The ID of the user which to get.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the user.
        /// </returns>
        Task<Result<User, Error>> Get(int id);

        /// <summary>
        /// Gets the user with the coresponding ID.
        /// Exclude any global filters that may apply.
        /// </summary>
        /// <param name="id">The ID of the user which to get.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the user.
        /// </returns>
        Task<Result<User, Error>> GetCurrent(int userId);

        /// <summary>
        /// Gets the list of users depending on the specified params.
        /// </summary>
        /// <param name="userParams">The user params.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the paged list of users.
        /// </returns>
        Task<Result<PagedList<User>, Error>> Get(UserParams userParams);

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <typeparam name="user">The user to update.</typeparam>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the updated user.
        /// </returns>
        Task<Result<User, Error>> Update(User user);
        
        /// <summary>
        /// Updates the user activity.
        /// </summary>
        /// <param name="userId">The ID of the user to update</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the created user.
        /// </returns>
        Task<Result<User, Error>> UpdateActivity(int userId);
        
        /// <summary>
        /// Adds a new like.
        /// </summary>
        /// <typeparam name="like">The like to create.</typeparam>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the created like.
        /// </returns>
        Task<Result<Like, Error>> AddLike(int id, int recipientId);

        /// <summary>
        /// Deletes a like.
        /// </summary>
        /// <param name="like">The like to delete.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// </returns>
        Task<Result<None, Error>> DeleteLike(int id, int recipientId);
    }
}