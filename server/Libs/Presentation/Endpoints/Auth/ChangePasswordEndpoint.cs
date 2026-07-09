using System.Net;
using System.Security.Claims;
using Application.Services;
using Domain;
using Domain.Models;
using FastEndpoints;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Presentation.Extensions;

namespace Presentation.Endpoints.Auth;

public record ChangePasswordEndpointRequest(string OldPassword, string NewPassword);

public class ChangePasswordEndpointValidator : Validator<ChangePasswordEndpointRequest>
{
    public ChangePasswordEndpointValidator()
    {
        RuleFor(x => x.OldPassword).NotEmpty().WithMessage("OldPassword is required");
        RuleFor(x => x.NewPassword).NotEmpty().WithMessage("NewPassword is required");
    }
}

public class ChangePasswordEndpoint(
    IUserService userService,
    ILogger<ChangePasswordEndpoint> logger
) : Endpoint<ChangePasswordEndpointRequest, EmptyResponse>
{
    public override void Configure()
    {
        Post("/auth/changePassword");
        Roles(Constants.Roles.User);
        Description(d => d.Produces200<EmptyResponse>().Produces400<ErrorResponse>());
    }

    public override async Task HandleAsync(ChangePasswordEndpointRequest req, CancellationToken ct)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "-1");
            if (userId == -1)
            {
                await Send.UnauthorizedAsync(ct);
                return;
            }
            var passwordChange = new PasswordChange(req.OldPassword, req.NewPassword);
            await userService.ChangePassword(userId, passwordChange);
            await Send.OkAsync(ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while changing the password");
            await Send.StringAsync(ex.Message, (int)HttpStatusCode.Unauthorized, cancellation: ct);
        }
    }
}
