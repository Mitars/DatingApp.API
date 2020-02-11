using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CSharpFunctionalExtensions;
using DatingApp.DataAccess.Dtos;
using DatingApp.Shared;
using Microsoft.Extensions.Options;
using Error = DatingApp.Shared.Error;

namespace DatingApp.DataAccess
{
    public class CloudinaryRepository : ICloudinaryRepository
    {
        private Cloudinary cloudinary;

        public CloudinaryRepository(IOptions<CloudinarySettings> cloudinaryConfig)
        {
            var cloudinaryAccount = new Account(
                cloudinaryConfig.Value.CloudName,
                cloudinaryConfig.Value.ApiKey,
                cloudinaryConfig.Value.ApiSecret);

            this.cloudinary = new Cloudinary(cloudinaryAccount);
        }

        /// <inheritdoc />
        public async Task<Result<CreatedCloudPhoto, Error>> upload(PhotoToUpload photoToUpload)
        {
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(photoToUpload.FileName, photoToUpload.Stream),
                Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
            };

            var uploadResults = await this.cloudinary.UploadAsync(uploadParams);

            var createdCloudPhoto = new CreatedCloudPhoto
            {
                Url = uploadResults.Uri.ToString(),
                PublicId = uploadResults.PublicId
            };

            return Result.Success<CreatedCloudPhoto, Error>(createdCloudPhoto);
        }

        /// <inheritdoc />
        public async Task<Result<None, Error>> Delete(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await this.cloudinary.DestroyAsync(deleteParams);

            if (result.Result == "ok")
            {
                return Result.Success<None, Error>(new None());
            }
            
            return Result.Failure<None, Error>(new Error("Failed to delete photo from Cloudinary"));
        }
    }
}