using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DatingApp.Models;
using DatingApp.Shared;
using DatingApp.Shared.ErrorTypes;
using DatingApp.Shared.FunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.DataAccess
{
    /// <summary>
    /// The user repository.
    /// </summary>
    public class MessageRepository : IMessageRepository
    {
        private readonly IBaseRepository baseRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRepository"/> class.
        /// </summary>
        /// <param name="context">The data context.</param>
        public MessageRepository(IBaseRepository baseRepository) =>
            this.baseRepository = baseRepository;

        /// <inheritdoc />
        public Task<Result<Message, Error>> Get(int id) =>
            this.baseRepository.Get<Message>(id);

        /// <inheritdoc />
        public Task<Result<PagedList<Message>, Error>> Get(MessageParams messageParams)
        {
            var messages = this.baseRepository.Context.Messages.AsQueryable();

            switch (messageParams.MessageContainer)
            {
                case "Inbox":
                    messages = messages.Where(m => m.RecipientId == messageParams.UserId && !m.RecipientDeleted);
                    break;
                case "Outbox":
                    messages = messages.Where(m => m.SenderId == messageParams.UserId && !m.SenderDeleted);
                    break;
                default:
                    messages = messages.Where(m => m.RecipientId == messageParams.UserId && !m.RecipientDeleted && !m.IsRead);
                    break;
            }

            return PagedList<Message>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize).Success();
        }
        
        /// <inheritdoc />
        public Task<Result<IEnumerable<Message>, Error>> GetThread(int senderId, int recipientId) =>
            this.baseRepository.Context.Messages
                .Where(m => (m.Sender.Id == senderId && m.Recipient.Id == recipientId && !m.SenderDeleted)
                    || (m.Recipient.Id == senderId && m.Sender.Id == recipientId && !m.RecipientDeleted))
                .OrderByDescending(m => m.MessageSent)
                .ToListAsync()
                .Success();

        /// <inheritdoc />
        public Task<Result<Message, Error>> Add(Message message) =>
            this.baseRepository.Add(message);
            
        /// <inheritdoc />
        public Task<Result<Message, Error>> Update(Message message) =>
            this.baseRepository.Update(message);     
            
        /// <inheritdoc />
        public Task<Result<None, Error>> Delete(Message message) =>
            this.baseRepository.Delete(message);
    }
}