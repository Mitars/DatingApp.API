using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DatingApp.Business.Dtos;
using DatingApp.Models;
using DatingApp.Shared;

namespace DatingApp.Business
{
    public interface IPhotoManager
    {
        Task<Result<Photo, Error>> Add(PhotoForCreationDto entity);

        Task<Result<Photo, Error>> Get(int id);
    }
}