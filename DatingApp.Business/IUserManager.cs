using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DatingApp.Models;
using DatingApp.Shared;

namespace DatingApp.Business
{
    public interface IUserManager
    {
        Task<Result<User, Error>> Get(int id);
        Task<Result<PagedList<User>, Error>> GetUsers(UserParams userParams);
        Task<Result<User, Error>> GetCurrent(int userId);
        Task<Result<User, Error>> GetByLike(Like like);
        Task<Result<User, Error>> Add(User entity);
        Task<Result<Like, Error>> Get(Like like);
        Task<Result<Like, Error>> Add(Like entity);
    }
}