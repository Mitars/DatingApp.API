using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DatingApp.Models;
using DatingApp.Shared;
using DatingApp.Shared.ErrorTypes;

namespace DatingApp.DataAccess
{
    /// <summary>
    /// The base repository class.
    /// </summary>
    public interface IBaseRepository
    {
        /// <summary>
        /// Gets or sets the data context.
        /// </summary>
        DataContext Context { get; set; }

        /// <summary>
        /// Gets all the entities of the specified type.
        /// </summary>
        /// <param name="Id">The ID of the entity which to get.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the list of entities.
        /// </returns>
        Task<Result<IEnumerable<T>, Error>> Get<T>() where T : class;

        /// <summary>
        /// Gets the entity with the coresponding ID.
        /// </summary>
        /// <param name="Id">The ID of the entity which to get.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the entity.
        /// </returns>
        Task<Result<T, Error>> Get<T>(int Id) where T : class, IBaseEntity;
        
        /// <summary>
        /// Adds a new entity.
        /// </summary>
        /// <typeparam name="T">The entity which should be created.</typeparam>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the created entity.
        /// </returns>
        Task<Result<T, Error>> Add<T>(T entity) where T : class;

        /// <summary>
        /// Updates the entity with the coresponding ID.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// The task result contains the updated entity.
        /// </returns>
        Task<Result<T, Error>> Update<T>(T entity) where T : class;

        /// <summary>
        /// Deletes the entity.
        /// </summary>
        /// <typeparam name="T">The entity to delete.</typeparam>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// </returns>
        Task<Result<None, Error>> Delete<T>(T entity) where T: class;

        /// <summary>
        /// Saves all commits.
        /// </summary>
        /// <returns>
        /// A task result that represents the asynchronous operation.
        /// A boolean value which indicates if the save operation was successful.
        /// </returns>
        Task<bool> SaveAll();
    }
}