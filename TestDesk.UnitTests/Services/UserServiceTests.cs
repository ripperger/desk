using Desk.Constants;
using Desk.Contracts.Services;
using Desk.Data;
using Desk.Models;
using Desk.Services;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace TestDesk.UnitTests.Services
{
    public class UserServiceTests : IDisposable
    {
        private readonly DeskContext _context;
        //private readonly DbSet<User> _users;
        private readonly IUserService _userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserServiceTests"/> class.
        /// Sets up the test environment.
        /// </summary>
        public UserServiceTests()
        {
            var options = new DbContextOptionsBuilder<DeskContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabaseForUserService")
                .Options;
            _context = new DeskContext(options);
            _context.Database.EnsureDeleted(); // Clear the database
            _context.Database.EnsureCreated(); // Recreate the database

            //_context = Substitute.For<DeskContext>(options);
            //_users = Substitute.For<DbSet<User>>();

            //// Setup the mock context to return the mock set
            //_context.Users.Returns(_users);

            // Instantiate the service with the mock context
            _userService = new UserService(_context);
        }

        /// <summary>
        /// Tests the GetUserByIdAsync method.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUser()
        {
            // Arrange
            var user = new User { UserName = "testuser" };
            _context.Users.Add(user);
            _context.SaveChanges();
            //_users.FindAsync(user.Id).Returns(user);

            // Act
            var result = await _userService.GetUserByIdAsync(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
        }

        /// <summary>
        /// Tests the GetUserBySidAsync method.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetUserBySidAsync_ShouldReturnUser()
        {
            // Arrange
            var sid = "S-1-5-21-1234567890-123456789-1234567890-1234";
            var user = new User { Sid = sid, UserName = "testuser" };
            _context.Users.Add(user);
            _context.SaveChanges();

            // Act
            var result = await _userService.GetUserBySidAsync(sid);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(sid, result.Sid);
        }

        /// <summary>
        /// Tests the GetUserByUserNameAsync method.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetUserByUserNameAsync_ShouldReturnUser()
        {
            // Arrange
            var user = new User { UserName = "testuser" };
            _context.Users.Add(user);
            _context.SaveChanges();

            // Act
            var result = await _userService.GetUserByUserNameAsync("testuser");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result.UserName);
        }

        /// <summary>
        /// Tests the GetUserByUserName method.
        /// </summary>
        [Fact]
        public void GetUserByUserName_ShouldReturnUser()
        {
            // Arrange
            var user = new User { UserName = "testuser" };
            _context.Users.Add(user);
            _context.SaveChanges();

            // Act
            var result = _userService.GetUserByUserName("testuser");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result.UserName);
        }

        /// <summary>
        /// Tests the LoggedInUserName method.
        /// If the user is not developer, the user name is returned without the domain.
        /// </summary>
        [Fact]
        public void LoggedInUserName_ShouldReturnUserName()
        {
            // Arrange
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "domain\\testuser") };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            // Act
            var result = _userService.LoggedInUserName(claimsPrincipal);

            // Assert
            Assert.Equal("testuser", result);
        }

        /// <summary>
        /// Tests the LoggedInUserName method.
        /// If the user is developer, the developer user name is returned.
        /// </summary>
        [Fact]
        public void LoggedInUserName_ShouldReturnDevUserName()
        {
            // Arrange
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, Dictionaries.Role["DEV_USER"]) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            // Act
            var result = _userService.LoggedInUserName(claimsPrincipal);

            // Assert
            Assert.Equal(Dictionaries.Role["DEV_USER"], result);
        }

        /// <summary>
        /// Tests the IsDevUser(ClaimsPrincipal) method for a developer user.
        /// </summary>
        [Fact]
        public void IsDevUser_ShouldReturnTrueForDevUser()
        {
            // Arrange
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, Dictionaries.Role["DEV_USER"]) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            // Act
            var result = _userService.IsDevUser(claimsPrincipal);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Tests the IsDevUser(ClaimsPrincipal) method for a non developer user.
        /// </summary>
        [Fact]
        public void IsDevUser_ShouldReturnFalseForNonDevUser()
        {
            // Arrange
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "domain\\testuser") };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            // Act
            var result = _userService.IsDevUser(claimsPrincipal);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Tests the IsDevUser(string) method for a developer user.
        /// </summary>
        [Fact]
        public void IsDevUser_ShouldReturnTrueForDevUserName()
        {
            // Arrange
            var userName = Dictionaries.Role["DEV_USER"];

            // Act
            var result = _userService.IsDevUser(userName);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Tests the IsDevUser(string) method for a non developer user.
        /// </summary>
        [Fact]
        public void IsDevUser_ShouldReturnFalseForNonDevUserName()
        {
            // Arrange
            var userName = "testuser";

            // Act
            var result = _userService.IsDevUser(userName);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Tests the UpdateUserAsync method for UserName and FullName.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task UpdateUserAsync_ShouldUpdateUser()
        {
            // Arrange
            var existingUser = new User { UserName = "testuser" };
            var newUser = new User { UserName = "updateduser", FullName = "Updated User" };
            _context.Users.Add(existingUser);
            _context.SaveChanges();

            // Act
            var result = await _userService.UpdateUserAsync(existingUser, newUser);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("updateduser", result.UserName);
            Assert.Equal("Updated User", result.FullName);
        }

        /// <summary>
        /// Tests the RegisterTempUser method for UserName and FullName.
        /// </summary>
        [Fact]
        public void RegisterTempUser_ShouldReturnRegisteredUser()
        {
            // Act
            var result = _userService.RegisterTempUser("tempuser");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("tempuser", result.UserName);
            Assert.Equal("tempuser (nem azonosított)", result.FullName);
        }

        /// <summary>
        /// Tests the RegisterTempUser method for adding to the database.
        /// </summary>
        [Fact]
        public void RegisterTempUser_ShouldRegisterUser()
        {
            // Act
            _userService.RegisterTempUser("tempuser");
            var result = _context.Users.FirstOrDefault(u => u.UserName == "tempuser");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("tempuser", result.UserName);
            Assert.Equal("tempuser (nem azonosított)", result.FullName);
        }


        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
