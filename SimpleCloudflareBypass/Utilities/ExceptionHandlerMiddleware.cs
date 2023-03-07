using Newtonsoft.Json;
using System.Net;

namespace SimpleCloudflareBypass.Utilities;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {           
            Console.WriteLine($"In endpoint was throwed exception, error = {ex.Message}");
            await WriteToResponseAsync(context, HttpStatusCode.UnprocessableEntity, ex.Message);
        }
    }

    private async Task WriteToResponseAsync(HttpContext context, HttpStatusCode httpStatusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)httpStatusCode;
        await context.Response.WriteAsync(JsonConvert.SerializeObject(new { message }));
    }
}
