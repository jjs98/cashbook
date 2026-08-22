using Application.Services;
using Domain.Models;
using Domain.Utilities;
using Infrastructure.Repositories.Interfaces;

namespace Application.UnitTests.Services;

public class AuthServiceTests
{
    private readonly AuthService _sut;
    private readonly Mock<IUserRepository> _userRepository = IUserRepository.Mock();
    private readonly Mock<ITokenService> _tokenService = ITokenService.Mock();

    public AuthServiceTests()
    {
        _sut = new AuthService(_userRepository.Object, _tokenService.Object);
    }

    [Test]
    public async Task Login_ShouldReturnAuthToken_WhenUserExists()
    {
        // Arrange
        var loginRequest = new LoginRequest("testuser", "password");
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Password = HashingUtility.HashPassword("password"),
            Email = "",
        };
        _userRepository.GetByUsername(loginRequest.Username).Returns(user);
        _tokenService.GenerateJwtToken(user).Returns("awesometoken");

        //Act
        var authToken = await _sut.Login(loginRequest);

        // Assert
        await Assert.That(authToken).IsNotNull();
        await Assert.That(authToken.Token).IsNotNullOrEmpty();
        await Assert.That(authToken.Refresh).IsNotNull();
        _userRepository.GetByUsername(loginRequest.Username).WasCalled(Times.Once);
        _tokenService.GenerateJwtToken(user).WasCalled(Times.Once);
    }

    [Test]
    public async Task Login_ShouldThrowUnauthorizedAccessException_WhenPasswordIsIncorrect()
    {
        // Arrange
        var loginRequest = new LoginRequest("testuser", "wrongpassword");
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Password = HashingUtility.HashPassword("password"),
            Email = "",
        };
        _userRepository.GetByUsername(loginRequest.Username).Returns(user);
        _tokenService.GenerateJwtToken(user).Returns("awesometoken");

        //Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await _sut.Login(loginRequest)
        );
        _userRepository.GetByUsername(loginRequest.Username).WasCalled(Times.Once);
        _tokenService.GenerateJwtToken(user).WasCalled(Times.Never);
    }

    [Test]
    public async Task ChangePassword_ShouldUpdatePassword_WhenOldPasswordIsCorrect()
    {
        // Arrange
        var userId = 1;
        var oldPassword = "oldpassword";
        var newPassword = "newpassword";
        var passwordChange = new PasswordChange(oldPassword, newPassword);
        var user = new User
        {
            Id = userId,
            Username = "testuser",
            Password = HashingUtility.HashPassword(oldPassword),
            Email = "test@test.com",
        };
        _userRepository.GetById(userId).Returns(user);
        _userRepository.UpdatePassword(userId, Arg.Any<string>()).Returns(() => Task.CompletedTask);

        // Act
        await _sut.ChangePassword(userId, passwordChange);

        // Assert
        _userRepository.GetById(userId).WasCalled(Times.Once);
        _userRepository.UpdatePassword(userId, Arg.Any<string>()).WasCalled(Times.Once);
    }

    [Test]
    public async Task ChangePassword_ShouldThrowUnauthorizedAccessException_WhenOldPasswordIsIncorrect()
    {
        // Arrange
        var userId = 1;
        var oldPassword = "oldpassword";
        var wrongOldPassword = "wrongpassword";
        var newPassword = "newpassword";
        var passwordChange = new PasswordChange(wrongOldPassword, newPassword);
        var user = new User
        {
            Id = userId,
            Username = "testuser",
            Password = HashingUtility.HashPassword(oldPassword),
            Email = "test@test.com",
        };
        _userRepository.GetById(userId).Returns(user);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
            await _sut.ChangePassword(userId, passwordChange)
        );
        _userRepository.GetById(userId).WasCalled(Times.Once);
        _userRepository.UpdatePassword(userId, Arg.Any<string>()).WasCalled(Times.Never);
    }
}
