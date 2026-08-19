namespace Domain.Models;

public class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public IEnumerable<UserRole>? UserRoles { get; set; }

    public string GetFullName()
    {
        if (string.IsNullOrWhiteSpace(FirstName) && string.IsNullOrWhiteSpace(LastName))
            return Username;

        if (string.IsNullOrWhiteSpace(FirstName))
            return LastName!;

        if (string.IsNullOrWhiteSpace(LastName))
            return FirstName;

        return $"{FirstName} {LastName}";
    }
}
