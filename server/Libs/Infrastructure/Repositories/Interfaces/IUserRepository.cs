using Domain.Models;

namespace Infrastructure.Repositories.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAll();
    Task<User> GetById(int id);
    Task<User> GetByUsername(string name);
    Task<User> Create(User user);
    Task Update(User user);
    Task UpdatePassword(int id, string password);
    Task Delete(int id);
}
