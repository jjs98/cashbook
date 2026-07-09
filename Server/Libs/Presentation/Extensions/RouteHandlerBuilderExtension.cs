using System.Net.Mime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Presentation.Extensions;

public static class RouteHandlerBuilderExtension
{
    public static RouteHandlerBuilder Produces<T>(this RouteHandlerBuilder builder, int statusCode)
    {
        builder.Produces<T>(statusCode, MediaTypeNames.Application.Json);
        return builder;
    }

    public static RouteHandlerBuilder Produces200<T>(this RouteHandlerBuilder builder)
    {
        builder.Produces<T>(StatusCodes.Status200OK, MediaTypeNames.Application.Json);
        return builder;
    }

    public static RouteHandlerBuilder Produces201<T>(this RouteHandlerBuilder builder)
    {
        builder.Produces<T>(StatusCodes.Status201Created, MediaTypeNames.Application.Json);
        return builder;
    }

    public static RouteHandlerBuilder Produces204(this RouteHandlerBuilder builder)
    {
        builder.Produces(StatusCodes.Status204NoContent);
        return builder;
    }

    public static RouteHandlerBuilder Produces400(this RouteHandlerBuilder builder)
    {
        builder.Produces<string>(StatusCodes.Status400BadRequest, MediaTypeNames.Text.Plain);
        return builder;
    }

    public static RouteHandlerBuilder Produces400<T>(this RouteHandlerBuilder builder)
    {
        builder.Produces<T>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json);
        return builder;
    }

    public static RouteHandlerBuilder Produces401(this RouteHandlerBuilder builder)
    {
        builder.Produces<string>(StatusCodes.Status401Unauthorized, MediaTypeNames.Text.Plain);
        return builder;
    }

    public static RouteHandlerBuilder Produces403(this RouteHandlerBuilder builder)
    {
        builder.Produces<string>(StatusCodes.Status403Forbidden, MediaTypeNames.Text.Plain);
        return builder;
    }

    public static RouteHandlerBuilder Produces404(this RouteHandlerBuilder builder)
    {
        builder.Produces<string>(StatusCodes.Status404NotFound, MediaTypeNames.Text.Plain);
        return builder;
    }

    public static RouteHandlerBuilder Produces500(this RouteHandlerBuilder builder)
    {
        builder.Produces<string>(
            StatusCodes.Status500InternalServerError,
            MediaTypeNames.Text.Plain
        );
        return builder;
    }
}
