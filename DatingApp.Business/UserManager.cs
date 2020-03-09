using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DatingApp.DataAccess;
using DatingApp.Models;
using DatingApp.Shared;
using DatingApp.Shared.ErrorTypes;
using DatingApp.Shared.FunctionalExtensions;

namespace DatingApp.Business
{
    /// <summary>
    /// The user manager class.
    /// </summary>
    public class UserManager : IUserManager
    {
        private readonly IUserRepository userRepository;
        public readonly ILikeRepository likeRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserManager"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="likeRepository">The like repository.</param>
        public UserManager(IUserRepository userRepository, ILikeRepository likeRepository)
        {
            this.userRepository = userRepository;
            this.likeRepository = likeRepository;
        }

        /// <inheritdoc />
        public Task<Result<User, Error>> Get(int id) =>
            this.userRepository.Get(id);

        /// <inheritdoc />
        public Task<Result<User, Error>> GetCurrent(int userId) =>
            this.userRepository.GetExcludingQueryFilters(userId);

        /// <inheritdoc />
        public async Task<Result<PagedList<User>, Error>> Get(UserParams userParams) =>
            await userParams.Success()
                .Bind(async userParams => {
                    if(string.IsNullOrEmpty(userParams.Gender)) {
                        var user = await this.userRepository.Get(userParams.UserId);
                        if (user.IsFailure) {
                            return Result.Failure<UserParams, Error>(user.Error);
                        }

                        userParams.Gender = user.Value.Gender == "male" ? "female" : "male";
                    }
                    
                    return userParams.Success();
                })
                .Bind(this.userRepository.Get);

        /// <inheritdoc />
        public Task<Result<User, Error>> Update(User entity) =>
            this.userRepository.Update(entity);
        
        /// <inheritdoc />
        public Task<Result<User, Error>> UpdateActivity(int userId) =>
            this.userRepository.Get(userId)
                .Tap(u => u.LastActive = DateTime.Now)
                .Bind(this.userRepository.Update);
        
        /// <inheritdoc />
        public Task<Result<Like, Error>> AddLike(int id, int recipientId) =>
            new Like
                {
                    LikerId = id,
                    LikeeId = recipientId
                }
                .Success()
                .EnsureNull(async like => await this.likeRepository.Get(like.LikerId, like.LikeeId), new Error("You already liked this user"))
                .EnsureNotNull(async like => await this.userRepository.Get(like.LikeeId), new NotFoundError("Cannot find user to like"))
                .Bind(like => this.likeRepository.Add(like));
        
        /// <inheritdoc />
        public Task<Result<None, Error>> DeleteLike(int id, int recipientId) =>
            this.likeRepository.Get(id, recipientId)
                .EnsureNotNull(new Error("You did not like this user"))
                .Bind(this.likeRepository.Delete);
    }
}