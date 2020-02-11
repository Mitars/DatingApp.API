using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CSharpFunctionalExtensions;
using DatingApp.Business.Dtos;
using DatingApp.DataAccess;
using DatingApp.DataAccess.Dtos;
using DatingApp.Models;
using DatingApp.Shared;
using Microsoft.AspNetCore.Identity;

namespace DatingApp.Business
{
    /// <summary>
    /// The photo manager class.
    /// </summary>
    public class AdminManager : IAdminManager
    {
        private readonly IMapper mapper;
        private readonly IPhotoRepository photoRepository;
        private readonly IUserRepository userRepository;
        private readonly CloudinaryRepository cloudinaryRepository;
        private readonly UserManager<User> identityUserManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminManager"/> class.
        /// </summary>
        /// <param name="photoRepository">The photo repository.</param>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="cloudinaryRepository">The cloudinary cloud image provider.</param>
        public AdminManager(
            IMapper mapper,
            IPhotoRepository photoRepository,
            UserManager<User> identityUserManager,
            IUserManager userManager,
            IUserRepository userRepository,
            CloudinaryRepository cloudinaryRepository)
        {            
            this.mapper = mapper;
            this.photoRepository = photoRepository;
            this.identityUserManager = identityUserManager;
            this.userRepository = userRepository;
            this.cloudinaryRepository = cloudinaryRepository;
        }

        /// <inheritdoc />
        public async Task<Result<IEnumerable<UserWithRoles>, Error>> GetUsersWithRoles()
        {
            return await this.userRepository.GetWithRoles()
                .Bind(u =>
                    u.OrderBy(u => u.UserName)
                    .Select(u => new UserWithRoles
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        Roles = this.userRepository.GetRoles(u).Value
                    }))
                    .Success();
        }

        /// <inheritdoc />
        public async Task<Result<IEnumerable<string>, Error>> EditRoles(string userName, RoleEditDto roleEditDto)
        {
            var user = await this.identityUserManager.FindByNameAsync(userName);
            var userRoles = await this.identityUserManager.GetRolesAsync(user);
            var selectedRoles = roleEditDto.RoleNames;
            selectedRoles = selectedRoles ?? new string[] { };
            var result = await this.identityUserManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded)
            {
                return Result.Failure<IEnumerable<string>, Error>(new Error("Failed to add the roles"));
            }

            result = await this.identityUserManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded)
            {
                return Result.Failure<IEnumerable<string>, Error>(new Error("Failed to remove the roles"));
            }
            
            return (await this.identityUserManager.GetRolesAsync(user) as IEnumerable<string>).Success();
        }

        /// <inheritdoc />
        public async Task<Result<IEnumerable<Photo>, Error>> GetPhotosForModeration() =>
            await this.photoRepository.GetPhotosForModeration();

        /// <inheritdoc />
        public async Task<Result<None, Error>> ApprovePhoto(int id) =>
            await this.photoRepository.Get(id)
                .Ensure(p => p != null, new UnauthorizedError("You cannot delete an non existing photo"))
                .Ensure(p => !p.isApproved, new Error("Photo is already approved"))
                .Tap(p => p.isApproved = true)
                .Tap(async p => {
                    var user = await this.userRepository.Get(p.UserId);
                    if (!user.Value.Photos.Any(p => p.IsMain)) {
                        await this.photoRepository.UpdateMainForUser(p.UserId, p.Id);
                    }
                })
                .Bind(this.photoRepository.Update)
                .DropResult();

        /// <inheritdoc />
        public async Task<Result<None, Error>> RejectPhoto(int id) =>
            await this.photoRepository.Get(id)
                .Ensure(p => p != null, new UnauthorizedError("You cannot delete an non existing photo"))
                .Ensure(p => p.IsMain, new UnauthorizedError("You cannot delete your main photo"))
                .TapIf(p => p.PublicId != null, async p => await this.cloudinaryRepository.Delete(p.PublicId))
                .TapIf(p => p.PublicId != null, p => this.photoRepository.Delete(p))
                .DropResult();

        /// <inheritdoc />
        public virtual async Task<Result<Photo, Error>> Get(int id) =>
            await this.photoRepository.Get(id);

        /// <inheritdoc />
        public async Task<Result<Photo, Error>> AddPhotoForUser(PhotoForCreationDto photoForCreationDto)
        {
            var photoToUpload = new PhotoToUpload
            {
                FileName = photoForCreationDto.FileName,
                Stream = photoForCreationDto.Stream
            };

            var createdCloudPhoto = this.cloudinaryRepository.upload(photoToUpload);

            var photo = this.mapper.Map<Photo>(photoForCreationDto);

            photo.User = (await this.userRepository.Get(photoForCreationDto.UserId)).Value;
            photo.UserId = photo.User.Id;
            return await this.photoRepository.Add(photo);
        }

        /// <inheritdoc />
        public async Task<Result<Photo, Error>> SetAsMain(int userId, int id) =>
            await this.photoRepository.Get(id)
                .Ensure(p => p.Value == null, new UnauthorizedError("Must specify an existing photo"))
                .Ensure(p => p.Value.Id == userId, new UnauthorizedError("Can not set other users' photo to main"))
                .Ensure(p => !p.IsMain, new Error("This is already the main photo"))
                .Bind(p => this.photoRepository.UpdateMainForUser(userId, p.Id));

        /// <inheritdoc />
        public async Task<Result<None, Error>> Delete(int userId, int id) =>
            await this.userRepository.GetExcludingQueryFilters(userId)
                .Ensure((User u) => !u.Photos.Any(p => p.Id == id), new Error("The specified photo does not exist"))
                .Bind(u => this.photoRepository.Get(id))
                .Ensure(p => p.IsMain, new Error("You cannot delete your main photo"))
                .TapIf(p => p.PublicId != null, async p => await this.cloudinaryRepository.Delete(p.PublicId).Tap(this.photoRepository.Delete(p)))
                .TapIf(p => p.PublicId == null, p => this.photoRepository.Delete(p))
                .DropResult();
    }
}