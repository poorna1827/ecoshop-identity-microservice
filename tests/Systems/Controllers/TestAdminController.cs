

using IdentityMicroservice.Controllers;
using IdentityMicroservice.DbContexts;
using IdentityMicroservice.Dto;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using tests.MockData;
using Xunit;

namespace tests.Systems.Controllers
{
    public class TestAdminController : IDisposable
    {
        private readonly IdentityDbContext _context;
        public TestAdminController()
        {
            var options = new DbContextOptionsBuilder<IdentityDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

            _context = new IdentityDbContext(options);

            _context.Database.EnsureCreated();

        }
        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();

        }

        [Fact]
        public async Task AdminRegisterAsync_ShouldReturn200Status()
        {
            //Arrange

            //database mock
            _context.Admins.AddRange(AdminMockData.GetSampleAdminItems());
            _context.SaveChanges();

            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(x => x["Authentication:SecretKey"]).Returns("sample_password_for_token");

            var sut = new AdminController(_context, configurationMock.Object);



            //Act
            AdminRegisterDto data = new AdminRegisterDto()
            {
                Email = "test-email@gmail.com",
                Password = "Test password",
                Name = "Test"

            };
            var result = await sut.AdminRegister(data);



            //Assert
            result.GetType().Should().Be(typeof(OkResult));

        }

        [Fact]
        public async Task AdminRegisterAsync_ShouldReturn409Status()
        {
            //Arrange

            //database mock
            _context.Admins.AddRange(AdminMockData.GetSampleAdminItems());
            _context.SaveChanges();

            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(x => x["Authentication:SecretKey"]).Returns("sample_password_for_token");

            var sut = new AdminController(_context, configurationMock.Object);



            //Act
            AdminRegisterDto data = new AdminRegisterDto()
            {
                Email = "admin@gmail.com",
                Password = "Test password",
                Name = "Test"

            };
            var result = await sut.AdminRegister(data);



            //Assert
            result.GetType().Should().Be(typeof(ConflictResult));

        }

        [Fact]
        public void AdminLoginAsync_ShouldReturn200Status()
        {
            //Arrange

            //database mock
            _context.Admins.AddRange(AdminMockData.GetSampleAdminItems());
            _context.SaveChanges();

            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(x => x["Authentication:SecretKey"]).Returns("sample_password_for_tokenkey_98&^3_34@^5dfr");
            configurationMock.Setup(x => x["Authentication:Issuer"]).Returns("sample_issuer");
            configurationMock.Setup(x => x["Authentication:Audience"]).Returns("sample_audience");

            var sut = new AdminController(_context, configurationMock.Object);



            //Act
            LoginDto data = new LoginDto()
            {
                Email = "admin@gmail.com",
                Password = "Admin@123",

            };
            var result = sut.AdminLogin(data);



            //Assert
            result.GetType().Should().Be(typeof(OkObjectResult));

        }

        [Fact]
        public void AdminLoginAsync_ShouldReturn404Status()
        {
            //Arrange

            //database mock
            _context.Admins.AddRange(AdminMockData.GetSampleAdminItems());
            _context.SaveChanges();

            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(x => x["Authentication:SecretKey"]).Returns("sample_password_for_token");
            configurationMock.Setup(x => x["Authentication:Issuer"]).Returns("sample_issuer");
            configurationMock.Setup(x => x["Authentication:Audience"]).Returns("sample_audience");

            var sut = new AdminController(_context, configurationMock.Object);



            //Act
            LoginDto data = new LoginDto()
            {
                Email = "invalid-email@gmail.com",
                Password = "Admin@123",

            };
            var result = sut.AdminLogin(data);



            //Assert
            result.GetType().Should().Be(typeof(NotFoundResult));

        }

        [Fact]
        public void AdminLoginAsync_ShouldReturn401Status()
        {
            //Arrange

            //database mock
            _context.Admins.AddRange(AdminMockData.GetSampleAdminItems());
            _context.SaveChanges();

            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(x => x["Authentication:SecretKey"]).Returns("sample_password_for_token");
            configurationMock.Setup(x => x["Authentication:Issuer"]).Returns("sample_issuer");
            configurationMock.Setup(x => x["Authentication:Audience"]).Returns("sample_audience");

            var sut = new AdminController(_context, configurationMock.Object);



            //Act
            LoginDto data = new LoginDto()
            {
                Email = "admin@gmail.com",
                Password = "incorrect-password",

            };
            var result = sut.AdminLogin(data);



            //Assert
            result.GetType().Should().Be(typeof(UnauthorizedResult));

        }
    }
}
