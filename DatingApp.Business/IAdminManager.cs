using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DatingApp.Business.Dtos;
using DatingApp.Models;
using DatingApp.Shared;
using DatingApp.Shared.ErrorTypes;

namespace DatingApp.Business
{
    /// <summary>
    /// The administrator manager interface.
    /// </summary>
    public interface IAdminManager
    {
        /// <summary>
        /// Gets the users with their coressponding roles.
        /// </summary>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the list of users with roles.
        /// </returns>
        Task<Result<IEnumerable<UserWithRoles>, Error>> GetUsersWithRoles();

        /// <summary>
        /// Edits the roles for the specified user.
        /// </summary>
        /// <param name="userName">The user name of the user for which to edit the roles.</param>
        /// <param name="roleEditDto">The list of roles to edit for the specified user.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the list of new roles for that user.
        /// </returns>
        Task<Result<IEnumerable<string>, Error>> EditRoles(string userName, RoleEditDto roleEditDto);

        /// <summary>
        /// Gets the list of unapproved photos which required moderation.
        /// </summary>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the list of photos which have not been approved.
        /// </returns>
        Task<Result<IEnumerable<Photo>, Error>> GetPhotosForModeration();

        /// <summary>
        /// Approves the photo with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the photo which should be approved.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// </returns>
        Task<Result<None, Error>> ApprovePhoto(int id);
        
        /// <summary>
        /// Rejects the photo with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the photo which should be rejected.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// </returns>
        Task<Result<None, Error>> RejectPhoto(int id);
    }
}