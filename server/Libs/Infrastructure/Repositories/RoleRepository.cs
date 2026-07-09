using Domain.Exceptions;
using Domain.Models;
using Infrastructure.Entities;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class RoleRepository(IDbContextFactory<AppDbContext> contextFactory) : IRoleRepository
{
    public async Task<IEnumerable<Role>> GetAll()
    {
        using var context = contextFactory.CreateDbContext();
        var roles = await context.Roles.AsNoTracking().ToListAsync();
        return roles.Select(x => x.ToDomainModel());
    }

    public async Task<Role> GetByName(string name)
    {
        using var context = contextFactory.CreateDbContext();
        var role = await context.Roles.AsNoTracking().FirstOrDefaultAsync(x => x.Name == name);

        return role?.ToDomainModel()
            ?? throw new EntityNotFoundException($"Role with name {name} not found");
    }

    public async Task<Role> GetById(int id)
    {
        using var context = contextFactory.CreateDbContext();
        var role = await context.Roles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return role?.ToDomainModel()
            ?? throw new EntityNotFoundException($"Role with id {id} not found");
    }

    public async Task<IEnumerable<Role>> GetByIds(IEnumerable<int> ids)
    {
        using var context = contextFactory.CreateDbContext();
        var roles = await context.Roles.AsNoTracking().Where(x => ids.Contains(x.Id)).ToListAsync();
        return roles.Select(x => x.ToDomainModel());
    }

    public async Task<Role> Create(Role role)
    {
        using var context = contextFactory.CreateDbContext();
        var roleEntity = RoleEntity.FromDomainModel(role);
        var result = await context.Roles.AddAsync(roleEntity);
        await context.SaveChangesAsync();
        return result.Entity.ToDomainModel();
    }

    public async Task Update(Role role)
    {
        using var context = contextFactory.CreateDbContext();
        var existingRole = await GetByIdWithTracking(role.Id);

        await context
            .Roles.Where(x => x.Id == role.Id)
            .ExecuteUpdateAsync(x => x.SetProperty(r => r.Name, role.Name));

        context.ChangeTracker.Clear();
    }

    public async Task Delete(int id)
    {
        using var context = contextFactory.CreateDbContext();
        var existingRole = await GetByIdWithTracking(id);

        context.Roles.Remove(existingRole);
        await context.SaveChangesAsync();
    }

    private async Task<RoleEntity> GetByIdWithTracking(int id)
    {
        using var context = contextFactory.CreateDbContext();
        var role = await context.Roles.FindAsync(id);

        return role ?? throw new EntityNotFoundException($"Role with id {id} not found");
    }
}
