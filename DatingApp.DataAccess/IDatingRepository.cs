using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.Models;

namespace DatingApp.DataAccess
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
        /// <param name="isCurrentUser">A value which indicates whether or not this is the current user.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the user.
        /// </returns>
        Task<User> GetUser(int Id);

        /// <summary>
        /// Gets the user with the coresponding ID.
        /// Exclude any global filters that may apply.
        /// </summary>
        /// <param name="Id">The ID of the user which to get.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the user.
        /// </returns>
        Task<User> GetCurrentUser(int Id);

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

        /// <summary>
        /// Gets the like if it exists.
        /// </summary>
        /// <param name="userId">The user ID of the user that made the like.</param>
        /// <param name="recipientId">The recipient ID of the user who received the like.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the like.
        /// </returns>
        Task<Like> GetLike(int userId, int recipientId);

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <param name="id">The message ID.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the message.
        /// </returns>
        Task<Message> GetMessage(int id);

        /// <summary>
        /// Gets the messages for the user.
        /// </summary>
        /// <param name="messageParams">The message parameters.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the messages.
        /// </returns>
        Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams);

        /// <summary>
        /// Gets the messages between two users.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="recipientId">The recipient ID.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the thread of messages between the two users.
        /// </returns>
        Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId);
    }
}