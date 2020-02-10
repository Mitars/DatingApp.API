using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DatingApp.Business.Dtos;
using DatingApp.Models;
using DatingApp.Shared;

namespace DatingApp.Business
{
    /// <summary>
    /// The photo manager interface.
    /// </summary>
    public interface IPhotoManager
    {
        Task<Result<Photo, Error>> Get(int id);

        Task<Result<Photo, Error>> AddPhotoForUser(PhotoForCreationDto photoForCreationDto);

        Task<Result<Photo, Error>> SetAsMain(int userId, int id);

        Task<Result<None, Error>> Delete(int userId, int id);
    }
}