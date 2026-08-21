using Application.Services;
using Domain.Models;
using Infrastructure.Repositories.Interfaces;

namespace Application.UnitTests.Services;

public class RoleServiceTests
{
    private readonly RoleService _sut;
    private readonly Mock<IRoleRepository> _roleRepository = IRoleRepository.Mock();

    public RoleServiceTests()
    {
        _sut = new RoleService(_roleRepository.Object);
    }

    [Test]
    public async Task GetAll_ShouldReturnAllRoles()
    {
        // Arrange
        var roles = new List<Role>
        {
            new() { Id = 1, Name = "Admin" },
            new() { Id = 2, Name = "User" }
        };
        _roleRepository.GetAll().Returns(roles);

        // Act
        var result = await _sut.GetAll();

        // Assert
        _roleRepository.GetAll().WasCalled(Times.Once);
        await Assert.That(result).IsEquivalentTo(roles);
    }

    [Test]
    public async Task GetById_ShouldReturnRole_WhenRoleExists()
    {
        // Arrange
        var role = new Role { Id = 1, Name = "Admin" };
        _roleRepository.GetById(1).Returns(role);

        // Act
        var result = await _sut.GetById(1);

        // Assert
        _roleRepository.GetById(1).WasCalled(Times.Once);
        await Assert.That(result).IsEqualTo(role);
    }

    [Test]
    public async Task GetByIds_ShouldReturnRoles_WhenRolesExist()
    {
        // Arrange
        var ids = new List<int> { 1, 2 };
        var roles = new List<Role>
        {
            new() { Id = 1, Name = "Admin" },
            new() { Id = 2, Name = "User" }
        };
        _roleRepository.GetByIds(ids).Returns(roles);

        // Act
        var result = await _sut.GetByIds(ids);

        // Assert
        _roleRepository.GetByIds(ids).WasCalled(Times.Once);
        await Assert.That(result).IsEquivalentTo(roles);
    }

    [Test]
    public async Task GetByName_ShouldReturnRole_WhenRoleExists()
    {
        // Arrange
        var role = new Role { Id = 1, Name = "Admin" };
        _roleRepository.GetByName("Admin").Returns(role);

        // Act
        var result = await _sut.GetByName("Admin");

        // Assert
        _roleRepository.GetByName("Admin").WasCalled(Times.Once);
        await Assert.That(result).IsEqualTo(role);
    }

    [Test]
    public async Task Create_ShouldReturnCreatedRole()
    {
        // Arrange
        var role = new Role { Name = "Moderator" };
        var createdRole = new Role { Id = 3, Name = "Moderator" };
        _roleRepository.Create(role).Returns(createdRole);

        // Act
        var result = await _sut.Create(role);

        // Assert
        _roleRepository.Create(role).WasCalled(Times.Once);
        await Assert.That(result).IsEqualTo(createdRole);
    }

    [Test]
    public async Task Update_ShouldCallRepositoryUpdate()
    {
        // Arrange
        var role = new Role { Id = 1, Name = "UpdatedAdmin" };
        _roleRepository.Update(role).Returns(() => Task.CompletedTask);

        // Act
        await _sut.Update(role);

        // Assert
        _roleRepository.Update(role).WasCalled(Times.Once);
    }

    [Test]
    public async Task Delete_ShouldCallRepositoryDelete()
    {
        // Arrange
        var roleId = 1;
        _roleRepository.Delete(roleId).Returns(() => Task.CompletedTask);

        // Act
        await _sut.Delete(roleId);

        // Assert
        _roleRepository.Delete(roleId).WasCalled(Times.Once);
    }
}
