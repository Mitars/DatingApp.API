using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DatingApp.Models;
using DatingApp.Shared.ErrorTypes;

namespace DatingApp.DataAccess
{
    /// <summary>
    /// The user repository inferface.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Gets the user with the coresponding ID.
        /// </summary>
        /// <param name="id">The ID of the user which to get.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the user.
        /// </returns>
        Task<Result<User, Error>> Get(int id);

        /// <summary>
        /// Gets the user with the coresponding ID.
        /// Exclude any global filters that may apply.
        /// </summary>
        /// <param name="id">The ID of the user which to get.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the user.
        /// </returns>
        Task<Result<User, Error>> GetExcludingQueryFilters(int id);

        /// <summary>
        /// Gets the list of users depending on the specified params.
        /// </summary>
        /// <param name="userParams">The user params used for filtering the user list.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the paged list of users.
        /// </returns>
        Task<Result<PagedList<User>, Error>> Get(UserParams userParams);

        /// <summary>
        /// Gets the user with roles.
        /// </summary>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the user.
        /// </returns>
        Task<Result<IEnumerable<User>, Error>> GetWithRoles();

        /// <summary>
        /// Gets the roles for the specified user.
        /// </summary>
        /// <param name="user">The user roles.</param>
        /// <returns>The list of user roles.</returns>
        Result<IEnumerable<string>, Error> GetRoles(User user);

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the updated user.
        /// </returns>
        Task<Result<User, Error>> Update(User user);
    }
}