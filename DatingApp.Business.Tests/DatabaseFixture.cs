using System;
using DatingApp.API;
using DatingApp.API.Data;
using DatingApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DatingApp.DataAccess.Tests
{
    /// <summary>
    /// The database fixture class used for testing.
    /// </summary>
    public class DatabaseFixture : IDisposable
    {
        private ServiceCollection services;
        private IServiceScope scope;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseFixture"/> class.
        /// </summary>
        public DatabaseFixture()
        {
            string databaseName = Guid.NewGuid().ToString();

            services = new ServiceCollection();
            services.AddDbContext<DataContext>(o => o.UseInMemoryDatabase(databaseName: databaseName));

            var builder = services.AddIdentityCore<User>(opt =>
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 4;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
            });

            builder = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);
            builder.AddEntityFrameworkStores<DataContext>();
            builder.AddRoleValidator<RoleValidator<Role>>();
            builder.AddRoleManager<RoleManager<Role>>();
            builder.AddSignInManager<SignInManager<User>>();

            var provider = services.BuildServiceProvider();

            using (var scope = provider.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
                var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
                DependencyInjectionConfiguration.Initialize(services, new ConfigurationBuilder().Build());
                Seed.SeedUsers(userManager, roleManager);
            }
        }

        /// <summary>
        /// Gets the database context.
        /// </summary>
        public DataContext DatabaseContext { get; private set; }

        /// <inheritdoc />
        public void Dispose() =>
            this.scope.Dispose();

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <typeparam name="T">The type of the service.</typeparam>
        /// <returns>Gets the requested service.</returns>
        public T GetService<T>()
        {
            this.scope = services.BuildServiceProvider().CreateScope();
            return scope.ServiceProvider.GetRequiredService<T>();
        }
    }
}
