using System;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    /// <summary>
    /// The dating repository class.
    /// Creates, deletes and fetches the user details. 
    /// </summary>
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatingRepository"/> class.
        /// </summary>
        /// <param name="context">The data context.</param>
        public DatingRepository(DataContext context)
        {
            this.context = context;
        }

        /// <inheritdoc/>
        public void Add<T>(T entity) where T : class
        {
            this.context.Add(entity);
        }

        /// <inheritdoc/>
        public void Delete<T>(T entity) where T : class
        {
            this.context.Remove(entity);
        }

        /// <inheritdoc/>
        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {            
            var minDateOfBirth = DateTime.Today.AddYears(-userParams.MaxAge - 1);
            var maxDateOfBirth = DateTime.Today.AddYears(-userParams.MinAge);

            var users = this.context.Users.Include(u => u.Photos)
                .Where(u => u.Id != userParams.UserId)
                .Where(u => u.Gender == userParams.Gender)
                .Where(u => u.DateOfBirth >= minDateOfBirth)
                .Where(u => u.DateOfBirth <= maxDateOfBirth);

            switch (userParams?.OrderBy) {
                case "created":
                    users = users.OrderByDescending(u => u.Created);
                    break;
                default:
                    users = users.OrderByDescending(u => u.LastActive);
                    break;
            }     
            
            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        /// <inheritdoc/>
        public async Task<User> GetUser(int Id)
        {
            var user = await this.context.Users.Include(u => u.Photos).FirstOrDefaultAsync(u => u.Id == Id);
            return user;
        }

        /// <inheritdoc/>
        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await this.context.Photos.FirstOrDefaultAsync(p => p.Id == id);
            return photo;
        }

        /// <inheritdoc/>
        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            var photo = await this.context.Photos.Where(u => u.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
            return photo;
        }


        /// <inheritdoc/>
        public async Task<bool> SaveAll()
        {
            return await this.context.SaveChangesAsync() > 0;
        }
    }
}