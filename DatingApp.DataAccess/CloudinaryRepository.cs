using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CSharpFunctionalExtensions;
using DatingApp.DataAccess.Dtos;
using DatingApp.Shared;
using DatingApp.Shared.FunctionalExtensions;
using Microsoft.Extensions.Options;
using Error = DatingApp.Shared.ErrorTypes.Error;

namespace DatingApp.DataAccess
{
    /// <summary>
    /// The cloudinary repository.
    /// Used to store photos.
    /// </summary>
    public class CloudinaryRepository : IPhotoRepository
    {
        private Cloudinary cloudinary;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudinaryRepository"/> class
        /// </summary>
        /// <param name="cloudinaryConfig">The configuration for cloudinary.</param>
        public CloudinaryRepository(IOptions<CloudinarySettings> cloudinaryConfig) =>
            this.cloudinary = new Cloudinary(
                new Account(
                    cloudinaryConfig.Value.CloudName,
                    cloudinaryConfig.Value.ApiKey,
                    cloudinaryConfig.Value.ApiSecret
                )
            );

        /// <inheritdoc />
        public async Task<Result<CreatedPhoto, Error>> Add(PhotoToCreate photoToUpload)
        {
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(photoToUpload.FileName, photoToUpload.Stream),
                Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
            };

            var uploadResults = await this.cloudinary.UploadAsync(uploadParams);

            return new CreatedPhoto
            {
                Url = uploadResults.Uri.ToString(),
                PublicId = uploadResults.PublicId
            }.Success();
        }

        /// <inheritdoc />
        public async Task<Result<None, Error>> Delete(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await this.cloudinary.DestroyAsync(deleteParams);
            return Result.SuccessIf<None, Error>(result.Result == "ok", new None(), new Error("Failed to delete photo from Cloudinary"));
        }
    }
}