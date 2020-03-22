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
        /// <param name="photoMetadataRepository">The photo metadata repository.</param>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="photoRepository">The photo repository.</param>
        public PhotoManager(
            IMapper mapper,
            IPhotoMetadataRepository photoMetadataRepository,
            IUserRepository userRepository,
            IPhotoRepository photoRepository)
        {
            this.mapper = mapper;
            this.photoMetadataRepository = photoMetadataRepository;
            this.userRepository = userRepository;
            this.photoRepository = photoRepository;
        }

        /// <inheritdoc />
        public virtual async Task<Result<Photo, Error>> Get(int id) =>
            await this.photoMetadataRepository.Get(id);

        /// <inheritdoc />
        public async Task<Result<Photo, Error>> Add(PhotoForCreationDto photoForCreationDto) =>
            await photoForCreationDto.Success()
                .Bind(this.mapper.Map<Photo>)
                .Tap(async photo =>
                {
                    await new PhotoToUpload
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
                .Bind(this.photoMetadataRepository.Add);

        /// <inheritdoc />
        public Task<Result<Photo, Error>> SetAsMain(int userId, int id) =>
            this.photoMetadataRepository.GetExcludingQueryFilters(id)
                .Ensure(p => p.Value == null, new UnauthorizedError("Must specify an existing photo"))
                .Ensure(p => p.UserId == userId, new UnauthorizedError("Can not set other users' photo to main"))
                .Ensure(p => !p.IsMain, new Error("This is already the main photo"))
                .Ensure(p => p.IsApproved, new Error("Photo must be first approved"))
                .Bind(p => this.photoMetadataRepository.UpdateMainForUser(userId, p.Id));

        /// <inheritdoc />
        public async Task<Result<None, Error>> Delete(int userId, int id) =>
            await this.userRepository.GetExcludingQueryFilters(userId)
                .Ensure((User u) => u.Photos.Any(p => p.Id == id), new UnauthorizedError("Cannot delete other users photos"))
                .Bind(u => this.photoMetadataRepository.GetExcludingQueryFilters(id))
                .Ensure(p => !p.IsMain, new Error("You cannot delete your main photo"))
                .TapIf(p => p.PublicId != null, async p => p.PublicId = (await this.photoRepository.Delete(p.PublicId)).IsSuccess ? null : p.PublicId)
                .TapIf(p => p.PublicId == null, this.photoMetadataRepository.Delete)
                .EnsureNull(async p => await this.photoMetadataRepository.GetExcludingQueryFilters(p.Id), new Error("Photo could not be deleted"))
                .None();
    }
}