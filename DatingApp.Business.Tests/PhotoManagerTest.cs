using System;
using AutoMapper;
using DatingApp.Business;
using DatingApp.Business.Dtos;
using DatingApp.DataAccess.Dtos;
using DatingApp.Models;
using DatingApp.Shared;
using DatingApp.Shared.ErrorTypes;
using DatingApp.Shared.FunctionalExtensions;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace DatingApp.DataAccess.Tests
{
    public class PhotoManagerTest : IDisposable
    {
        private readonly DatabaseFixture fixture;
        private readonly IPhotoManager photoManager;
        private readonly IAdminManager adminManager;

        public PhotoManagerTest()
        {
            this.fixture = new DatabaseFixture();

            var mapper = fixture.GetService<IMapper>();
            var photoMetadataRepository = fixture.GetService<IPhotoMetadataRepository>();
            var identityUserManager = fixture.GetService<UserManager<User>>();
            var userRepository = fixture.GetService<IUserRepository>();
            var photoRepositoryMock = new Mock<IPhotoRepository>();
            photoRepositoryMock.Setup(p => p.Add(It.IsAny<PhotoToUpload>()))
                .ReturnsAsync(new CreatedPhoto() { PublicId = "123", Url = "http://unittest.com/" }.Success());
            photoRepositoryMock.Setup(p => p.Delete(It.IsAny<string>()))
                .ReturnsAsync(new None().Success());
            this.photoManager = new PhotoManager(mapper, photoMetadataRepository, userRepository, photoRepositoryMock.Object);

            this.adminManager = new AdminManager(photoMetadataRepository, identityUserManager, userRepository, photoRepositoryMock.Object);
        }

        public void Dispose() =>
            this.fixture.Dispose();

        [Fact]
        private async void Get_PhotoMetadataExists_PhotoMetadata()
        {
            var photo = await this.photoManager.Get(1);
            photo.Value.Should().NotBeNull();
        }

        [Fact]
        private async void Get_PhotoMetadataDoesNotExist_Null()
        {
            var photo = await this.photoManager.Get(12);
            photo.Value.Should().BeNull();
        }

        [Fact]
        private async void Add_NewPhoto_Photo()
        {
            var photoToCreate = new PhotoForCreationDto
            {
                UserId = 1,
            };
            var createdPhoto = await this.photoManager.Add(photoToCreate);
            await adminManager.ApprovePhoto(createdPhoto.Value.Id);

            var photo = await this.photoManager.Get(createdPhoto.Value.Id);
            photo.Value.Should().NotBeNull();
        }

        [Fact]
        private async void UpdateMainForUser_PhotoExists_Successful()
        {
            var photoToCreate = new PhotoForCreationDto
            {
                UserId = 1,
            };
            var createdPhoto = await this.photoManager.Add(photoToCreate);
            await adminManager.ApprovePhoto(createdPhoto.Value.Id);

            var updatedPhoto = await this.photoManager.SetAsMain(1, createdPhoto.Value.Id);
            updatedPhoto.Value.IsMain.Should().BeTrue();
        }

        [Fact]
        private async void UpdateMainForUser_PhotoDoesntExist_UnauthorizedError()
        {
            var updatedPhoto = await this.photoManager.SetAsMain(1, 11);
            updatedPhoto.Error.GetType().Should().Be(typeof(UnauthorizedError));
        }

        [Fact]
        private async void UpdateMainForUser_PhotoAlreadySetAsMain_Error()
        {
            var updatedPhoto = await this.photoManager.SetAsMain(1, 1);
            updatedPhoto.Error.GetType().Should().Be(typeof(Error));
        }

        [Fact]
        private async void UpdateMainForUser_PhotoNotFromCurrentUser_UnauthorizedError()
        {
            var updatedPhoto = await this.photoManager.SetAsMain(1, 2);
            updatedPhoto.Error.GetType().Should().Be(typeof(UnauthorizedError));
        }

        [Fact]
        private async void UpdateMainForUser_PhotoUnapprovd_Error()
        {
            var photoToCreate = new PhotoForCreationDto
            {
                UserId = 1,
            };
            var createdPhoto = await this.photoManager.Add(photoToCreate);

            var updatedPhoto = await this.photoManager.SetAsMain(1, createdPhoto.Value.Id);
            updatedPhoto.Error.GetType().Should().Be(typeof(Error));
        }

        [Fact]
        private async void Delete_PhotoExists_Successful()
        {
            var photoToCreate = new PhotoForCreationDto
            {
                UserId = 1,
                DateAdded = DateTime.Now,
                Description = "This is a work of art",
                FileName = "infinityserver3",
                Stream = null
            };
            var createdPhoto = await this.photoManager.Add(photoToCreate);
            var result = await adminManager.ApprovePhoto(createdPhoto.Value.Id);

            var photo = await this.photoManager.Delete(1, createdPhoto.Value.Id);
            photo.IsSuccess.Should().BeTrue();
        }

        [Fact]
        private async void Delete_PhotoExists_Successful1()
        {
            var photoToCreate = new PhotoForCreationDto
            {
                UserId = 1,
                DateAdded = DateTime.Now,
                Description = "This is a work of art",
                FileName = "infinityserver3",
                Stream = null
            };
            var createdPhoto = await this.photoManager.Add(photoToCreate);
            var result = await adminManager.ApprovePhoto(createdPhoto.Value.Id);
            var updatedPhoto = await this.photoManager.SetAsMain(1, createdPhoto.Value.Id);

            var photo = await this.photoManager.Delete(1, 1);
            photo.IsSuccess.Should().BeTrue();
        }

        [Fact]
        private async void Delete_PhotoExists_Successful2()
        {
            var photo = await this.photoManager.Delete(1, 2);
            photo.Error.GetType().Should().Be(typeof(UnauthorizedError));
        }

        [Fact]
        private async void Delete_PhotoExists_Successful3()
        {
            var photo = await this.photoManager.Delete(1, 1);
            photo.Error.GetType().Should().Be(typeof(Error));
        }
    }
}
