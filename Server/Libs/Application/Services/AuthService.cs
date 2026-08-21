using Domain.Models;
using Domain.Utilities;
using Infrastructure.Repositories.Interfaces;
using InterfaceGenerator;

namespace Application.Services;

[GenerateAutoInterface]
public class AuthService(IUserRepository userRepository, ITokenService tokenService) : IAuthService
{
    public async Task<AuthToken> Login(LoginRequest loginRequest)
    {
        var user = await userRepository.GetByUsername(loginRequest.Username);
        if (!HashingUtility.VerifyPassword(loginRequest.Password, user.Password))
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        return await GenerateJwtToken(user);
    }

    public async Task<AuthToken> Refresh(User user)
    {
        return await GenerateJwtToken(user);
    }

    private async Task<AuthToken> GenerateJwtToken(User user)
    {
        var jwtToken = await tokenService.GenerateJwtToken(user);
        return new AuthToken(jwtToken, "");
    }
}
