using CSharpFunctionalExtensions;
using DatingApp.Models;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace DatingApp.DataAccess.Test
{
    [Collection("Database collection")]
    public class PhotoMetadataRepositoryTest : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture fixture;
        private readonly IPhotoMetadataRepository photoMetadataRepository;
        private readonly IBaseRepository baseRepository;

        public PhotoMetadataRepositoryTest(DatabaseFixture fixture)
        {
            this.fixture = fixture;
            this.baseRepository = new BaseRepository(fixture.DatabaseContext);
            this.photoMetadataRepository = new PhotoMetadataRepository(this.baseRepository);
        }

        [Fact]
        private async void Get_PhotoMetadataExists_PhotoMetadata()
        {
            var user = await this.photoMetadataRepository.Get(1);
            user.Value.Should().NotBeNull();
        }

        [Fact]
        private async void Get_PhotoMetadataDoesNotExist_Null()
        {
            var user = await this.photoMetadataRepository.Get(12);
            user.Value.Should().BeNull();
        }

        [Fact]
        private async void Add_NewPhoto_Photo()
        {
            var photoToCreate = new Photo()
            {
                UserId = 1,
            };
            var photo = await this.photoMetadataRepository.Add(photoToCreate);
            photo.Value.Should().NotBeNull();
        }

        [Fact]
        private async void GetPhotosForModeration_PhotoForModerationExists_Photo()
        {
            var photo = await this.photoMetadataRepository.Get(1)
                .Tap(p => p.isApproved = false)
                .Bind(this.photoMetadataRepository.Update);

            var photos = await this.photoMetadataRepository.GetPhotosForModeration();
            photos.Value.Count().Should().BeGreaterThan(0);
        }
    }
}
