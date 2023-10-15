

using IdentityMicroservice.Controllers;
using IdentityMicroservice.DbContexts;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Moq;
using tests.MockData;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using IdentityMicroservice.Dto;
using FluentAssertions;

namespace tests.Systems.Controllers
{
    public class TestUserController : IDisposable
    {
        private readonly IdentityDbContext _context;
        public TestUserController()
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
        public async Task UserRegisterAsync_ShouldReturn200Status()
        {
            //Arrange

            //database mock
            _context.Users.AddRange(UserMockData.GetSampleUserItems());
            _context.SaveChanges();

            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(x => x["Authentication:SecretKey"]).Returns("sample_password_for_token");

            var sut = new UserController(_context , configurationMock.Object);



            //Act
            UserRegisterDto data = new UserRegisterDto()
            {
                Email = "test-email@gmail.com",
                Password = "Test password",
                Name = "Test"

            };
            var result = await sut.UserRegister(data);



            //Assert
            result.GetType().Should().Be(typeof(OkResult));

        }

        [Fact]
        public async Task UserRegisterAsync_ShouldReturn409Status()
        {
            //Arrange

            //database mock
            _context.Users.AddRange(UserMockData.GetSampleUserItems());
            _context.SaveChanges();

            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(x => x["Authentication:SecretKey"]).Returns("sample_password_for_token");

            var sut = new UserController(_context, configurationMock.Object);



            //Act
            UserRegisterDto data = new UserRegisterDto()
            {
                Email = "poorna@gmail.com",
                Password = "Test password",
                Name = "Test"

            };
            var result = await sut.UserRegister(data);



            //Assert
            result.GetType().Should().Be(typeof(ConflictResult));

        }

        [Fact]
        public  void UserLoginAsync_ShouldReturn200Status()
        {
            //Arrange

            //database mock
            _context.Users.AddRange(UserMockData.GetSampleUserItems());
            _context.SaveChanges();

            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(x => x["Authentication:SecretKey"]).Returns("sample_password_for_tokenkey_98&^3_34@^5dfr");
            configurationMock.Setup(x => x["Authentication:Issuer"]).Returns("sample_issuer");
            configurationMock.Setup(x => x["Authentication:Audience"]).Returns("sample_audience");

            var sut = new UserController(_context, configurationMock.Object);



            //Act
            LoginDto data = new LoginDto()
            {
                Email = "poorna@gmail.com",
                Password = "Poorna@123",

            };
            var result =  sut.UserLogin(data);



            //Assert
            result.GetType().Should().Be(typeof(OkObjectResult));

        }

        [Fact]
        public void UserLoginAsync_ShouldReturn404Status()
        {
            //Arrange

            //database mock
            _context.Users.AddRange(UserMockData.GetSampleUserItems());
            _context.SaveChanges();

            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(x => x["Authentication:SecretKey"]).Returns("sample_password_for_token");
            configurationMock.Setup(x => x["Authentication:Issuer"]).Returns("sample_issuer");
            configurationMock.Setup(x => x["Authentication:Audience"]).Returns("sample_audience");

            var sut = new UserController(_context, configurationMock.Object);



            //Act
            LoginDto data = new LoginDto()
            {
                Email = "non-valid-email@gmail.com",
                Password = "Poorna@123",

            };
            var result = sut.UserLogin(data);



            //Assert
            result.GetType().Should().Be(typeof(NotFoundResult));

        }

        [Fact]
        public void UserLoginAsync_ShouldReturn401Status()
        {
            //Arrange

            //database mock
            _context.Users.AddRange(UserMockData.GetSampleUserItems());
            _context.SaveChanges();

            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(x => x["Authentication:SecretKey"]).Returns("sample_password_for_token");
            configurationMock.Setup(x => x["Authentication:Issuer"]).Returns("sample_issuer");
            configurationMock.Setup(x => x["Authentication:Audience"]).Returns("sample_audience");

            var sut = new UserController(_context, configurationMock.Object);



            //Act
            LoginDto data = new LoginDto()
            {
                Email = "poorna@gmail.com",
                Password = "incorrect-password",

            };
            var result = sut.UserLogin(data);



            //Assert
            result.GetType().Should().Be(typeof(UnauthorizedResult));

        }




    }
}
