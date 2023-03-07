using SimpleCloudflareBypass.Models;
using SimpleCloudflareBypass.Utilities;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ChromeDriverFactory>();

var app = builder.Build();

app.MapPost("/getHtml", SimpleCloudflareBypass.Controllers.Controller.GetHtmlAsync)
   .Accepts<GetHtmlRequest>("application/json")
   .AddEndpointFilter<EndpointFilter>()
   .AllowAnonymous();


app.UseMiddleware<ExceptionHandlerMiddleware>();

app.Run();
