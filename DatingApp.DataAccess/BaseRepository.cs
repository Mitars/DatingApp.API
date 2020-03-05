using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DatingApp.Models;
using DatingApp.Shared;
using DatingApp.Shared.ErrorTypes;
using DatingApp.Shared.FunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.DataAccess
{
    /// <summary>
    /// The base repository class.
    /// Contains methods that can be used by repositories to implement the basic CRUD operations.
    /// </summary>
    public class BaseRepository : IBaseRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRepository"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        public BaseRepository(DataContext dataContext) =>
            this.Context = dataContext;

        /// <inheritdoc />
        public DataContext Context { get; set; }

        /// <inheritdoc />
        public async Task<Result<IEnumerable<T>, Error>> Get<T>() where T : class =>
            await this.Context.Set<T>().ToListAsync().Success();

        /// <inheritdoc />
        public async Task<Result<T, Error>> Get<T>(int Id) where T : class, IBaseEntity =>
            await this.Context.Set<T>().FirstOrDefaultAsync(u => u.Id == Id).Success();        

        /// <inheritdoc />
        public async Task<Result<T, Error>> Add<T>(T entity) where T : class
        {
            await this.Context.AddAsync(entity);
            var SaveSuccessful = await this.SaveAll();
            return entity.SuccessIf<T, DatabaseError>(SaveSuccessful, "Failed saving adding entity to the database");
        }

        /// <inheritdoc />
        public async Task<Result<T, Error>> Update<T>(T entity) where T : class
        {
            this.Context.Update(entity);
            var SaveSuccessful = await this.SaveAll();
            return entity.SuccessIf<T, DatabaseError>(SaveSuccessful, "Failed saving adding entity to the database");
        }

        /// <inheritdoc />
        public async Task<Result<None, Error>> Delete<T>(T entity) where T : class
        {
            this.Context.Remove(entity);
            var SaveSuccessful = await this.SaveAll();
            return new None().SuccessIf<None, DatabaseError>(SaveSuccessful, "Failed saving adding entity to the database");
        }
        
        /// <inheritdoc />
        public async Task<bool> SaveAll() =>
            await this.Context.SaveChangesAsync() > 0;
    }
}