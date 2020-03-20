using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DatingApp.Models;
using DatingApp.Shared;
using DatingApp.Shared.ErrorTypes;

namespace DatingApp.DataAccess
{
    /// <summary>
    /// The message repository inferface.
    /// </summary>
    public interface IMessageRepository
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
        /// <param name="message">The message to create.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the created message.
        /// </returns>
        Task<Result<Message, Error>> Add(Message message);

        /// <summary>
        /// Updates an existing message.
        /// </summary>
        /// <param name="message">The message to update.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the updated message.
        /// </returns>
        Task<Result<Message, Error>> Update(Message message);

        /// <summary>
        /// Deletes a message.
        /// </summary>
        /// <param name="message">The message to delete.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// </returns>
        Task<Result<None, Error>> Delete(Message message);
    }
}