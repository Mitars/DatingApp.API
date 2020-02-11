using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DatingApp.Business.Dtos;
using DatingApp.Models;
using DatingApp.Shared;

namespace DatingApp.Business
{
    /// <summary>
    /// The auth manager interface.
    /// </summary>
    public interface IAdminManager
    {
        Task<Result<IEnumerable<UserWithRoles>, Error>> GetUsersWithRoles();

        Task<Result<IEnumerable<string>, Error>> EditRoles(string userName, RoleEditDto roleEditDto);

        Task<Result<IEnumerable<Photo>, Error>> GetPhotosForModeration();

        Task<Result<None, Error>> ApprovePhoto(int id);
        
        Task<Result<None, Error>> RejectPhoto(int id);
    }
}