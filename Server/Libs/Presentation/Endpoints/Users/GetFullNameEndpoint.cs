using System.Security.Claims;
using Application.Services;
using Domain;
using Domain.Exceptions;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Presentation.Extensions;

namespace Presentation.Endpoints.Users;

public record GetFullNameRequest([property: RouteParam] int Id);

public class GetFullNameValidator : Validator<GetFullNameRequest>
{
    public GetFullNameValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Id is required");
    }
}

public class GetFullNameEndpoint(ILogger<GetFullNameEndpoint> logger, IUserService userService)
    : Endpoint<GetFullNameRequest, string>
{
    public override void Configure()
    {
        Get("/user/{Id}/fullname");
        Roles(Constants.Roles.User);
        Description(d =>
            d.Produces200<string>().Produces400<ErrorResponse>().Produces403().Produces404()
        );
    }

    public override async Task HandleAsync(GetFullNameRequest req, CancellationToken ct)
    {
        var requestingUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "-1");
        try
        {
            if (requestingUserId != req.Id)
            {
                await Send.ForbiddenAsync(ct);
                return;
            }

            var user = await userService.GetById(req.Id);
            await Send.OkAsync(user.GetFullName(), ct);
        }
        catch (Exception ex)
        {
            if (ex is EntityNotFoundException)
            {
                await Send.NotFoundAsync(ct);
                return;
            }

            logger.LogError(ex, "An error occurred while getting user by id for id {Id}", req.Id);
            await Send.StringAsync(
                string.Empty,
                StatusCodes.Status500InternalServerError,
                cancellation: ct
            );
        }
    }
}
