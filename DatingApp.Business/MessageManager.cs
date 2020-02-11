using System;
using System.Collections.Generic;
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
    public class MessageManager : IMessageManager
    {
        private readonly IMessageRepository messagesRepository;
        private readonly IUserRepository userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserManager"/> class.
        /// </summary>
        /// <param name="messagesRepository">The user repository.</param>
        public MessageManager(IMessageRepository messagesRepository, IUserRepository userRepository)
        {
            this.messagesRepository = messagesRepository;
            this.userRepository = userRepository;
        }

        /// <inheritdoc />
        public async Task<Result<Message, Error>> Get(int id) =>
            await this.messagesRepository.Get(id);

        /// <inheritdoc />
        public async Task<Result<PagedList<Message>, Error>> Get(MessageParams messageParams) =>
            await this.messagesRepository.Get(messageParams);

        /// <inheritdoc />
        public async Task<Result<IEnumerable<Message>, Error>> GetThread(int senderId, int recipientId) =>
            await this.messagesRepository.GetThread(senderId, recipientId);

        /// <inheritdoc />
        public async Task<Result<Message, Error>> Add(int userId, Message message)
        {
            var sender = (await this.userRepository.Get(message.SenderId)).Value;

            if (message.SenderId == message.RecipientId)
            {
                return message.Failure<Message, Error>("Cannot send message to self");
            }

            message.SenderId = sender.Id;
            var recipient = (await this.userRepository.Get(message.RecipientId)).Value;
            if (recipient == null)
            {
                return message.Failure<Message, Error>("Could not find user");
            }

            return await this.messagesRepository.Add(message);
        }
        
        /// <inheritdoc />
        public async Task<Result<None, Error>> Delete(int userId, int id) =>
            await this.messagesRepository.Get(id)
                .TapIf(m => m.SenderId == userId, m => m.SenderDeleted = true)
                .TapIf(m => m.RecipientId == userId, m => m.RecipientDeleted = true)
                .Bind(async m => (m.SenderDeleted && m.RecipientDeleted) ?
                    await this.messagesRepository.Delete(m) :
                    await this.messagesRepository.Update(m).DropResult()
                );
        
        /// <inheritdoc />
        public virtual async Task<Result<Message, Error>> MarkAsRead(int userId, int id) =>
            await this.messagesRepository.Get(id)
                .Ensure(m => m.Value.Id == userId, new UnauthorizedError("Can not mark other users' messages as read"))
                .Tap(m => { m.IsRead = true; m.DateRead = DateTime.Now; })
                .Bind(this.messagesRepository.Update);
    }
}