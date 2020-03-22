using System;
using System.Linq;
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
    public class AdminManagerTest : IDisposable
    {
        private readonly DatabaseFixture fixture;
        private readonly IPhotoManager photoManager;
        private readonly IAdminManager adminManager;

        public AdminManagerTest()
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
        private async void GetUsersWithRoles_UsersWithRolesExist_UsersWithRoles()
        {
            var usersWithRoles = await this.adminManager.GetUsersWithRoles();
            usersWithRoles.Value.Count().Should().Be(11);
        }

        [Fact]
        private async void GetUsersWithRoles_UsersWithRolesExist_ConfirmedUserRoles()
        {
            var usersWithRoles = await this.adminManager.GetUsersWithRoles();
            var userOne = usersWithRoles.Value.First(u => u.UserName == "Mitzi");
            userOne.Roles.First().Should().Be("Member");
        }

        [Fact]
        private async void EditRoles_UsersWithRolesExist_ConfirmedUserRoles()
        {
            await this.adminManager.EditRoles("Mitzi", new RoleEditDto() { RoleNames = new string[] { "Admin", "Moderator" } });
            var usersWithRoles = await this.adminManager.GetUsersWithRoles();
            var userOne = usersWithRoles.Value.First(u => u.UserName == "Mitzi");
            userOne.Roles.First().Should().Be("Admin");
        }

        [Fact]
        private async void GetPhotosForModeration_NoUnapprovedPhotosExist_NoPhotos()
        {
            var photo = await this.adminManager.GetPhotosForModeration();
            photo.Value.Count().Should().Be(0);
        }

        [Fact]
        private async void GetPhotosForModeration_OneUnapprovedPhotoExists_Photo()
        {
            var photoToCreate = new PhotoForCreationDto
            {
                UserId = 1,
            };
            await this.photoManager.Add(photoToCreate);

            var photo = await this.adminManager.GetPhotosForModeration();
            photo.Value.Count().Should().Be(1);
        }

        [Fact]
        private async void ApprovePhoto_UnapprovedPhoto_Approved()
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
        private async void ApprovePhoto_NonExistentPhoto_UnauthorizedError()
        {
            var result = await adminManager.ApprovePhoto(11);
            result.Error.GetType().Should().Be(typeof(UnauthorizedError));
        }

        [Fact]
        private async void ApprovePhoto_PhotoAlreadyApproved_Error()
        {
            var result = await adminManager.ApprovePhoto(1);
            result.Error.GetType().Should().Be(typeof(Error));
        }

        [Fact]
        private async void ApprovePhoto_FirstPhotoForNewUser_UnauthorizedError()
        {
            var result = await adminManager.ApprovePhoto(11);
            result.Error.GetType().Should().Be(typeof(UnauthorizedError));
        }

        [Fact]
        private async void RejectPhoto_UnapprovedPhoto_NoPhoto()
        {
            var photoToCreate = new PhotoForCreationDto
            {
                UserId = 1,
            };
            var createdPhoto = await this.photoManager.Add(photoToCreate);
            await adminManager.RejectPhoto(createdPhoto.Value.Id);

            var photo = await this.photoManager.Get(createdPhoto.Value.Id);
            photo.Value.Should().BeNull();
        }

        [Fact]
        private async void RejectPhoto_NonExistentPhoto_UnauthorizedError()
        {
            var result = await adminManager.RejectPhoto(11);
            result.Error.GetType().Should().Be(typeof(UnauthorizedError));
        }

        [Fact]
        private async void RejectPhoto_PhotoAlreadyApproved_UnauthorizedError()
        {
            var result = await adminManager.RejectPhoto(1);
            result.Error.GetType().Should().Be(typeof(UnauthorizedError));
        }
    }
}
