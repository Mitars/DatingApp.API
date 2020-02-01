using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.DataAccess;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    /// <summary>
    /// The photos controller class.
    /// </summary>
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository repo;
        private readonly IMapper mapper;
        private readonly IOptions<CloudinarySettings> cloudinaryConfig;
        private readonly Cloudinary cloudinary;

        /// <summary>
        /// Initializes a new instance of the <see cref="PhotosController"/> class.
        /// </summary>
        /// <param name="repo">The dating repository.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="cloudinaryConfig">The Cloudinary settings.</param>
        public PhotosController(IDatingRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            this.cloudinaryConfig = cloudinaryConfig;
            this.mapper = mapper;
            this.repo = repo;

            var cloudinaryAccount = new Account(
                this.cloudinaryConfig.Value.CloudName,
                this.cloudinaryConfig.Value.ApiKey,
                this.cloudinaryConfig.Value.ApiSecret);

            this.cloudinary = new Cloudinary(cloudinaryAccount);
        }

        /// <summary>
        /// Gets the photo.
        /// </summary>
        /// <param name="id">The ID of the photo to retrieve.</param>
        /// <returns>The photo.</returns>
        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<ActionResult<PhotoForReturnDto>> GetPhoto(int id)
        {
            var photoFromRepo = await this.repo.GetPhoto(id);
            var photo = this.mapper.Map<PhotoForReturnDto>(photoFromRepo);
            return photo;
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
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var userFromRepo = await this.repo.GetCurrentUser(userId);
            var file = photoForCreationDto.File;
            var uploadResults = new ImageUploadResult();

            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    uploadResults = this.cloudinary.Upload(uploadParams);
                }
            }

            photoForCreationDto.Url = uploadResults.Uri.ToString();
            photoForCreationDto.PublicId = uploadResults.PublicId;

            var photo = this.mapper.Map<Photo>(photoForCreationDto);

            userFromRepo.Photos.Add(photo);

            if (await this.repo.SaveAll())
            {
                var photoToReturn = this.mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new { userId = userId, id = photo.Id }, photoToReturn);
            }

            return BadRequest("Could not add the photo");
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

            var user = await this.repo.GetCurrentUser(userId);
            if (!user.Photos.Any(p => p.Id == id))
            {
                return Unauthorized();
            }

            var photoFromRepo = await this.repo.GetPhoto(id);

            if (photoFromRepo.IsMain)
            {
                return BadRequest("This is already the main photo");
            }

            var currentMainPhoto = await this.repo.GetMainPhotoForUser(userId);
            currentMainPhoto.IsMain = false;
            photoFromRepo.IsMain = true;
            if (await this.repo.SaveAll())
            {
                return NoContent();
            }

            return BadRequest("Could not set photo to main");
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

            var user = await this.repo.GetCurrentUser(userId);
            if (!user.Photos.Any(p => p.Id == id))
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
            } else {
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