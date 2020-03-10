using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DatingApp.Models;
using DatingApp.Shared;
using DatingApp.Shared.ErrorTypes;
using DatingApp.Shared.FunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.DataAccess
{
    /// <summary>
    /// The like repository.
    /// </summary>
    public class LikeRepository: ILikeRepository
    {
        private readonly IBaseRepository baseRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="LikeRepository"/> class.
        /// </summary>
        /// <param name="context">The data context.</param>
        public LikeRepository(IBaseRepository baseRepository) =>
            this.baseRepository = baseRepository;

        /// <inheritdoc />
        public Task<Result<Like, Error>> Get(int userId, int recipientId) =>
            this.baseRepository.Context.Likes.FirstOrDefaultAsync(l => l.LikerId == userId && l.LikeeId == recipientId).Success();

        /// <inheritdoc />
        public Task<Result<Like, Error>> Add(Like entity) =>
            this.baseRepository.Add<Like>(entity);
        
        /// <inheritdoc />
        public Task<Result<None, Error>> Delete(Like entity) =>
            this.baseRepository.Delete<Like>(entity);
    }
}