using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SimpleCloudflareBypass.Utilities;

public static class Awaiter
{ 
    public static async Task WaitAsync<TValue>(TValue value, Func<TValue, bool> action, int timeout, CancellationToken cancellationToken)
    {
        IClock clock = new SystemClock();
        TimeSpan delay = TimeSpan.FromMicroseconds(200);
        DateTime whenNeedStop = clock.LaterBy(TimeSpan.FromSeconds(timeout));
        bool result = false;
        while (result is false && cancellationToken.IsCancellationRequested is false)
        {
            if (clock.IsNowBefore(whenNeedStop) is false)
                throw new WebDriverTimeoutException($"Timed out after {timeout} seconds");
            try
            {
                result = action(value);
            }
            catch(WebDriverException)
            { 
            }           
            await Task.Delay(delay, cancellationToken);
        }
    }
}
