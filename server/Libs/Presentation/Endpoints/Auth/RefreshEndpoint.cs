using System.Net;
using System.Security.Claims;
using Application.Services;
using Domain;
using FastEndpoints;
using Presentation.Extensions;

namespace Presentation.Endpoints.Auth;

public record RefreshEndpointResponse(string? Token, string? Refresh);

public class RefreshEndpoint(IAuthService authService, IUserService userService)
    : Endpoint<EmptyRequest, RefreshEndpointResponse>
{
    public override void Configure()
    {
        Post("/auth/refresh");
        Roles(Constants.Roles.User);
        Description(d => d.Produces200<RefreshEndpointResponse>().Produces400());
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "-1");
            if (userId == -1)
            {
                await Send.UnauthorizedAsync(ct);
                return;
            }

            var user = await userService.GetById(userId);
            var token = await authService.Refresh(user);
            var response = new RefreshEndpointResponse(token.Token, token.Refresh);
            await Send.OkAsync(response, cancellation: ct);
        }
        catch (Exception ex)
        {
            await Send.StringAsync(
                ex.Message,
                statusCode: (int)HttpStatusCode.BadRequest,
                cancellation: ct
            );
        }
    }
}
