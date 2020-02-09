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

        /// <summary>
        /// Initializes a new instance of the <see cref="UserManager"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        public UserManager(IUserRepository userRepository) =>
            this.userRepository = userRepository;

        /// <inherits />
        public async Task<Result<User, Error>> Add(User entity) =>
            await this.userRepository.Add(entity);
        
        /// <inherits />
        public async Task<Result<User, Error>> Get(int id) =>
            await this.userRepository.Get(id);
                    
        /// <inherits />
        public async Task<Result<User, Error>> GetCurrent(int userId) =>
            await this.userRepository.GetCurrentUser(userId);

        /// <inherits />
        public async Task<Result<PagedList<User>, Error>> GetUsers(UserParams userParams) =>
            await this.userRepository.GetUsers(userParams);
        /// <inherits />
        public async Task<Result<User, Error>> GetByLike(Like like) =>
            await this.userRepository.Get(like.LikeeId);
        
        /// <inherits />
        public async Task<Result<Like, Error>> Get(Like like) => 
            await this.userRepository.Get(like.LikerId, like.LikeeId);
        
        /// <inherits />
        public virtual async Task<Result<Like, Error>> Add(Like entity) =>
            await this.userRepository.Add(entity);
    }
}