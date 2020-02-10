using DatingApp.Business;
using DatingApp.DataAccess;
using DatingApp.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DatingApp.API
{
    /// <summary>
    /// Configures the dependency injection for the various layers in the application.
    /// </summary>
    public class DependencyInjectionConfiguration
    {
        /// <summary>
        /// Initializes the dependency injection for the various layers in the application.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="configuration">The configuration.</param>
        public static void Initialize(IServiceCollection services, IConfiguration configuration) {
            SetupBusinessLayer(services,configuration);
            SetupRepositoryLayer(services, configuration);
        }   

        /// <summary>
        /// Initializes the dependency injection for business layer.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="configuration">The configuration.</param>
        private static void SetupBusinessLayer(IServiceCollection services, IConfiguration configuration) {
            services.AddScoped<IUserManager, UserManager>();
            services.AddScoped<IMessageManager, MessageManager>();
            services.AddScoped<IPhotoManager, PhotoManager>();
        }

        /// <summary>
        /// Initializes the dependency injection for repository layer.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <param name="configuration">The configuration.</param>
        private static void SetupRepositoryLayer(IServiceCollection services, IConfiguration configuration) {
            services.AddScoped<IBaseRepository, BaseRepository>();
            services.AddScoped<IDatingRepository, DatingRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ILikeRepository, LikeRepository>();
            services.AddScoped<IPhotoRepository, PhotoRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();

            services.AddScoped<ICloudinaryRepository, CloudinaryRepository>();
            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
        }
    }
}
