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
    public class UserRepository : IUserRepository
    {
        private readonly IBaseRepository baseRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="context">The data context.</param>
        public UserRepository(IBaseRepository baseRepository) =>
            this.baseRepository = baseRepository;

        /// <inheritdoc/>
        public Task<Result<User, Error>> Get(int id) =>
            this.baseRepository.Get<User>(id);

        /// <inheritdoc/>
        public async Task<Result<User, Error>> GetExcludingQueryFilters(int Id) =>
            await this.baseRepository.Context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == Id).Success();

        /// <inheritdoc/>
        public async Task<Result<PagedList<User>, Error>> Get(UserParams userParams)
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
        public Task<Result<User, Error>> Add(User user) =>
            this.baseRepository.Add(user);

        /// <inheritdoc/>
        public Task<Result<User, Error>> Update(User user) =>
            this.baseRepository.Update(user);
    }
}