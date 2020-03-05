using DatingApp.Models;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace DatingApp.DataAccess.Test
{
    public class UserRepositoryTest : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture fixture;
        private readonly IUserRepository userRepository;
        private readonly IBaseRepository baseRepository;

        public UserRepositoryTest(DatabaseFixture fixture)
        {
            this.fixture = fixture;
            this.baseRepository = new BaseRepository(fixture.DatabaseContext);
            this.userRepository = new UserRepository(this.baseRepository);
        }

        [Fact]
        private async void Get_UserExists_User()
        {
            var user = await userRepository.Get(1);
            user.Value.UserName.Should().NotBeNull();
        }

        [Fact]
        private async void Get_UserDoesNotExist_Null()
        {
            var user = await userRepository.Get(12);
            user.Value.Should().BeNull();
        }

        [Fact]
        private async void GetUserWithRoles_UserExists_UserWithRoles()
        {
            var users = await userRepository.GetWithRoles();
            users.Value.First().UserRoles.Count().Should().BeGreaterThan(0);
        }

        [Fact]
        private async void GetExcludingQueryFilters_UserExists_UserWithoutQueryFilters()
        {
            var user = await userRepository.GetExcludingQueryFilters(1);
            user.Value.UserName.Should().NotBeNull();
        }

        [Fact]
        private async void GetExcludingQueryFilters_UserDoesNotExist_Null()
        {
            var user = await userRepository.GetExcludingQueryFilters(12);
            user.Value.Should().BeNull();
        }

        [Fact]
        private async void Update_ValuesUpdated_ListOfRoles()
        {
            var userResult = await userRepository.Get(1);

            var user = userResult.Value;

            user.Country = "Japan";
            user.City = "Tokyo";
            user.Email = "mitzi@gmail.com";
            user.LookingFor = "smart guys";
            user.Interests = "Fitness and cookies";

            var updatedUserResult = await userRepository.Update(user);
            updatedUserResult.Value.Should().Match<User>(u =>
                u.Country== "Japan"
                && u.City == "Tokyo"
                && u.Email == "mitzi@gmail.com"
                && u.LookingFor == "smart guys"
                && u.Interests == "Fitness and cookies");
        }

        [Fact]
        private async void GetRoles_UserExists_ListOfRoles()
        {
            var user = await userRepository.GetWithRoles();
            var roles = userRepository.GetRoles(user.Value.First());
            roles.Value.Count().Should().BeGreaterThan(0);
        }

        [Fact]
        private async void GetInMemoryPersonRepository2()
        {
            var messageRepository = new MessageRepository(this.baseRepository);

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
