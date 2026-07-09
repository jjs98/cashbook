using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using InterfaceGenerator;

namespace Application.Services;

[GenerateAutoInterface]
public class RoleService(IRoleRepository roleRepository) : IRoleService
{
    public async Task<IEnumerable<Role>> GetAll()
    {
        return await roleRepository.GetAll();
    }

    public async Task<Role> GetById(int id)
    {
        return await roleRepository.GetById(id);
    }

    public async Task<IEnumerable<Role>> GetByIds(IEnumerable<int> ids)
    {
        return await roleRepository.GetByIds(ids);
    }

    public async Task<Role> GetByName(string name)
    {
        return await roleRepository.GetByName(name);
    }

    public async Task<Role> Create(Role role)
    {
        return await roleRepository.Create(role);
    }

    public async Task Update(Role role)
    {
        await roleRepository.Update(role);
    }

    public async Task Delete(int id)
    {
        await roleRepository.Delete(id);
    }
}
