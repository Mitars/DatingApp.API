using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DatingApp.Business.Dtos;
using DatingApp.DataAccess;
using DatingApp.Models;
using DatingApp.Shared;
using DatingApp.Shared.ErrorTypes;
using DatingApp.Shared.FunctionalExtensions;
using Microsoft.AspNetCore.Identity;

namespace DatingApp.Business
{
    /// <summary>
    /// The administrator manager class.
    /// </summary>
    public class AdminManager : IAdminManager
    {
        private readonly IPhotoMetadataRepository photoMetadataRepository;
        private readonly IUserRepository userRepository;
        private readonly IPhotoRepository photoRepository;
        private readonly UserManager<User> identityUserManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminManager"/> class.
        /// </summary>
        /// <param name="photoMetadataRepository">The photo metadata repository.</param>
        /// <param name="identityUserManager">The user manager.</param>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="photoRepository">The photo repository.</param>
        public AdminManager(
            IPhotoMetadataRepository photoMetadataRepository,
            UserManager<User> identityUserManager,
            IUserRepository userRepository,
            IPhotoRepository photoRepository)
        {
            this.photoMetadataRepository = photoMetadataRepository;
            this.identityUserManager = identityUserManager;
            this.userRepository = userRepository;
            this.photoRepository = photoRepository;
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
            await this.photoMetadataRepository.GetPhotosForModeration();

        /// <inheritdoc />
        public async Task<Result<None, Error>> ApprovePhoto(int id) =>
            await this.photoMetadataRepository.GetExcludingQueryFilters(id)
                .Ensure(p => p != null, new UnauthorizedError("You cannot approve an non existing photo"))
                .Ensure(p => !p.IsApproved, new Error("Photo is already approved"))
                .Tap(p => p.IsApproved = true)
                .Tap(async p =>
                {
                    var user = await this.userRepository.GetExcludingQueryFilters(p.UserId);
                    if (!user.Value.Photos.Any(p => p.IsMain))
                    {
                        await this.photoMetadataRepository.UpdateMainForUser(p.UserId, p.Id);
                    }
                })
                .Bind(this.photoMetadataRepository.Update)
                .None();

        /// <inheritdoc />
        public async Task<Result<None, Error>> RejectPhoto(int id) =>
            await this.photoMetadataRepository.GetExcludingQueryFilters(id)
                .Ensure(p => p != null, new UnauthorizedError("You cannot delete an non existing photo"))
                .Ensure(p => !p.IsApproved, new UnauthorizedError("This photo has already been approved"))
                .TapIf(p => p.PublicId != null, async p => p.PublicId = (await this.photoRepository.Delete(p.PublicId)).IsSuccess ? null : p.PublicId)
                .TapIf(p => p.PublicId == null, this.photoMetadataRepository.Delete)
                .EnsureNull(async p => await this.photoMetadataRepository.GetExcludingQueryFilters(p.Id), new Error("Photo could not be deleted"))
                .None();
    }
}