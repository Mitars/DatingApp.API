using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DatingApp.DataAccess;
using DatingApp.Models;
using DatingApp.Shared;

namespace DatingApp.Business
{
    /// <summary>
    /// The user manager class.
    /// </summary>
    public class MessagesManager : MessageManager
    {
        private readonly MessageRepository messagesRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserManager"/> class.
        /// </summary>
        /// <param name="messagesRepository">The user repository.</param>
        public MessagesManager(MessageRepository messagesRepository) =>
            this.messagesRepository = messagesRepository;

        /// <inherits />
        public async Task<Result<User, Error>> Get(int id) =>
            await this.messagesRepository.Get(id);
                    
        /// <inherits />
        public async Task<Result<PagedList<Message>, Error>> Get(MessageParams messageParams) =>
            await this.messagesRepository.Get(messageParams);

        /// <inherits />
        public async Task<Result<PagedList<Message>, Error>> GetMessageThread(int userFromId, int userToId) =>
            await this.messagesRepository.Get(userParams);

        /// <inherits />
        public async Task<Result<Message, Error>> Create(Message like) =>
            await this.messagesRepository.Add(like.LikeeId);
        
        /// <inherits />
        public async Task<Result<None, Error>> DeleteMessage(int id, int userId) {
            var messageFromRepo = (await this.messagesRepository.Get(id)).Value;

            if (messageFromRepo.SenderId == userId)
            {
                messageFromRepo.SenderDeleted = true;
            }
            else if (messageFromRepo.RecipientId == userId)
            {
                messageFromRepo.RecipientDeleted = true;
            }

            Result<None, Error> result;
            if (messageFromRepo.SenderDeleted && messageFromRepo.RecipientDeleted)
            {
                result = await this.messagesRepository.Delete(messageFromRepo);
            } else {
                result = await this.messagesRepository.Update(messageFromRepo);
            }

            return result;
        }
        
        /// <inherits />
        public virtual async Task<Result<None, Error>> MarkMessageAsRead(int id, int userId)
        {
            var message = (await this.messagesRepository.Get<Message>(Id)).Value;

            if (message.RecipientId != userId)
            {
                return None.Failure<None, UnauthorizedError>("Can not mark other persons message as read");
            }

            message.IsRead = true;
            message.DateRead = DateTime.Now;
            return this.messagesRepository.Update(message);
        }
    }
}