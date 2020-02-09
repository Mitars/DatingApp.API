using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DatingApp.Models;
using DatingApp.Shared;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.DataAccess
{
    public class BaseRepository : IBaseRepository
    {
        public BaseRepository(DataContext dataContext)
        {
            this.Context = dataContext;
        }
        
        public DataContext Context { get; set; }

        /// <inheritdoc/>
        public async Task<Result<T, Error>> Get<T>(int Id) where T : class, IBaseEntity =>
            await this.Context.Set<T>().FirstOrDefaultAsync(u => u.Id == Id).Success();
        

        /// <inheritdoc/>
        public async Task<Result<T, Error>> Add<T>(T entity) where T : class
        {
            await this.Context.AddAsync(entity);
            var SaveSuccessful = await this.SaveAll();
            return entity.SuccessIf<T, DatabaseError>(SaveSuccessful, "Failed saving adding entity to the database");
        }

        /// <inheritdoc/>
        public async Task<Result<None, Error>> Update<T>(T entity) where T : class
        {
            this.Context.Update(entity);
            var SaveSuccessful = await this.SaveAll();
            return new None().SuccessIf<None, DatabaseError>(SaveSuccessful, "Failed saving adding entity to the database");
        }

        /// <inheritdoc/>
        public async Task<Result<None, Error>> Delete<T>(T entity) where T : class
        {
            this.Context.Remove(entity);
            var SaveSuccessful = await this.SaveAll();
            return new None().SuccessIf<None, DatabaseError>(SaveSuccessful, "Failed saving adding entity to the database");
        }
        
        /// <inheritdoc/>
        public async Task<bool> SaveAll() =>
            await this.Context.SaveChangesAsync() > 0;
    }
}