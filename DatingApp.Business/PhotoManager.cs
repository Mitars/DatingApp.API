using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DatingApp.Business.Dtos;
using DatingApp.DataAccess;
using DatingApp.DataAccess.Dtos;
using DatingApp.Models;
using DatingApp.Shared;

namespace DatingApp.Business
{
    /// <summary>
    /// The photo manager class.
    /// </summary>
    public class PhotoManager : IPhotoManager
    {
        private readonly CloudinaryRepository cloudinaryProvider;
        private readonly IPhotoRepository photoRepository;
        private readonly IUserRepository userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="PhotoManager"/> class.
        /// </summary>
        /// <param name="photoRepository">The photo repository.</param>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="cloudinaryRepository">The cloudinary cloud image provider.</param>
        public PhotoManager(IPhotoRepository photoRepository, IUserRepository userRepository, CloudinaryRepository cloudinaryRepository)
        {
            this.photoRepository = photoRepository;
            this.userRepository = userRepository;
            this.cloudinaryProvider = cloudinaryRepository;
        }
        
        /// <inherits />
        public virtual async Task<Result<Photo, Error>> Get(int id) =>
            await this.photoRepository.Get(id);

        /// <inherits />
        public async Task<Result<Photo, Error>> AddPhotoForUser(PhotoForCreationDto photoForCreationDto)
        {
            var photoToUpload = new PhotoToUpload {
                FileName = photoForCreationDto.FileName,
                Stream = photoForCreationDto.Stream
            }

            var createdCloudPhoto = cloudinaryProvider.uploadPhoto(photoToUpload);

            var photoToCreate = new PhotoForCreationDto();

            var photo = this.mapper.Map<Photo>(photoForCreationDto);

            var userFromRepo = await this.photoRepository.GetCurrentUser(photoForCreationDto.UserId);
            userFromRepo.Photos.Add(photo); // Save to cloudinary!

            if (await this.photoRepository.SaveAll())
            {
                var photoToReturn = this.mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new { userId = photoForCreationDto.UserId, id = photo.Id }, photoToReturn);
            }

            return BadRequest("Could not add the photo");
        }

        /// <inherits />
        public async Task<Result<Photo, Error>> SetAsMain(int userId, int id) =>
            await this.photoRepository.Get(id)
                .Ensure(p => p.Value == null, new UnauthorizedError("Must specify an existing photo"))
                .Ensure(p => p.Value.Id == userId, new UnauthorizedError("Can not set other users' photo to main"))
                .Bind(p => this.photoRepository.UpdateMainForUser(userId, p.Id));
        
        /// <inherits />
        public async Task<Result<None, Error>> Delete(int userId, int id) {
            var user = await this.userRepository.GetExcludingQueryFilters(userId)
                .Ensure(u => !u.Value.Photos.Any(p => p.Id == id), new Error("Unauthorized"))
                .Bind(u => this.photoRepository.Get(id))
                .Ensure(u => photoFromRepo.IsMain, new Error("You cannot delete your main photo"))
                .TapIf(p => p.PublicId != null, m => 
                {
                    var deleteParams = new DeletionParams(photoFromRepo.PublicId);
                    var result = cloudinary.Destroy(deleteParams);
                    if (result.Result == "ok")
                    {
                        this.photoRepository.Delete(photoFromRepo);
                    }
                })
                .TapIf(p => p.PublicId != null, m => this.photoRepository.Delete(photoFromRepo));
        }
    }
}