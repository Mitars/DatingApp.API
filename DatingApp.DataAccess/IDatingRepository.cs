using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DatingApp.Models;
using DatingApp.Shared;

namespace DatingApp.DataAccess
{
    /// <summary>
    /// The dating repository inferface.
    /// </summary>
    public interface IDatingRepository
    {   
        /// <summary>
        /// Gets the user with the coresponding ID.
        /// </summary>
        /// <param name="Id">The ID of the user which to get.</param>
        /// <param name="isCurrentUser">A value which indicates whether or not this is the current user.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the user.
        /// </returns>
        Task<Result<T, Error>> Get<T>(int Id) where T : class, IBaseEntity;
        
        /// <summary>
        /// Adds a new entity.
        /// </summary>
        /// <typeparam name="T">The entity which to create.</typeparam>
        Task<Result<T, Error>> Add<T>(T entity) where T : class;

        /// <summary>
        /// Deletes the entity.
        /// </summary>
        /// <typeparam name="T">The entity to delete.</typeparam>
        void Delete<T>(T entity) where T: class;

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