using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DatingApp.Models;
using DatingApp.Shared.ErrorTypes;
using DatingApp.Shared.FunctionalExtensions;
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
        /// <param name="baseRepository">The base repository.</param>
        public UserRepository(IBaseRepository baseRepository) =>
            this.baseRepository = baseRepository;

        /// <inheritdoc />
        public Task<Result<IEnumerable<User>, Error>> GetWithRoles() =>
            this.baseRepository.Context.Set<User>().Include(u => u.UserRoles).ToListAsync().Success();

        /// <inheritdoc />
        public Task<Result<User, Error>> Get(int id) =>
            this.baseRepository.Get<User>(id);

        /// <inheritdoc />
        public Task<Result<User, Error>> GetExcludingQueryFilters(int id) =>
            this.baseRepository.Context.Users.IgnoreQueryFilters().Include(u => u.Photos).FirstOrDefaultAsync(u => u.Id == id).Success();

        /// <inheritdoc />
        public Task<Result<PagedList<User>, Error>> Get(UserParams userParams)
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

            users = userParams.OrderBy switch
            {
                "created" => users.OrderByDescending(u => u.Created),
                _ => users.OrderByDescending(u => u.LastActive),
            };

            return PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize).Success();
        }

        /// <inheritdoc />
        public Task<Result<User, Error>> Update(User user) =>
            this.baseRepository.Update(user);

        /// <inheritdoc />
        public Result<IEnumerable<string>, Error> GetRoles(User user) =>
            user.UserRoles.Join(this.baseRepository.Context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name).Success();
    }
}