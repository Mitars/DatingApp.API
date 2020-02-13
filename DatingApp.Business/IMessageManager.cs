using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DatingApp.Models;
using DatingApp.Shared;

namespace DatingApp.Business
{
    /// <summary>
    /// The message manager interface.
    /// </summary>
    public interface IMessageManager
    {
        /// <summary>
        /// Gets the message with the coresponding ID.
        /// </summary>
        /// <param name="id">The ID of the message which to get.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the message.
        /// </returns>
        Task<Result<Message, Error>> Get(int id);

        /// <summary>
        /// Gets the list of message depending on the specified params.
        /// </summary>
        /// <param name="messageParams">The message params used for filtering the messages list.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the list of messages.
        /// </returns>
        Task<Result<PagedList<Message>, Error>> Get(MessageParams messageParams);

        /// <summary>
        /// Gets the list of message between two users.
        /// </summary>
        /// <param name="senderId">The sender ID.</param>
        /// <param name="recipientId">The recipient ID.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the list of messages.
        /// </returns>
        Task<Result<IEnumerable<Message>, Error>> GetThread(int senderId, int recipientId);

        /// <summary>
        /// Adds a new message.
        /// </summary>
        /// <typeparam name="message">The message to create.</typeparam>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the created message.
        /// </returns>
        Task<Result<Message, Error>> Add(int userId, Message message);

        /// <summary>
        /// Deletes a message.
        /// </summary>
        /// <param name="message">The message to delete.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// </returns>
        Task<Result<None, Error>> Delete(int userId, int id);

        /// <summary>
        /// Marks the message with the specified ID as read.
        /// </summary>
        /// <param name="userId">The user who read the message.</param>
        /// <param name="id">The message ID.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the read message.
        /// </returns>
        Task<Result<Message, Error>> MarkAsRead(int userId, int id);
    }
}