using Domain.Models;

namespace Domain.UnitTests.Models;

public class UserTests
{
    [Test]
    [Arguments("John", "Doe", "John Doe")]
    [Arguments("John", null, "John")]
    [Arguments(null, "Doe", "Doe")]
    [Arguments(null, null)]
    [Arguments("", null)]
    [Arguments(null, "")]
    [Arguments("", "")]
    [Arguments("   ", null)]
    [Arguments(null, "   ")]
    [Arguments("   ", "   ")]
    public async Task GetFullName_ShouldReturnUsername_WhenFirstAndLastNameAreNotUsable(
        string? firstName,
        string? lastName,
        string expectedFullName = "johndoe"
    )
    {
        // Arrange
        var user = new User
        {
            Username = "johndoe",
            Password = "password",
            FirstName = firstName,
            LastName = lastName,
        };
        // Act
        var fullName = user.GetFullName();
        // Assert
        await Assert.That(fullName).IsEqualTo(expectedFullName);
    }
}
