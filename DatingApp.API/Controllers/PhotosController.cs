using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CSharpFunctionalExtensions;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.Business;
using DatingApp.Business.Dtos;
using DatingApp.Shared.FunctionalExtensions;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    /// <summary>
    /// The photos controller class.
    /// </summary>
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IPhotoManager photoManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="PhotosController"/> class.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        /// <param name="photoManager">The photo manager.</param>
        public PhotosController(IMapper mapper, IPhotoManager photoManager)
        {
            this.mapper = mapper;
            this.photoManager = photoManager;
        }

        /// <summary>
        /// Gets the photo.
        /// </summary>
        /// <param name="id">The ID of the photo to retrieve.</param>
        /// <returns>The photo.</returns>
        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<ActionResult<PhotoForReturnDto>> GetPhoto(int id)
        {
            return await id.Success()
                .Bind(this.photoManager.Get)
                .Bind(this.mapper.Map<PhotoForReturnDto>)
                .Finally(result => Ok(result), error => ActionResultError.Get(error, BadRequest));
        }

        /// <summary>
        /// Adds the photo for the specified user.
        /// </summary>
        /// <param name="userId">The user ID, for which user to add the photo.</param>
        /// <param name="photoForCreationDto">The photo parameters which to add to the user.</param>
        /// <returns>The created at root response with the details of the created photo.</returns>
        [HttpPost]
        public async Task<ActionResult> AddPhotoForUser(int userId, [FromForm]PhotoForCreationDto photoForCreationDto)
        {
            return await this.photoManager.Add(photoForCreationDto)
                .Finally(result => CreatedAtRoute("GetPhoto", new { userId = result.UserId, id = result.Id }, result), error => ActionResultError.Get(error, BadRequest));
        }

        /// <summary>
        /// Sets the specified photo as the main photo of the user.
        /// </summary>
        /// <param name="userId">The user ID for which the photo should be set as the main photo.</param>
        /// <param name="id">The ID of the photo to set as the main photo for the user.</param>
        /// <returns>A 204 No Content response if the photo has successfully been assigned as the main photo for the user.</returns>
        [HttpPost("{id}/setMain")]
        public async Task<ActionResult> SetMainPhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            return await this.photoManager.SetAsMain(userId, id)
                .Finally(_ => NoContent(), error => ActionResultError.Get(error, BadRequest));
        }

        /// <summary>
        /// Deletes the specified photo.
        /// The photo ID must correspond to a photo from the specified user.
        /// </summary>
        /// <param name="userId">The user ID of the user from which to delete the photo.</param>
        /// <param name="id">The photo ID of the photo to be deleted.</param>
        /// <returns>Returns an OK 200 response if the photo has successfully been deleted.</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            return await this.photoManager.Delete(userId, id)
                .Finally(_ => Ok(), error => ActionResultError.Get(error, BadRequest));
        }
    }
}