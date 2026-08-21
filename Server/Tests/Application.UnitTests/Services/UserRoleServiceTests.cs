using Application.Services;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;

namespace Application.UnitTests.Services;

public class UserRoleServiceTests
{
    private readonly UserRoleService _sut;
    private readonly Mock<IUserRoleRepository> _userRoleRepository = IUserRoleRepository.Mock();

    public UserRoleServiceTests()
    {
        _sut = new UserRoleService(_userRoleRepository.Object);
    }

    [Test]
    public async Task GetByUserId_ShouldReturnUserRoles_WhenUserHasRoles()
    {
        // Arrange
        var userId = 1;
        var userRoles = new List<UserRole>
        {
            new() { UserId = userId, RoleId = 1 },
            new() { UserId = userId, RoleId = 2 }
        };
        _userRoleRepository.GetByUserId(userId).Returns(userRoles);

        // Act
        var result = await _sut.GetByUserId(userId);

        // Assert
        _userRoleRepository.GetByUserId(userId).WasCalled(Times.Once);
        await Assert.That(result).IsEquivalentTo(userRoles);
    }

    [Test]
    public async Task GetByUserId_ShouldReturnEmpty_WhenUserHasNoRoles()
    {
        // Arrange
        var userId = 1;
        var userRoles = new List<UserRole>();
        _userRoleRepository.GetByUserId(userId).Returns(userRoles);

        // Act
        var result = await _sut.GetByUserId(userId);

        // Assert
        _userRoleRepository.GetByUserId(userId).WasCalled(Times.Once);
        await Assert.That(result).IsEmpty();
    }

    [Test]
    public async Task GetByRoleId_ShouldReturnUserRoles_WhenRoleHasUsers()
    {
        // Arrange
        var roleId = 1;
        var userRoles = new List<UserRole>
        {
            new() { UserId = 1, RoleId = roleId },
            new() { UserId = 2, RoleId = roleId }
        };
        _userRoleRepository.GetByRoleId(roleId).Returns(userRoles);

        // Act
        var result = await _sut.GetByRoleId(roleId);

        // Assert
        _userRoleRepository.GetByRoleId(roleId).WasCalled(Times.Once);
        await Assert.That(result).IsEquivalentTo(userRoles);
    }

    [Test]
    public async Task GetByRoleId_ShouldReturnEmpty_WhenRoleHasNoUsers()
    {
        // Arrange
        var roleId = 1;
        var userRoles = new List<UserRole>();
        _userRoleRepository.GetByRoleId(roleId).Returns(userRoles);

        // Act
        var result = await _sut.GetByRoleId(roleId);

        // Assert
        _userRoleRepository.GetByRoleId(roleId).WasCalled(Times.Once);
        await Assert.That(result).IsEmpty();
    }

    [Test]
    public async Task Create_ShouldReturnCreatedUserRole()
    {
        // Arrange
        var userRole = new UserRole { UserId = 1, RoleId = 1 };
        _userRoleRepository.Create(userRole).Returns(userRole);

        // Act
        var result = await _sut.Create(userRole);

        // Assert
        _userRoleRepository.Create(userRole).WasCalled(Times.Once);
        await Assert.That(result).IsEqualTo(userRole);
    }

    [Test]
    public async Task Delete_ShouldCallRepositoryDelete()
    {
        // Arrange
        var userRole = new UserRole { UserId = 1, RoleId = 1 };
        _userRoleRepository.Delete(userRole).Returns(() => Task.CompletedTask);

        // Act
        await _sut.Delete(userRole);

        // Assert
        _userRoleRepository.Delete(userRole).WasCalled(Times.Once);
    }
}
