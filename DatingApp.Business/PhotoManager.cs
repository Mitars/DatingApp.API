using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
        private readonly IMapper mapper;
        private readonly IPhotoRepository photoRepository;
        private readonly IUserRepository userRepository;
        private readonly CloudinaryRepository cloudinaryRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="PhotoManager"/> class.
        /// </summary>
        /// <param name="photoRepository">The photo repository.</param>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="cloudinaryRepository">The cloudinary cloud image provider.</param>
        public PhotoManager(IMapper mapper, IPhotoRepository photoRepository, IUserRepository userRepository, CloudinaryRepository cloudinaryRepository)
        {
            this.mapper = mapper;
            this.photoRepository = photoRepository;
            this.userRepository = userRepository;
            this.cloudinaryRepository = cloudinaryRepository;
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
            };

            var createdCloudPhoto = this.cloudinaryRepository.upload(photoToUpload);

            var photo = this.mapper.Map<Photo>(photoForCreationDto);

            photo.User = (await this.userRepository.Get(photoForCreationDto.UserId)).Value;
            photo.UserId = photo.User.Id;
            return await this.photoRepository.Add(photo);
        }

        /// <inherits />
        public async Task<Result<Photo, Error>> SetAsMain(int userId, int id) =>
            await this.photoRepository.Get(id)
                .Ensure(p => p.Value == null, new UnauthorizedError("Must specify an existing photo"))
                .Ensure(p => p.IsMain, new UnauthorizedError("This is already the main photo"))
                .Ensure(p => p.Value.Id == userId, new Error("Can not set other users' photo to main"))
                .Bind(p => this.photoRepository.UpdateMainForUser(userId, p.Id));
        
        /// <inherits />
        public async Task<Result<None, Error>> Delete(int userId, int id) {
            return await this.userRepository.GetExcludingQueryFilters(userId)
                .Ensure((User u) => !u.Photos.Any(p => p.Id == id), new Error("Unauthorized"))
                .Bind(u => this.photoRepository.Get(id))
                .Ensure(p => p.IsMain, new Error("You cannot delete your main photo"))
                .TapIf(p => p.PublicId != null, async p => await this.cloudinaryRepository.Delete(p.PublicId))
                .TapIf(p => p.PublicId != null, p => this.photoRepository.Delete(p))
                .Finally(p => Result.Success<None, Error>(new None()), p => Result.Failure<None, Error>(p.Error));
        }
    }
}