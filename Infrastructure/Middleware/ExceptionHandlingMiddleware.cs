using System.Text.Json;
using HospitalAPI.Infrastructure.Exceptions;

namespace HospitalAPI.Infrastructure.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (CustomHttpException exception)
        {
            await WriteErrorResponseAsync(context, exception.StatusCode, exception.Message);
        }
        catch (Exception)
        {
            await WriteErrorResponseAsync(context, StatusCodes.Status500InternalServerError, "Ocurrio un error interno del servidor.");
        }
    }

    private static async Task WriteErrorResponseAsync(
        HttpContext context,
        int statusCode,
        string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(new { message }));
    }
}
