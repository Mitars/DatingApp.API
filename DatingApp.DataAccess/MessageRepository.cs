using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DatingApp.Models;
using DatingApp.Shared;
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
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="context">The data context.</param>
        public MessageRepository(IBaseRepository baseRepository) =>
            this.baseRepository = baseRepository;

        /// <inheritdoc/>
        public Task<Result<Message, Error>> Get(int id) =>
            this.baseRepository.Get<Message>(id);
            

        /// <inheritdoc/>
        public async Task<Result<PagedList<Message>, Error>> GetMessagesForUser(MessageParams messageParams)
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

            return Result.Success<PagedList<Message>, Error>(await PagedList<Message>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize));
        }
        
        /// <inheritdoc/>
        public async Task<Result<IEnumerable<Message>, Error>> GetThread(int senderId, int recipientId)
        {
            return Result.Success<IEnumerable<Message>, Error>(await this.baseRepository.Context.Messages
                .Where(m => (m.Sender.Id == senderId && m.Recipient.Id == recipientId && !m.SenderDeleted)
                    || (m.Recipient.Id == senderId && m.Sender.Id == recipientId && !m.RecipientDeleted))
                .OrderByDescending(m => m.MessageSent)
                .ToListAsync());
        }

        /// <inheritdoc/>
        public Task<Result<Message, Error>> Add(Message entity) =>
            this.baseRepository.Add<Message>(entity);
            
        /// <inheritdoc/>
        public async Task<Result<Message, Error>> Update(Message message) =>
            await this.baseRepository.Update<Message>(message);     
            
        /// <inheritdoc/>
        public async Task<Result<None, Error>> Delete(Message message) =>
            await this.baseRepository.Delete<Message>(message);
    }
}