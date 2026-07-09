using Domain.Models;

namespace Infrastructure.Repositories.Interfaces;

public interface IRoleRepository
{
    Task<IEnumerable<Role>> GetAll();
    Task<Role> GetByName(string name);
    Task<Role> GetById(int id);
    Task<IEnumerable<Role>> GetByIds(IEnumerable<int> id);
    Task<Role> Create(Role role);
    Task Update(Role role);
    Task Delete(int id);
}
