using System;
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
        public async Task<Result<None, Error>> Delete(Message message) =>
            await this.baseRepository.Delete<Message>(message);

        /// <inheritdoc/>
        public async Task<Result<None, Error>> Update(Message message) =>
            await this.baseRepository.Update<Message>(message);

        /// <inheritdoc/>
        public async Task<Result<PagedList<User>, Error>> GetUsers(UserParams userParams)
        {
            var minDateOfBirth = DateTime.Today.AddYears(-userParams.MaxAge - 1);
            var maxDateOfBirth = DateTime.Today.AddYears(-userParams.MinAge);

            var users = this.baseRepository.Context.Users
                .Where(u => u.DateOfBirth >= minDateOfBirth)
                .Where(u => u.DateOfBirth <= maxDateOfBirth);

            if (userParams.Likees)
            {
                users = users
                    .Where(u => u.Id == userParams.UserId)
                    .SelectMany(u => u.Likees.Select(l => l.Likee))
                    .Distinct();
            }
            else if (userParams.Likers)
            {
                users = users
                    .Where(u => u.Id == userParams.UserId)
                    .SelectMany(u => u.Likers.Select(l => l.Liker))
                    .Distinct();
            }
            else
            {
                users = users
                    .Where(u => u.Id != userParams.UserId)
                    .Where(u => u.Gender == userParams.Gender);
            }

            switch (userParams.OrderBy)
            {
                case "created":
                    users = users.OrderByDescending(u => u.Created);
                    break;
                default:
                    users = users.OrderByDescending(u => u.LastActive);
                    break;
            }

            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize).Success();
        }

        /// <inheritdoc/>
        public Task<Result<User, Error>> Add(User entity) =>
            this.baseRepository.Add<User>(entity);
        
        /// <inheritdoc/>
        public async Task<Result<Like, Error>> Get(int userId, int recipientId) =>
            await this.baseRepository.Context.Likes.FirstOrDefaultAsync(l => l.LikerId == userId && l.LikeeId == recipientId).Success();

        /// <inheritdoc/>
        public Task<Result<Like, Error>> Add(Like entity) =>
            this.baseRepository.Add<Like>(entity);        
    }
}