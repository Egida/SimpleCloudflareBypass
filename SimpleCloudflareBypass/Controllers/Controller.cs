using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium;
using SimpleCloudflareBypass.Models;
using SimpleCloudflareBypass.Utilities;

namespace SimpleCloudflareBypass.Controllers;

public static class Controller
{
    private static SemaphoreSlim _semaphoreSlim = new(1);

    public static async Task<string> GetHtmlAsync([FromBody] GetHtmlRequest request, [FromServices] ChromeDriverFactory chromeDriverFactory, CancellationToken cancellationToken)
    {
        return await TryGetPageSourceAsync(request, chromeDriverFactory, cancellationToken);
    }

    private static async ValueTask<string> TryGetPageSourceAsync(GetHtmlRequest request, ChromeDriverFactory chromeDriverFactory, CancellationToken cancellationToken)
    {
        try
        {
            await _semaphoreSlim.WaitAsync(cancellationToken);
            int rebootsCounter = 0;
            do
            {
                try
                {
                    return await GetPageSourceAsync(request, chromeDriverFactory, cancellationToken);
                }
                catch (WebDriverException ex)
                {
                    Console.WriteLine(ex.Message);
                    chromeDriverFactory.Reboot();
                    rebootsCounter++;
                }
            } while (rebootsCounter < request.RebootsCount && cancellationToken.IsCancellationRequested is false);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
        throw new Exception("Can't resolve the cloudflare challenges.");
    }

    private static async ValueTask<string> GetPageSourceAsync(GetHtmlRequest request, ChromeDriverFactory chromeDriverFactory, CancellationToken cancellationToken)
    {
        Console.WriteLine($"{DateTime.Now}: Start resolve the challenges, url({request.Url}).");
        IWebDriver webDriver = chromeDriverFactory.CreateIfCalledReboot();
        webDriver.Url = request.Url;
        await WaitUntilSolvingChallengeAsync(webDriver, request.Timeout, cancellationToken);
        Console.WriteLine($"{DateTime.Now}: Finish resolve the challenges, url({request.Url}).");
        return webDriver.PageSource;
    }

    private static async ValueTask WaitUntilSolvingChallengeAsync(IWebDriver webDriver, int timeout, CancellationToken cancellationToken)
    {
        await Awaiter.WaitAsync(webDriver, driver =>
        {
            string pageTitle = webDriver.Title;
            IEnumerable<IWebElement>? webElements = null;
            if (Cloudflare.ChallengeTitles.Any(title => pageTitle.Contains(title)))
            {
                for (int i = 0; i < Cloudflare.ChallengeSelectors.Count && (webElements?.Any() ?? false) is false; i++)
                    webElements = driver.FindElements(By.CssSelector(Cloudflare.ChallengeSelectors[i]));

                if (webElements is not null && webElements.Any())
                    return false;
            }
            else if (Cloudflare.AccessDeniedTitles.Any(title => pageTitle.Contains(title)))
            {
                for (int i = 0; i < Cloudflare.AccessDeniedSelectors.Count && (webElements?.Any() ?? false) is false; i++)
                    webElements = driver.FindElements(By.CssSelector(Cloudflare.AccessDeniedSelectors[i]));

                if (webElements is not null && webElements.Any())
                    throw new Exception("Site returns access denied, maybe your ip is banned by cloudflare, check this site in browser.");
            }
            return true;
        }, timeout, cancellationToken);
    }
}
