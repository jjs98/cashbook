using System.Net.Mime;
using Domain.Exceptions;
using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace Presentation;

public class ErrorHandlingFilter : IGlobalPostProcessor
{
    public async Task PostProcessAsync(IPostProcessorContext ctx, CancellationToken ct)
    {
        if (ctx.ExceptionDispatchInfo?.SourceException is null)
            return;

        ctx.HttpContext.Response.ContentType = MediaTypeNames.Text.Plain;
        ctx.HttpContext.Response.StatusCode = ctx.ExceptionDispatchInfo?.SourceException switch
        {
            EntityNotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError,
        };

        await ctx.HttpContext.Response.WriteAsync(
            ctx.ExceptionDispatchInfo?.SourceException.Message ?? "There was an error!",
            ct
        );

        // Exception als handled markieren
        ctx.MarkExceptionAsHandled();
    }
}
