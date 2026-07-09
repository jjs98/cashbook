using System.Net;
using Application.Services;
using Domain.Models;
using FastEndpoints;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Presentation.Extensions;

namespace Presentation.Endpoints.Auth;

public record LoginEndpointRequest(string Username, string Password);

public record LoginEndpointResponse(string? Token, string? Refresh);

public class LoginEndpointValidator : Validator<LoginEndpointRequest>
{
    public LoginEndpointValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
    }
}

public class LoginEndpoint(IAuthService authService, ILogger<LoginEndpoint> logger)
    : Endpoint<LoginEndpointRequest, LoginEndpointResponse>
{
    public override void Configure()
    {
        Post("/auth/login");
        AllowAnonymous();
        Description(d => d.Produces200<LoginEndpointResponse>().Produces400<ErrorResponse>());
    }

    public override async Task HandleAsync(LoginEndpointRequest req, CancellationToken ct)
    {
        try
        {
            var loginRequest = new LoginRequest(req.Username, req.Password);
            var authToken = await authService.Login(loginRequest);
            var response = new LoginEndpointResponse(authToken.Token, authToken.Refresh);
            await Send.OkAsync(response, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while logging in");
            await Send.StringAsync(ex.Message, (int)HttpStatusCode.Unauthorized, cancellation: ct);
        }
    }
}
