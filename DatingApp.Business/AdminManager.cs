using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CSharpFunctionalExtensions;
using DatingApp.Business.Dtos;
using DatingApp.DataAccess;
using DatingApp.Models;
using DatingApp.Shared;
using DatingApp.Shared.ErrorTypes;
using Microsoft.AspNetCore.Identity;

namespace DatingApp.Business
{
    /// <summary>
    /// The administrator manager class.
    /// </summary>
    public class AdminManager : IAdminManager
    {
        private readonly IMapper mapper;
        private readonly IPhotoMetadataRepository photoRepository;
        private readonly IUserRepository userRepository;
        private readonly IPhotoRepository cloudinaryRepository;
        private readonly UserManager<User> identityUserManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminManager"/> class.
        /// </summary>
        /// <param name="photoMetadataRepository">The photo repository.</param>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="photoRepository">The cloudinary cloud image provider.</param>
        public AdminManager(
            IMapper mapper,
            IPhotoMetadataRepository photoMetadataRepository,
            UserManager<User> identityUserManager,
            IUserManager userManager,
            IUserRepository userRepository,
            IPhotoRepository photoRepository)
        {            
            this.mapper = mapper;
            this.photoRepository = photoMetadataRepository;
            this.identityUserManager = identityUserManager;
            this.userRepository = userRepository;
            this.cloudinaryRepository = photoRepository;
        }

        /// <inheritdoc />
        public async Task<Result<IEnumerable<UserWithRoles>, Error>> GetUsersWithRoles()
        {
            return (await this.userRepository.GetWithRoles()).Value.OrderBy(u => u.UserName)
                .Select(u => new UserWithRoles
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Roles = this.userRepository.GetRoles(u).Value
                })
                .Success();
        }

        /// <inheritdoc />
        public async Task<Result<IEnumerable<string>, Error>> EditRoles(string userName, RoleEditDto roleEditDto)
        {
            var user = await this.identityUserManager.FindByNameAsync(userName);
            var userRoles = await this.identityUserManager.GetRolesAsync(user);
            var selectedRoles = roleEditDto.RoleNames ?? new string[] { };
            
            return await user.Success()
                .Ensure(async user => (await this.identityUserManager.AddToRolesAsync(user, selectedRoles.Except(userRoles))).Succeeded, new Error("Failed to add the roles"))
                .Ensure(async user => (await this.identityUserManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles))).Succeeded, new Error("Failed to remove the roles"))
                .Bind(async result => await this.identityUserManager.GetRolesAsync(result).Success());
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
    }
}