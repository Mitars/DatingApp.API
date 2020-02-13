using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DatingApp.Models;
using DatingApp.Shared;

namespace DatingApp.DataAccess
{
    /// <summary>
    /// The like repository inferface.
    /// </summary>
    public interface ILikeRepository
    {                
        /// <summary>
        /// Gets the like between two users.
        /// </summary>
        /// <param name="userId">The user ID of the user that made the like.</param>
        /// <param name="recipientId">The recipient ID of the user who received the like.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the like.
        /// </returns>
        Task<Result<Like, Error>> Get(int userId, int recipientId);

        /// <summary>
        /// Adds a new like.
        /// </summary>
        /// <typeparam name="like">The like to create.</typeparam>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the created like.
        /// </returns>
        Task<Result<Like, Error>> Add(Like like);        
    }
}