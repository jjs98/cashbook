using Application.Services;
using Domain.Enums;
using Infrastructure.Repositories.Interfaces;

namespace Application.UnitTests.Services;

public class HealthServiceTests
{
    private readonly HealthService _sut;
    private readonly Mock<IHealthRepository> _healthRepository = IHealthRepository.Mock();

    public HealthServiceTests()
    {
        _sut = new HealthService(_healthRepository.Object);
    }

    [Test]
    public async Task GetDatabaseHealth_ShouldReturnHealthy_WhenDatabaseIsAccessible()
    {
        // Arrange
        _healthRepository.GetHealth().Returns(true);

        // Act
        var healthStatus = await _sut.GetDatabaseHealth();

        // Assert
        _healthRepository.GetHealth().WasCalled(Times.Once);
        await Assert.That(healthStatus.ModuleName).IsEqualTo("Database");
        await Assert.That(healthStatus.Status).IsEqualTo(HealthStatusEnum.Healthy.ToString());
    }

    [Test]
    public async Task GetDatabaseHealth_ShouldReturnUnhealthy_WhenDatabaseIsNotAccessible()
    {
        // Arrange
        _healthRepository.GetHealth().Returns(false);

        // Act
        var healthStatus = await _sut.GetDatabaseHealth();

        // Assert
        _healthRepository.GetHealth().WasCalled(Times.Once);
        await Assert.That(healthStatus.ModuleName).IsEqualTo("Database");
        await Assert.That(healthStatus.Status).IsEqualTo(HealthStatusEnum.Unhealthy.ToString());
    }

    [Test]
    public async Task GetDatabaseHealth_ShouldReturnUnhealthy_WhenExceptionIsThrown()
    {
        // Arrange
        _healthRepository.GetHealth().Throws(new Exception("Database connection error"));

        // Act
        var healthStatus = await _sut.GetDatabaseHealth();

        // Assert
        _healthRepository.GetHealth().WasCalled(Times.Once);
        await Assert.That(healthStatus.ModuleName).IsEqualTo("Database");
        await Assert.That(healthStatus.Status).IsEqualTo(HealthStatusEnum.Unhealthy.ToString());
    }

    [Test]
    public async Task GetApiHealth_ShouldReturnHealthy()
    {
        // Act
        var healthStatus = HealthService.GetApiHealth();

        // Assert
        await Assert.That(healthStatus.ModuleName).IsEqualTo("Api");
        await Assert.That(healthStatus.Status).IsEqualTo(HealthStatusEnum.Healthy.ToString());
    }
}
