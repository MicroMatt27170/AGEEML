using System.Net;
using System.Text.Json;

namespace Ageeml.Service.Middleware;

public sealed class GlobalExceptionMiddleware(RequestDelegate next)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            context.Response.Clear();
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var payload = new ErrorResponse(
                Error: "internal_server_error",
                Title: "Unexpected error",
                Message: ex.Message,
                TraceId: context.TraceIdentifier
            );

            await context.Response.WriteAsync(JsonSerializer.Serialize(payload, JsonOptions));
        }
    }

    private sealed record ErrorResponse(string Error, string Title, string Message, string TraceId);
}
