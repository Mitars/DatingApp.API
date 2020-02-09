using System;
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
        private readonly IDatingRepository repository;

        public PhotoManager(IDatingRepository repository, CloudinaryProvider cloudinaryProvider)
        {
            this.cloudinaryProvider = cloudinaryProvider;
            this.repository = repository;
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
    }
}