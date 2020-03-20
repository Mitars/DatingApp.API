using System;
using System.Linq;
using DatingApp.Business;
using DatingApp.Models;
using DatingApp.Shared.ErrorTypes;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DatingApp.DataAccess.Tests
{
    public class UserManagerTest : IDisposable
    {
        private readonly DatabaseFixture fixture;

        private readonly IUserManager userManager;

        public UserManagerTest()
        {
            this.fixture = new DatabaseFixture();
            userManager = fixture.GetService<IUserManager>();

            var likeRepository = fixture.GetService<ILikeRepository>();
            likeRepository.Add(new Like { LikerId = 2, LikeeId = 3 }).GetAwaiter().GetResult();
            likeRepository.Add(new Like { LikerId = 2, LikeeId = 4 }).GetAwaiter().GetResult();
            likeRepository.Add(new Like { LikerId = 3, LikeeId = 2 }).GetAwaiter().GetResult();
        }

        public void Dispose() =>
            this.fixture.Dispose();

        [Fact]
        private async void Get_UserExists_User()
        {
            var user = await userManager.Get(1);
            user.Value.UserName.Should().Be("Mitzi");
        }

        [Fact]
        private async void Get_UserDoesNotExist_Null()
        {
            var user = await userManager.Get(12);
            user.Value.Should().BeNull();
        }

        [Fact]
        private async void Get_UserExists_UserWithIncludedFields()
        {
            var user = await userManager.GetCurrent(1);
            user.Value.UserName.Should().NotBeNull();
        }

        [Fact]
        private async void Get_MultipleMaleUsersExist_Users()
        {
            var userParams = new UserParams()
            {
                Gender = "male"
            };

            var retrievedUsers = await userManager.Get(userParams);
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

            var retrievedUsers = await userManager.Get(userParams);
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

            var retrievedUsers = await userManager.Get(userParams);
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

            var retrievedUsers = await userManager.Get(userParams);
            retrievedUsers.Value.Count().Should().Be(2);
        }

        [Fact]
        private async void Get_UserDoesntExist_NotFoundError()
        {
            var userParams = new UserParams()
            {
                UserId = 12
            };

            var retrievedUsers = await userManager.Get(userParams);
            retrievedUsers.Error.GetType().Should().Be(typeof(NotFoundError));
        }

        [Fact]
        private async void Update_UpdatedUser_UpdatedUser()
        {
            var userResult = await userManager.Get(1);
            var user = userResult.Value;
            user.Country = "Japan";
            user.City = "Tokyo";
            user.Email = "mitzi@gmail.com";
            user.LookingFor = "smart guys";
            user.Interests = "Fitness and cookies";

            var updatedUserResult = await userManager.Update(user);
            updatedUserResult.Value.Should().Match<User>(u =>
                u.Country == "Japan"
                && u.City == "Tokyo"
                && u.Email == "mitzi@gmail.com"
                && u.LookingFor == "smart guys"
                && u.Interests == "Fitness and cookies");
        }

        [Fact]
        private async void Update_UpdatedUserActivity_UpdatedUser()
        {
            var userResult = await userManager.Get(1);
            var originalActivityTime = userResult.Value.LastActive;

            var updatedUserResult = await userManager.UpdateActivity(1);
            updatedUserResult.Value.LastActive.Should().BeOnOrAfter(originalActivityTime);
        }

        [Fact]
        private async void Add_NewLike_Like()
        {
            var like = await this.userManager.AddLike(1, 3);
            like.Value.LikerId.Should().Be(1);
            like.Value.LikeeId.Should().Be(3);
        }

        [Fact]
        private async void Add_DuplicateLike_Error()
        {
            await this.userManager.AddLike(1, 3);
            var duplicateLike = await this.userManager.AddLike(1, 3);
            duplicateLike.Error.GetType().Should().Be(typeof(Error));
        }

        [Fact]
        private async void Add_LikeToNonExistantUser_NotFoundError()
        {
            var like = await this.userManager.AddLike(1, 12);
            like.Error.GetType().Should().Be(typeof(NotFoundError));
        }

        [Fact]
        private async void Delete_LikeExists_Successful()
        {
            await this.userManager.AddLike(3, 4);
            var like = await this.userManager.DeleteLike(3, 4);
            like.IsSuccess.Should().BeTrue();
        }
    }
}
