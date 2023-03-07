using SimpleCloudflareBypass.Models;
using SimpleCloudflareBypass.Utilities;
using System.Net.Http.Json;

namespace SimpleCloudflareBypass.IntegrationTests;



public class ControllerTests : IClassFixture<WebAplicationFactoryFixture>
{
    private readonly HttpClient _httpClient;
    private const string _enpointName = "/getHtml";

    public ControllerTests(WebAplicationFactoryFixture webAplicationFactoryFixture)
    {
        _httpClient = webAplicationFactoryFixture.HttpClient;
    }

    public static IEnumerable<object[]> GetUrlsWithChallenge()
    {
        yield return new[] { "https://extratorrent.st/search/?srt=added&order=desc&search=720p&new=1&x=0&y=0" };
        yield return new[] { "https://torrentcore.xyz/index" };
        yield return new[] { "http://bitturk.net/" };
        yield return new[] { "https://nowsecure.nl" };
        yield return new[] { "https://www.muziekfabriek.org" };
        yield return new[] { "https://idope.se/torrent-list/harry/" };
        yield return new[] { "https://bt4g.org/search/2022" };
    }

    public static IEnumerable<object[]> GetUrlsWithAccessDenied()
    {
        yield return new[] { "https://cpasbiens3.fr/index.php?do=search&subaction=search" };
    }

    [Theory]
    [MemberData(nameof(GetUrlsWithChallenge))]
    public async Task GetHtml_ResolvingChallange_CorrectResponse(string url)
    {
        // Arrange
        HttpRequestMessage httpRequestMessage = CreateHttpRequestMessage(url);

        //Act
        HttpResponseMessage response = await _httpClient.SendAsync(httpRequestMessage);
        string responseHtml = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.True(response.IsSuccessStatusCode);
        CheckResponse(responseHtml);
    }

    [Theory]
    [MemberData(nameof(GetUrlsWithAccessDenied))]
    public async Task GetHtml_AccessDeniedPage_StatusCodeIsNotSuccess(string url)
    {
        // Arrange
        HttpRequestMessage httpRequestMessage = CreateHttpRequestMessage(url);
        //Act
        HttpResponseMessage response = await _httpClient.SendAsync(httpRequestMessage);
        //Assert
        Assert.False(response.IsSuccessStatusCode);
    }


    private void CheckResponse(string responseHtml)
    {
        Assert.All(Cloudflare.ChallengeSelectors, selector => Assert.DoesNotContain(selector, responseHtml));
        Assert.All(Cloudflare.ChallengeTitles, selector => Assert.DoesNotContain(selector, responseHtml));
        Assert.All(Cloudflare.ChallengeDescriptions, selector => Assert.DoesNotContain(selector, responseHtml));
        Assert.All(Cloudflare.AccessDeniedSelectors, selector => Assert.DoesNotContain(selector, responseHtml));
        Assert.All(Cloudflare.AccessDeniedTitles, selector => Assert.DoesNotContain(selector, responseHtml));
    }

    private HttpRequestMessage CreateHttpRequestMessage(string url)
    {
        GetHtmlRequest request = new(url);
        return new(HttpMethod.Post, _httpClient.BaseAddress!.OriginalString + _enpointName)
        {
            Content = JsonContent.Create(request)
        };
    }
}
