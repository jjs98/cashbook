using Domain.Utilities;

namespace Domain.UnitTests.Utilities;

public class HashingUtilityTests
{
    [Test]
    public async Task HashPassword_ShouldReturnHashedPassword()
    {
        // Arrange
        var password = "mysecretpassword";
        // Act
        var hashedPassword = HashingUtility.HashPassword(password);
        // assert
        await Assert.That(hashedPassword).IsNotNull();
        await Assert.That(hashedPassword).IsNotEqualTo(password);
    }

    [Test]
    public async Task HashPassword_ShouldReturnDifferentHashesForSamePassword()
    {
        // Arrange
        var password = "mysecretpassword";
        // Act
        var hashedPassword1 = HashingUtility.HashPassword(password);
        var hashedPassword2 = HashingUtility.HashPassword(password);
        // Assert
        await Assert.That(hashedPassword1).IsNotEqualTo(hashedPassword2);
    }

    [Test]
    public async Task VerifyPassword_ShouldReturnTrueForCorrectPassword()
    {
        // Arrange
        var password = "mysecret";
        var hashedPassword = HashingUtility.HashPassword(password);
        // Act
        var result = HashingUtility.VerifyPassword(password, hashedPassword);
        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task VerifyPassword_ShouldReturnFalseForIncorrectPassword()
    {
        // Arrange
        var password = "mysecret";
        var incorrectPassword = "wrongpassword";
        var hashedPassword = HashingUtility.HashPassword(password);
        // Act
        var result = HashingUtility.VerifyPassword(incorrectPassword, hashedPassword);
        // Assert
        await Assert.That(result).IsFalse();
    }
}
