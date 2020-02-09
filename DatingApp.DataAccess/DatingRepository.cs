using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.Models;
using Microsoft.EntityFrameworkCore;
using DatingApp.Shared;

namespace DatingApp.DataAccess
{
    /// <summary>
    /// The dating repository class.
    /// Creates, deletes and fetches the user details. 
    /// </summary>
    public class DatingRepository : IDatingRepository
    {
        private readonly IDatingRepository datingRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatingRepository"/> class.
        /// </summary>
        /// <param name="context">The data context.</param>
        public DatingRepository(DataContext context, IDatingRepository datingRepository)
        {
            this.datingRepository = datingRepository;
        }
        
        Task<Result<T, Error>> IDatingRepository.Get<T>(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<Result<T, Error>> Add<T>(T entity) where T : class
        {
            throw new NotImplementedException();
        }

        public void Delete<T>(T entity) where T : class
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await this.context.Photos.Where(u => u.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
        }


        /// <inheritdoc/>
        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            var messages = this.context.Messages.AsQueryable();

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

            return await PagedList<Message>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
        {
            var messages = await this.context.Messages
                .Where(m => (m.Sender.Id == userId && m.Recipient.Id == recipientId && !m.SenderDeleted)
                    || (m.Recipient.Id == userId && m.Sender.Id == recipientId && !m.RecipientDeleted))
                .OrderByDescending(m => m.MessageSent)
                .ToListAsync();

            return messages;
        }
    }
}