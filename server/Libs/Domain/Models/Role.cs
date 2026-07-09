namespace Domain.Models;

public class Role
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public IEnumerable<UserRole>? UserRoles { get; set; }
}
