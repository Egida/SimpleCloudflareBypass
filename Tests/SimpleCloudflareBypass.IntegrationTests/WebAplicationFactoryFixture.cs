using Microsoft.AspNetCore.Mvc.Testing;

namespace SimpleCloudflareBypass.IntegrationTests;

public class WebAplicationFactoryFixture : IDisposable  
{
    private readonly WebApplicationFactory<Program> _webApplicationFactory;
    public HttpClient HttpClient { get; }

    public WebAplicationFactoryFixture()
    {
        _webApplicationFactory = new WebApplicationFactory<Program>();
        HttpClient = _webApplicationFactory.CreateDefaultClient();
    }

    public void Dispose()
    {
        HttpClient.Dispose();
        _webApplicationFactory.Dispose();
    }
}
