using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DatingApp.Models;
using DatingApp.Shared;

namespace DatingApp.DataAccess
{
    /// <summary>
    /// The dating repository inferface.
    /// </summary>
    public interface IDatingRepository
    {   
        /// <summary>
        /// Gets the user with the coresponding ID.
        /// </summary>
        /// <param name="Id">The ID of the user which to get.</param>
        /// <param name="isCurrentUser">A value which indicates whether or not this is the current user.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the user.
        /// </returns>
        Task<Result<T, Error>> Get<T>(int Id) where T : class, IBaseEntity;
        
        /// <summary>
        /// Adds a new entity.
        /// </summary>
        /// <typeparam name="T">The entity which to create.</typeparam>
        Task<Result<T, Error>> Add<T>(T entity) where T : class;

        /// <summary>
        /// Deletes the entity.
        /// </summary>
        /// <typeparam name="T">The entity to delete.</typeparam>
        void Delete<T>(T entity) where T: class;
    }
}