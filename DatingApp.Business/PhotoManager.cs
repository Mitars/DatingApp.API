using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CSharpFunctionalExtensions;
using DatingApp.Business.Dtos;
using DatingApp.DataAccess;
using DatingApp.DataAccess.Dtos;
using DatingApp.Models;
using DatingApp.Shared;
using DatingApp.Shared.ErrorTypes;
using DatingApp.Shared.FunctionalExtensions;

namespace DatingApp.Business
{
    /// <summary>
    /// The photo manager class.
    /// </summary>
    public class PhotoManager : IPhotoManager
    {
        private readonly IMapper mapper;
        private readonly IPhotoMetadataRepository photoMetadataRepository;
        private readonly IUserRepository userRepository;
        private readonly IPhotoRepository photoRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="PhotoManager"/> class.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        /// <param name="photoRepository">The photo repository.</param>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="cloudinaryRepository">The cloudinary cloud image provider.</param>
        public PhotoManager(
            IMapper mapper,
            IPhotoMetadataRepository photoRepository,
            IUserRepository userRepository,
            IPhotoRepository cloudinaryRepository)
        {
            this.mapper = mapper;
            this.photoMetadataRepository = photoRepository;
            this.userRepository = userRepository;
            this.photoRepository = cloudinaryRepository;
        }
        
        /// <inheritdoc />
        public virtual async Task<Result<Photo, Error>> Get(int id) =>
            await this.photoMetadataRepository.Get(id);

        /// <inheritdoc />
        public async Task<Result<Photo, Error>> Add(PhotoForCreationDto photoForCreationDto) =>
            await photoForCreationDto.Success().AutoMap(this.mapper.Map<Photo>)
                .Tap(async photo => {
                    await new PhotoToCreate
                    {
                        FileName = photoForCreationDto.FileName,
                        Stream = photoForCreationDto.Stream
                    }
                    .Success()
                    .Bind(this.photoRepository.Add)
                    .Tap(createdCloudPhoto =>
                    {
                        photo.PublicId = createdCloudPhoto.PublicId;
                        photo.Url = createdCloudPhoto.Url;
                    });
                })
                .Tap(async photo => {
                    photo.User = (await this.userRepository.Get(photoForCreationDto.UserId)).Value;
                    photo.UserId = photo.User.Id;
                })
                .Bind(this.photoMetadataRepository.Add);

        /// <inheritdoc />
        public async Task<Result<Photo, Error>> SetAsMain(int userId, int id) =>
            await this.photoMetadataRepository.Get(id)
                .Ensure(p => p.Value == null, new UnauthorizedError("Must specify an existing photo"))
                .Ensure(p => p.IsMain, new UnauthorizedError("This is already the main photo"))
                .Ensure(p => p.Value.Id == userId, new Error("Can not set other users' photo to main"))
                .Bind(p => this.photoMetadataRepository.UpdateMainForUser(userId, p.Id));
        
        /// <inheritdoc />
        public async Task<Result<None, Error>> Delete(int userId, int id) {
            return await this.userRepository.GetExcludingQueryFilters(userId)
                .Ensure((User u) => !u.Photos.Any(p => p.Id == id), new Error("Unauthorized"))
                .Bind(u => this.photoMetadataRepository.Get(id))
                .Ensure(p => p.IsMain, new Error("You cannot delete your main photo"))
                .TapIf(p => p.PublicId != null, async p => await this.photoRepository.Delete(p.PublicId))
                .TapIf(p => p.PublicId != null, this.photoMetadataRepository.Delete)
                .None();
        }
    }
}