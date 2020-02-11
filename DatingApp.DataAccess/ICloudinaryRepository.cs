using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DatingApp.DataAccess.Dtos;
using DatingApp.Shared;

namespace DatingApp.DataAccess
{
    /// <summary>
    /// The cloudinary repository inferface.
    /// </summary>
    public interface ICloudinaryRepository
    {
        Task<Result<CreatedCloudPhoto, Error>> Upload(PhotoToUpload photoToUpload);
        Task<Result<None, Error>> Delete(string publicId);
    }
}