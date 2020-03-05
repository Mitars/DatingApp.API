using DatingApp.API.Data;
using DatingApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace DatingApp.DataAccess.Test
{
    public class UnitTest1
    {
        

        [Fact]
        public void PassingTest()
        {
            Assert.Equal(2, 2);
        }

        [Fact]
        public void FailingTest()
        {
            Assert.Equal(2, 3);
        }

        [Fact]
        private void GetInMemoryPersonRepository()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "test_dating_app_db")
                .Options;

            //var services = new ServiceCollection();
            //services.AddIdentity<UserManager<User>, RoleManager<Role>>()
            //    .AddEntityFrameworkStores<DataContext>();
            //var provider = services.BuildServiceProvider();


            var services = new ServiceCollection();
            services.AddDbContext<DataContext>(o => o.UseInMemoryDatabase(databaseName: "test_dating_app_db"));

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
                var services2 = scope.ServiceProvider;
                //var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var context = services2.GetRequiredService<DataContext>();
                var userManager = services2.GetRequiredService<UserManager<User>>();
                var roleManager = services2.GetRequiredService<RoleManager<Role>>();
                //context.Database.Migrate();
                Seed.SeedUsers(userManager, roleManager);
            }

            using (var context = new DataContext(options))
            {
                var baseRepository = new BaseRepository(context);

                var userRepository = new UserRepository(baseRepository);

                var userOne = userRepository.Get(1);

                var messageRepository = new MessageRepository(baseRepository);

                var message = new Message()
                {
                    SenderId = 1,
                    RecipientId = 2,
                    Content = "Hello Mike",
                    IsRead = false,
                    MessageSent = DateTime.UtcNow,
                    SenderDeleted = false,
                    RecipientDeleted = false
                };

                var messagy = messageRepository.Get(1).Result;
                messageRepository.Add(message).Wait();
                var messagy2 = messageRepository.Get(1).Result;
                messageRepository.Delete(messagy2.Value).Wait();
                var messagy3 = messageRepository.Get(1).Result;
            }
        }
    }
}
