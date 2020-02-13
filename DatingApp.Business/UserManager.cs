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
        public async Task<Result<User, Error>> Get(int id) =>
            await this.userRepository.Get(id);

        /// <inheritdoc />
        public async Task<Result<User, Error>> GetCurrent(int userId) =>
            await this.userRepository.GetExcludingQueryFilters(userId);

        /// <inheritdoc />
        public async Task<Result<PagedList<User>, Error>> Get(UserParams userParams) =>
            await Result.Success<UserParams, Error>(userParams)
                .Bind(async userParams => {
                    if(string.IsNullOrEmpty(userParams.Gender)) {
                        var user = await this.userRepository.Get(userParams.UserId);
                        if (user.IsFailure) {
                            return Result.Failure<UserParams, Error>(user.Error);
                        }

                        userParams.Gender = user.Value.Gender == "male" ? "female" : "male";
                    }
                    
                    return Result.Success<UserParams, Error>(userParams);
                })
                .Bind(this.userRepository.Get);

        /// <inheritdoc />
        public async Task<Result<User, Error>> Update(User entity) =>
            await this.userRepository.Update(entity);
        
        /// <inheritdoc />
        public async Task<Result<User, Error>> UpdateActivity(int userId) =>
            await this.userRepository.Get(userId)
                .Tap(u => u.LastActive = DateTime.Now)
                .Bind(this.userRepository.Update);
        
        /// <inheritdoc />
        public async Task<Result<Like, Error>> AddLike(int id, int recipientId) =>
            await new Like
                {
                    LikerId = id,
                    LikeeId = recipientId
                }
                .Success()
                .EnsureNull(async like => await this.likeRepository.Get(like.LikerId, like.LikeeId), new Error("You already liked this user"))
                .EnsureNotNull(async like => await this.userRepository.Get(like.LikeeId), new NotFoundError("Cannot find user to like"))
                .Bind(this.likeRepository.Add);
    }
}