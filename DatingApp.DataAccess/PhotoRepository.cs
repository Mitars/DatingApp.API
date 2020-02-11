using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DatingApp.Models;
using DatingApp.Shared;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.DataAccess
{
    /// <summary>
    /// The photo repository.
    /// </summary>
    public class PhotoRepository: IPhotoRepository
    {
        private readonly IBaseRepository baseRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="PhotoRepository"/> class.
        /// </summary>
        /// <param name="context">The data context.</param>
        public PhotoRepository(IBaseRepository baseRepository) =>
            this.baseRepository = baseRepository;
        
        /// <inheritdoc/>
        public Task<Result<Photo, Error>> Get(int id) =>
            this.baseRepository.Get<Photo>(id);

        /// <inheritdoc/>
        public async Task<Result<Photo, Error>> GetMainForUser(int userId) =>
            Result.Success<Photo, Error>(await this.baseRepository.Context.Photos
                .Where(u => u.UserId == userId)
                .FirstOrDefaultAsync(p => p.IsMain));

        /// <inheritdoc/>
        public Task<Result<Photo, Error>> Add(Photo photo) =>
            this.baseRepository.Add(photo);

        /// <inheritdoc/>
        public async Task<Result<Photo, Error>> UpdateMainForUser(int userId, int photoId) {
            var currentMainPhoto = await this.baseRepository.Context.Photos
                .Where(p => p.UserId == userId)
                .FirstOrDefaultAsync(p => p.IsMain);
            
            var newMainPhoto = await this.baseRepository.Context.Photos
                .FirstOrDefaultAsync(p => p.Id == photoId);
            
            currentMainPhoto.IsMain = false;
            newMainPhoto.IsMain = true;
            var isSaveSuccessful = await this.baseRepository.SaveAll();

            return Result.SuccessIf<Photo, Error>(isSaveSuccessful, newMainPhoto, new Error("Failed updating the main user photo"));
        }
        
        /// <inheritdoc/>
        public async Task<Result<None, Error>> Delete(Photo photo) =>
            await this.baseRepository.Delete(photo);
    }
}