namespace SimpleCloudflareBypass.Models;

public class GetHtmlRequest
{
    public string Url { get; set; } = null!;
    public int Timeout { get; set; }
    public int RebootsCount { get; set; }

    public GetHtmlRequest(string url, int timeout = 30, int rebootsCount = 2)
    {
        Url = url;
        Timeout = timeout;
        RebootsCount = rebootsCount;
    }
}
