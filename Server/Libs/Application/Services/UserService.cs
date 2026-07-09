using Domain.Models;
using Domain.Utilities;
using Infrastructure.Repositories.Interfaces;
using InterfaceGenerator;

namespace Application.Services;

[GenerateAutoInterface]
public class UserService(IUserRepository userRepository, IAuthService authService) : IUserService
{
    public async Task<IEnumerable<User>> GetAll()
    {
        return await userRepository.GetAll();
    }

    public async Task<User> GetById(int id)
    {
        return await userRepository.GetById(id);
    }

    public async Task<User> GetByUsername(string username)
    {
        return await userRepository.GetByUsername(username);
    }

    public async Task<User> Create(User user)
    {
        return await userRepository.Create(user);
    }

    public async Task Update(User user)
    {
        await userRepository.Update(user);
    }

    public async Task ChangePassword(int id, PasswordChange passwordChange)
    {
        var user = await userRepository.GetById(id);
        if (user == null || !BCrypt.Net.BCrypt.Verify(passwordChange.OldPassword, user.Password))
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }
        var hashedPassword = HashingUtility.HashPassword(passwordChange.NewPassword);
        await userRepository.UpdatePassword(id, hashedPassword);
    }

    public async Task Delete(int id)
    {
        await userRepository.Delete(id);
    }
}
