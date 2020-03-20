using System;
using System.Linq;
using DatingApp.Models;
using FluentAssertions;
using Xunit;

namespace DatingApp.DataAccess.Tests
{
    public class UserRepositoryTest : IDisposable
    {
        private readonly DatabaseFixture fixture;
        private readonly IUserRepository userRepository;
        private readonly IBaseRepository baseRepository;

        public UserRepositoryTest()
        {
            this.fixture = new DatabaseFixture();
            this.baseRepository = new BaseRepository(fixture.DatabaseContext);
            this.userRepository = new UserRepository(this.baseRepository);

            var likeRepository = new LikeRepository(this.baseRepository);
            likeRepository.Add(new Like { LikerId = 2, LikeeId = 3 }).GetAwaiter().GetResult();
            likeRepository.Add(new Like { LikerId = 2, LikeeId = 4 }).GetAwaiter().GetResult();
            likeRepository.Add(new Like { LikerId = 3, LikeeId = 2 }).GetAwaiter().GetResult();
        }

        public void Dispose() =>
            this.fixture.Dispose();

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
        private async void GetWithRoles_UserExists_UserWithRoles()
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
        private async void Get_MultipleUsersExist_Users()
        {
            var userParams = new UserParams();

            var retrievedUsers = await userRepository.Get(userParams);
            retrievedUsers.Value.Count().Should().Be(0);
        }

        [Fact]
        private async void Get_MultipleMaleUsersExist_Users()
        {
            var userParams = new UserParams()
            {
                Gender = "male"
            };

            var retrievedUsers = await userRepository.Get(userParams);
            retrievedUsers.Value.Count().Should().Be(5);
        }

        [Fact]
        private async void Get_MultipleMaleUsersExistAgeRange_Users()
        {
            var userParams = new UserParams()
            {
                MinAge = 30,
                MaxAge = 40,
                Gender = "male"
            };

            var retrievedUsers = await userRepository.Get(userParams);
            retrievedUsers.Value.Count().Should().Be(2);
        }

        [Fact]
        private async void Get_UserExistAsLikee_User()
        {
            var userParams = new UserParams()
            {
                UserId = 2,
                Likers = true
            };

            var retrievedUsers = await userRepository.Get(userParams);
            retrievedUsers.Value.Count().Should().Be(1);
        }

        [Fact]
        private async void Get_MultipleUsersExistAsLikers_Users()
        {
            var userParams = new UserParams()
            {
                UserId = 2,
                Likees = true
            };

            var retrievedUsers = await userRepository.Get(userParams);
            retrievedUsers.Value.Count().Should().Be(2);
        }

        [Fact]
        private async void Update_UpdatedUser_UpdatedUser()
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
                u.Country == "Japan"
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
    }
}
