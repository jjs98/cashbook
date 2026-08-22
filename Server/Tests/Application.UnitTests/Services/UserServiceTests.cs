using Application.Services;
using Domain.Models;
using Domain.Utilities;
using Infrastructure.Repositories.Interfaces;

namespace Application.UnitTests.Services;

public class UserServiceTests
{
    private readonly UserService _sut;
    private readonly Mock<IUserRepository> _userRepository = IUserRepository.Mock();

    public UserServiceTests()
    {
        _sut = new UserService(_userRepository.Object);
    }

    [Test]
    public async Task GetAll_ShouldReturnAllUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new()
            {
                Id = 1,
                Username = "user1",
                Password = "hash1",
                Email = "user1@test.com",
            },
            new()
            {
                Id = 2,
                Username = "user2",
                Password = "hash2",
                Email = "user2@test.com",
            },
        };
        _userRepository.GetAll().Returns(users);

        // Act
        var result = await _sut.GetAll();

        // Assert
        _userRepository.GetAll().WasCalled(Times.Once);
        await Assert.That(result).IsEquivalentTo(users);
    }

    [Test]
    public async Task GetById_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Password = "hash",
            Email = "test@test.com",
        };
        _userRepository.GetById(1).Returns(user);

        // Act
        var result = await _sut.GetById(1);

        // Assert
        _userRepository.GetById(1).WasCalled(Times.Once);
        await Assert.That(result).IsEqualTo(user);
    }

    [Test]
    public async Task GetByUsername_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Password = "hash",
            Email = "test@test.com",
        };
        _userRepository.GetByUsername("testuser").Returns(user);

        // Act
        var result = await _sut.GetByUsername("testuser");

        // Assert
        _userRepository.GetByUsername("testuser").WasCalled(Times.Once);
        await Assert.That(result).IsEqualTo(user);
    }

    [Test]
    public async Task Create_ShouldReturnCreatedUser()
    {
        // Arrange
        var user = new User
        {
            Username = "newuser",
            Password = "hash",
            Email = "new@test.com",
        };
        var createdUser = new User
        {
            Id = 1,
            Username = "newuser",
            Password = "hash",
            Email = "new@test.com",
        };
        _userRepository.Create(user).Returns(createdUser);

        // Act
        var result = await _sut.Create(user);

        // Assert
        _userRepository.Create(user).WasCalled(Times.Once);
        await Assert.That(result).IsEqualTo(createdUser);
    }

    [Test]
    public async Task Update_ShouldCallRepositoryUpdate()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "updateduser",
            Password = "hash",
            Email = "updated@test.com",
        };
        _userRepository.Update(user).Returns(() => Task.CompletedTask);

        // Act
        await _sut.Update(user);

        // Assert
        _userRepository.Update(user).WasCalled(Times.Once);
    }

    [Test]
    public async Task Delete_ShouldCallRepositoryDelete()
    {
        // Arrange
        var userId = 1;
        _userRepository.Delete(userId).Returns(() => Task.CompletedTask);

        // Act
        await _sut.Delete(userId);

        // Assert
        _userRepository.Delete(userId).WasCalled(Times.Once);
    }
}
