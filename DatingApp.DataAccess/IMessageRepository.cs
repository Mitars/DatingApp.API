using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DatingApp.Models;
using DatingApp.Shared;

namespace DatingApp.DataAccess
{
    /// <summary>
    /// The dating repository inferface.
    /// </summary>
    public interface IMessageRepository
    {   
        /// <summary>
        /// Gets the user with the coresponding ID.
        /// </summary>
        /// <param name="id">The ID of the user which to get.</param>
        /// <param name="isCurrentUser">A value which indicates whether or not this is the current user.</param>
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
        Task<Result<User, Error>> GetCurrentUser(int id);
        
        /// <summary>
        /// Gets all the users.
        /// </summary>
        /// <param name="userParams">The user params.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the paged list of users.
        /// </returns>
        Task<Result<PagedList<User>, Error>> GetUsers(UserParams userParams);

        /// <summary>
        /// Adds a new entity.
        /// </summary>
        /// <typeparam name="T">The entity which to create.</typeparam>
        Task<Result<User, Error>> Add(User entity);        

        /// <summary>
        /// Gets the like if it exists.
        /// </summary>
        /// <param name="userId">The user ID of the user that made the like.</param>
        /// <param name="recipientId">The recipient ID of the user who received the like.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the like.
        /// </returns>
        Task<Result<Like, Error>> Get(int userId, int recipientId);

        /// <summary>
        /// Adds a new entity.
        /// </summary>
        /// <typeparam name="T">The entity which to create.</typeparam>
        Task<Result<Like, Error>> Add(Like entity);        
    }
}