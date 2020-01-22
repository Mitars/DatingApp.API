using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    /// <summary>
    /// The data context class.
    /// </summary>
    public class DataContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataContext" /> class.
        /// </summary>
        /// <param name="options">The data context options.</param>
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the users.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Gets or sets the photos.
        /// </summary>
        public DbSet<Photo> Photos { get; set; }

        /// <summary>
        /// Gets or sets the likes.
        /// </summary>
        public DbSet<Like> Likes { get; set; }

        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        public DbSet<Value> Values { get; set; }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Like>()
                .HasKey(k => new { k.LikerId, k.LikeeId });

            builder.Entity<Like>()
                .HasOne(l => l.Likee)
                .WithMany(u => u.Likers)
                .HasForeignKey(l => l.LikeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Like>()
                .HasOne(l => l.Liker)
                .WithMany(u => u.Likees)
                .HasForeignKey(l => l.LikerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}