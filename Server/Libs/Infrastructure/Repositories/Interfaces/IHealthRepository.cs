namespace Infrastructure.Repositories.Interfaces;

public interface IHealthRepository
{
    Task<bool> GetHealth();
}
