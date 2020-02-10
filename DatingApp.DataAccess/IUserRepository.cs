using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DatingApp.Models;
using DatingApp.Shared;

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
        /// Adds a new user.
        /// </summary>
        /// <typeparam name="user">The user to create.</typeparam>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the created user.
        /// </returns>
        Task<Result<User, Error>> Add(User user);
                
        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <typeparam name="user">The user to update.</typeparam>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the updated user.
        /// </returns>
        Task<Result<User, Error>> Update(User user);    
    }
}