using Domain.Utilities;
using FastEndpoints;
using FluentValidation;
using Presentation.Extensions;

namespace Presentation.Endpoints.Auth;

public record HashEndpointRequest(string Password);

public record HashEndpointResponse(string HashedPassword);

public class HashEndpointValidator : Validator<HashEndpointRequest>
{
    public HashEndpointValidator()
    {
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
    }
}

public class HashEndpoint() : Endpoint<HashEndpointRequest, HashEndpointResponse>
{
    public override void Configure()
    {
        Post("/auth/hash");
        AllowAnonymous();
        Description(d => d.Produces200<HashEndpointResponse>().Produces400<ErrorResponse>());
    }

    public override async Task HandleAsync(HashEndpointRequest req, CancellationToken ct)
    {
        var hashedPassword = HashingUtility.HashPassword(req.Password);
        var result = new HashEndpointResponse(hashedPassword);
        await Send.OkAsync(result, ct);
    }
}
