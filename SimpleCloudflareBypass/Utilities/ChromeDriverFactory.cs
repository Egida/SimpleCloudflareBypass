using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.DevTools.V110.Page;

namespace SimpleCloudflareBypass.Utilities;

public class ChromeDriverFactory : IDisposable
{
    private bool _isRebootCalled = false;
    private ChromeDriver? _chromeDriver = null;

    public ChromeDriver CreateIfCalledReboot()
    {
        if (_isRebootCalled)
        {
            _isRebootCalled = false;
            if (_chromeDriver!.HasActiveDevToolsSession is true)
                _chromeDriver!.GetDevToolsSession().Dispose();
            _chromeDriver?.Dispose();
            _chromeDriver = null;
        }

        if (_chromeDriver is null)
            _chromeDriver = Create();
        return _chromeDriver;
    }

    public ChromeDriver Create()
    {
        ChromeOptions options = new();
        options.AddArgument("start-maximized");
        options.AddArgument("disable-infobars");
        options.AddArgument("--disable-extensions");
        options.AddArgument("--disable-blink-features=AutomationControlled");
        options.AddArgument("--no-sandbox");
        //options.AddArgument("--headless");
        options.AddArgument("--disable-gpu");
        options.AddArgument("--disable-software-rasterizer");
        options.AddAdditionalOption("useAutomationExtension", false);
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36");
        TimeSpan commandTimeout = TimeSpan.FromSeconds(5);
        ChromeDriver chromeDriver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), options, commandTimeout);
        DevToolsSession session = chromeDriver.GetDevToolsSession();
        session.CommandTimeout = commandTimeout;
        chromeDriver.Manage().Timeouts().PageLoad = commandTimeout;
        var domains = session.GetVersionSpecificDomains<OpenQA.Selenium.DevTools.V110.DevToolsSessionDomains>();
        domains.Page.Enable(new EnableCommandSettings());
        domains.Page.AddScriptToEvaluateOnNewDocument(new AddScriptToEvaluateOnNewDocumentCommandSettings()
        {
            Source = "delete window.cdc_adoQpoasnfa76pfcZLmcfl_Array;" +
                     "delete window.cdc_adoQpoasnfa76pfcZLmcfl_Promise;" +
                     "delete window.cdc_adoQpoasnfa76pfcZLmcfl_Symbol;"
        });
        return chromeDriver;
    }

    public void Reboot()
    {
        Console.WriteLine($"{DateTime.Now}: Rebooting the chrome driver.");
        _isRebootCalled = true;
    }

    public void Dispose()
    {
        if (_chromeDriver is not null)
        {
            if(_chromeDriver.HasActiveDevToolsSession is true)
                _chromeDriver!.GetDevToolsSession().Dispose();
            _chromeDriver.Dispose();
        }        
    }
}
