using Domain.Models;
using Infrastructure.Repositories.Interfaces;
using InterfaceGenerator;

namespace Application.Services;

[GenerateAutoInterface]
public class UserRoleService(IUserRoleRepository userRoleRepository) : IUserRoleService
{
    public async Task<IEnumerable<UserRole>> GetByUserId(int userId)
    {
        return await userRoleRepository.GetByUserId(userId);
    }

    public async Task<IEnumerable<UserRole>> GetByRoleId(int roleId)
    {
        return await userRoleRepository.GetByRoleId(roleId);
    }

    public async Task<UserRole> Create(UserRole userRole)
    {
        return await userRoleRepository.Create(userRole);
    }

    public async Task Delete(UserRole userRole)
    {
        await userRoleRepository.Delete(userRole);
    }
}
