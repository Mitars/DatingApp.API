using System;
using System.Linq;
using CSharpFunctionalExtensions;
using DatingApp.Models;
using FluentAssertions;
using Xunit;

namespace DatingApp.DataAccess.Test
{
    public class PhotoMetadataRepositoryTest : IDisposable
    {
        private readonly DatabaseFixture fixture;
        private readonly IPhotoMetadataRepository photoMetadataRepository;
        private readonly IBaseRepository baseRepository;

        public PhotoMetadataRepositoryTest()
        {
            this.fixture = new DatabaseFixture();
            this.baseRepository = new BaseRepository(fixture.DatabaseContext);
            this.photoMetadataRepository = new PhotoMetadataRepository(this.baseRepository);
        }

        public void Dispose() =>
            this.fixture.Dispose();

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
        private async void GetPhotosForModeration_PhotoForModerationExists_OnePhotoForModerationFound()
        {
            var photo = await this.photoMetadataRepository.Get(1)
                .Tap(p => p.IsApproved = false)
                .Bind(this.photoMetadataRepository.Update);

            var photos = await this.photoMetadataRepository.GetPhotosForModeration();
            photos.Value.Count().Should().Be(1);
        }

        [Fact]
        private async void UpdateMainForUser_PhotoExists_Successful()
        {
            var photoToCreate = new Photo()
            {
                UserId = 1,
            };
            await this.photoMetadataRepository.Add(photoToCreate);

            var updatedPhoto = await this.photoMetadataRepository.UpdateMainForUser(1, 2);
            updatedPhoto.Value.IsMain.Should().BeTrue();
        }

        [Fact]
        private async void Delete_PhotoExists_Successful()
        {
            var photoToCreate = new Photo();
            var photoToDelete = await this.photoMetadataRepository.Add(photoToCreate);

            var photo = await this.photoMetadataRepository.Delete(photoToDelete.Value);
            photo.IsSuccess.Should().BeTrue();
        }
    }
}
