using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    /// <summary>
    /// The administrator controller class.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly DataContext context;
        private readonly UserManager<User> userManager;
        private readonly IDatingRepository repo;
        private readonly IOptions<CloudinarySettings> cloudinaryConfig;
        private Cloudinary cloudinary;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminController"/> class.
        /// </summary>
        /// <param name="context">The data context.</param>
        /// <param name="userManager">the user manager.</param>
        /// <param name="repo">The dating repository.</param>
        /// <param name="cloudinaryConfig">The Cloudinary settings.</param>
        public AdminController(DataContext context, UserManager<User> userManager, IDatingRepository repo, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            this.context = context;
            this.userManager = userManager;
            this.repo = repo;

            this.cloudinaryConfig = cloudinaryConfig;

            var cloudinaryAccount = new Account(
                this.cloudinaryConfig.Value.CloudName,
                this.cloudinaryConfig.Value.ApiKey,
                this.cloudinaryConfig.Value.ApiSecret);

            this.cloudinary = new Cloudinary(cloudinaryAccount);
        }

        /// <summary>
        /// Gets the users and their roles.
        /// </summary>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The list of users and their roles.
        /// </returns>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("usersWithRoles")]
        public async Task<ActionResult> GetUsersWithRolesAsync()
        {
            var userList = await context.Users
                .OrderBy(u => u.UserName)
                .Select(u => new
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Roles = u.UserRoles.Join(this.context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                }).ToListAsync();
            return Ok(userList);
        }

        /// <summary>
        /// Edits the user roles.
        /// </summary>
        /// <param name="userName">The user for whom to change the roles.</param>
        /// <param name="roleEditDto">The roles which to assign to the user.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The list of roles assigned to the user.
        /// </returns>
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("editRoles/{userName}")]
        public async Task<ActionResult> EditRoles(string userName, RoleEditDto roleEditDto)
        {
            var user = await this.userManager.FindByNameAsync(userName);
            var userRoles = await this.userManager.GetRolesAsync(user);
            var selectedRoles = roleEditDto.RoleNames;
            selectedRoles = selectedRoles ?? new string[] { };
            var result = await this.userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded)
            {
                return BadRequest("Failed to add the roles");
            }

            result = await this.userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded)
            {
                return BadRequest("Failed to remove the roles");
            }

            return Ok(await this.userManager.GetRolesAsync(user));
        }

        /// <summary>
        /// Gets the photos for moderation.
        /// </summary>
        /// <returns>The photos for moderation.</returns>
        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photosForModeration")]
        public ActionResult GetPhotosForModeration()
        {
            return Ok(this.context.Photos.IgnoreQueryFilters().Where(p => !p.isApproved));
        }

        /// <summary>
        /// Approves the photo.
        /// </summary>
        /// <param name="photoId">The photo ID of the photo to approve.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// A status code 200 if the photo has been approved.
        /// </returns>
        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("approvePhoto/{photoId}")]
        public async Task<ActionResult> ApprovePhoto(int photoId)
        {
            var photo = await this.context.Photos.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == photoId);

            if (photo == null)
            {
                return BadRequest("Photo does not exist");
            }

            if (photo.isApproved)
            {
                return BadRequest("Photo is already approved");
            }

            photo.isApproved = true;

            if (!this.context.Users.Include(u => u.Photos).IgnoreQueryFilters()
                .First(u => u.Photos.Any(p => p.Id == photoId))
                .Photos.Any(p => p.IsMain))
            {
                photo.IsMain = true;
            }

            if (await this.context.SaveChangesAsync() > 0)
            {
                return Ok();
            }

            return BadRequest("Failed approving the photo");
        }

        /// <summary>
        /// Rejects the photo and deletes it.
        /// </summary>
        /// <param name="id">The photo ID of the photo to reject.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// A status code 200 if the photo has been deleted.
        /// </returns>
        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("rejectPhoto/{id}")]
        public async Task<ActionResult> RejectPhoto(int id)
        {
            var photo = await this.repo.GetPhoto(id);
            if (photo == null)
            {
                return Unauthorized();
            }

            var photoFromRepo = await this.repo.GetPhoto(id);

            if (photoFromRepo.IsMain)
            {
                return BadRequest("You cannot delete your main photo");
            }

            if (photoFromRepo.PublicId != null)
            {
                var deleteParams = new DeletionParams(photoFromRepo.PublicId);
                var result = cloudinary.Destroy(deleteParams);
                if (result.Result == "ok")
                {
                    repo.Delete(photoFromRepo);
                }
            }
            else
            {
                repo.Delete(photoFromRepo);
            }

            if (await repo.SaveAll())
            {
                return Ok();
            }

            return BadRequest("Failed to delete the photo");
        }
    }
}