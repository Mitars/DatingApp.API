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
    public class PhotoManager : IPhotoManager
    {
        private readonly CloudinaryProvider cloudinaryProvider;
        private readonly IPhotoRepository repository;
        private readonly IUserRepository userRepository;

        public PhotoManager(IPhotoRepository repository, IUserRepository userRepository, CloudinaryProvider cloudinaryProvider)
        {
            this.cloudinaryProvider = cloudinaryProvider;
            this.repository = repository;
            this.userRepository = userRepository;
        }
        
        public virtual async Task<Result<Photo, Error>> Get(int id)
        {
            return await this.repository.Get<Photo>(id);
        }

        public async Task<Result<Photo, Error>> Add(PhotoForCreationDto photoForCreationDto)
        {
            var photoToUpload = new PhotoToUpload {
                FileName = photoForCreationDto.FileName,
                Stream = photoForCreationDto.Stream
            }

            var createdCloudPhoto = cloudinaryProvider.uploadPhoto(photoToUpload);

            var photoToCreate = new PhotoForCreationDto();

            var photo = this.mapper.Map<Photo>(photoForCreationDto);

            var userFromRepo = await this.repository.GetCurrentUser(photoForCreationDto.UserId);
            userFromRepo.Photos.Add(photo); // Save to cloudinary!

            if (await this.repository.SaveAll())
            {
                var photoToReturn = this.mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new { userId = photoForCreationDto.UserId, id = photo.Id }, photoToReturn);
            }

            return BadRequest("Could not add the photo");
        }
/// <inherits />
        public async Task<Result<Photo, Error>> Get(int id) {
            return await this.messagesRepository.Get(id);
        }

        /// <inherits />
        public async Task<Result<PagedList<Photo>, Error>> Get(MessageParams messageParams) {
            return await this.messagesRepository.GetMessagesForUser(messageParams);
        }

        /// <inherits />
        public async Task<Result<IEnumerable<Photo>, Error>> GetThread(int senderId, int recipientId) {
            return await this.messagesRepository.GetThread(senderId, recipientId);
        }

        /// <inherits />
        public async Task<Result<Photo, Error>> SetAsMain(int userId, int id) =>
        await this.repository.Get(id)
                .Ensure(m => m.Value.Id == userId, new UnauthorizedError("Can not mark other users' messages as read"))
                .Tap(m => { m.IsRead = true; m.IsMain = true; })
                .Bind(this.repository.Update);
        
        /// <inherits />
        public async Task<Result<None, Error>> Delete(int userId, int id) {
            var user = await this.userRepository.GetCurrentUser(userId)
                .Ensure(u => !u.Value.Photos.Any(p => p.Id == id), new Error("Unauthorized"))
                .Bind(u => this.repository.Get(id))
                .Ensure(u => photoFromRepo.IsMain, new Error("You cannot delete your main photo"))
                .TapIf(p => p.PublicId != null, m => 
                {
                    var deleteParams = new DeletionParams(photoFromRepo.PublicId);
                    var result = cloudinary.Destroy(deleteParams);
                    if (result.Result == "ok")
                    {
                        this.repository.Delete(photoFromRepo);
                    }
                })
                .TapIf(p => p.PublicId != null, m => this.repository.Delete(photoFromRepo));
        }
    }
}