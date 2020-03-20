using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.Business;
using DatingApp.Shared.FunctionalExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    /// <summary>
    /// The administrator controller class.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminManager adminManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminController"/> class.
        /// </summary>
        /// <param name="adminManager">The administrator manager.</param>
        public AdminController(IAdminManager adminManager)
        {
            this.adminManager = adminManager;
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
        public async Task<ActionResult> GetUsersWithRolesAsync() =>
            await this.adminManager.GetUsersWithRoles()
                .Finally(result => Ok(result), e => ActionResultError.Get(e, BadRequest));

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
        public async Task<ActionResult> EditRoles(string userName, RoleEditDto roleEditDto) =>
            await this.adminManager.EditRoles(userName, roleEditDto)
                .Finally(result => Ok(result), error => ActionResultError.Get(error, BadRequest));

        /// <summary>
        /// Gets the photos for moderation.
        /// </summary>
        /// <returns>The photos for moderation.</returns>
        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photosForModeration")]
        public async Task<ActionResult> GetPhotosForModeration() =>
            await this.adminManager.GetPhotosForModeration()
                .Finally(_ => Ok(), error => ActionResultError.Get(error, BadRequest));

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
        public async Task<ActionResult> ApprovePhoto(int photoId) =>
            await this.adminManager.ApprovePhoto(photoId)
                .Finally(_ => Ok(), error => ActionResultError.Get(error, BadRequest));

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
        public async Task<ActionResult> RejectPhoto(int id) =>
            await this.adminManager.RejectPhoto(id)
                .Finally(_ => Ok(), error => ActionResultError.Get(error, BadRequest));
    }
}