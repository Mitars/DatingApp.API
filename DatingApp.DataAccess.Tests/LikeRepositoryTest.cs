using DatingApp.Models;
using FluentAssertions;
using Xunit;

namespace DatingApp.DataAccess.Test
{
    [Collection("Database collection")]
    public class LikeRepositoryTest : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture fixture;
        private readonly IBaseRepository baseRepository;
        private readonly ILikeRepository likeRepository;

        public LikeRepositoryTest(DatabaseFixture fixture)
        {
            this.fixture = fixture;
            this.baseRepository = new BaseRepository(fixture.DatabaseContext);
            this.likeRepository = new LikeRepository(this.baseRepository);
        }

        [Fact]
        private async void Get_LikeExists_Like()
        {
            var likeToCreate = new Like()
            {
                LikerId = 1,
                LikeeId = 2,
            };
            await this.likeRepository.Add(likeToCreate);

            var like = await this.likeRepository.Get(1, 2);
            like.Value.Should().NotBeNull();
        }

        [Fact]
        private async void LikeDoesNotExist_Null()
        {
            var likeToCreate = new Like()
            {
                LikerId = 0,
                LikeeId = 1,
            };

            var like = await this.likeRepository.Get(2, 1);
            like.Value.Should().BeNull();
        }

        [Fact]
        private async void Add_NewLike_Like()
        {
            var likeToCreate = new Like()
            {
                LikerId = 2,
                LikeeId = 3
            };
            var like = await this.likeRepository.Add(likeToCreate);
            like.Value.Should().NotBeNull();
        }

        [Fact]
        private async void Delete_LikeExists_Successful()
        {
            var likeToCreate = new Like()
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
