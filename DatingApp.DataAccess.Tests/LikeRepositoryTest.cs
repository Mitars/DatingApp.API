using System;
using DatingApp.Models;
using FluentAssertions;
using Xunit;

namespace DatingApp.DataAccess.Tests
{
    public class LikeRepositoryTest : IDisposable
    {
        private readonly DatabaseFixture fixture;
        private readonly ILikeRepository likeRepository;

        public LikeRepositoryTest()
        {
            this.fixture = new DatabaseFixture();
            this.likeRepository = fixture.GetService<ILikeRepository>();
        }

        public void Dispose() =>
            this.fixture.Dispose();

        [Fact]
        private async void Get_LikeExists_Like()
         {
            var likeToCreate = new Like
            {
                LikerId = 1,
                LikeeId = 2,
            };
            await this.likeRepository.Add(likeToCreate);

            var like = await this.likeRepository.Get(1, 2);
            like.Value.Should().NotBeNull();
        }

        [Fact]
        private async void Get_LikeDoesNotExist_Null()
        {
            var like = await this.likeRepository.Get(1, 0);
            like.Value.Should().BeNull();
        }

        [Fact]
        private async void Add_NewLike_Like()
        {
            var likeToCreate = new Like
            {
                LikerId = 1,
                LikeeId = 3
            };
            var like = await this.likeRepository.Add(likeToCreate);
            like.Value.Should().NotBeNull();
        }

        [Fact]
        private async void Delete_LikeExists_Successful()
        {
            var likeToCreate = new Like
            {
                LikerId = 3,
                LikeeId = 4,
            };
            var likeToDelete = await this.likeRepository.Add(likeToCreate);

            var like = await this.likeRepository.Delete(likeToDelete.Value);
            like.IsSuccess.Should().BeTrue();
        }
    }
}
