using System.Collections.Generic;
using System.Linq;
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
    /// The photo metadata repository.
    /// </summary>
    public class PhotoMetadataRepository : IPhotoMetadataRepository
    {
        private readonly IBaseRepository baseRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="PhotoMetadataRepository"/> class.
        /// </summary>
        /// <param name="baseRepository">The base repository.</param>
        public PhotoMetadataRepository(IBaseRepository baseRepository) =>
            this.baseRepository = baseRepository;

        /// <inheritdoc />
        public Task<Result<Photo, Error>> Get(int id) =>
            this.baseRepository.Get<Photo>(id);

        /// <inheritdoc />
        public Task<Result<IEnumerable<Photo>, Error>> GetPhotosForModeration() =>
            this.baseRepository.Context.Photos.IgnoreQueryFilters().Where(p => !p.IsApproved).ToListAsync().Success();

        /// <inheritdoc />
        public Task<Result<Photo, Error>> Add(Photo photo) =>
            this.baseRepository.Add(photo);

        /// <inheritdoc />
        public async Task<Result<Photo, Error>> UpdateMainForUser(int userId, int photoId)
        {
            var currentMainPhoto = await this.baseRepository.Context.Photos
                .Where(p => p.UserId == userId)
                .FirstOrDefaultAsync(p => p.IsMain);

            var newMainPhoto = await this.baseRepository.Context.Photos
                .FirstOrDefaultAsync(p => p.Id == photoId);

            currentMainPhoto.IsMain = false;
            newMainPhoto.IsMain = true;
            var isSaveSuccessful = await this.baseRepository.SaveAll();

            return Result.SuccessIf(isSaveSuccessful, newMainPhoto, new Error("Failed updating the main user photo"));
        }

        /// <inheritdoc />
        public Task<Result<Photo, Error>> Update(Photo photo) =>
            this.baseRepository.Update(photo);

        /// <inheritdoc />
        public Task<Result<None, Error>> Delete(Photo photo) =>
            this.baseRepository.Delete(photo);
    }
}