#  SimpleCloudflareBypass

## About

This is a simple way to bypass Cloudflare and DDoS-GUARD. Why simple? Because It starts the proxy server and keeps one exemplar of the chrome driver opened, as a result process doesn't consume a lot of resources, but all requests to the server are handled consecutively. The server will return the html page when challenges are solved.

To bypass Cloudflare is used the feature of rebooting the chrome driver, because challenges may not be solved the first time.

#### Why have I written this

There is one reason I could not find something which could bypass Cloudflare protection. I tried to use the most popular repositories on GitHub to solve the challenges, but they all failed, so I tried to solve them by myself, and I managed.

## Usage
By default port is 5000, you can easy to change it in appsettings.json. Endpoint has name 'getHtml'.

The POST request consists of three fields: url, which contains the needed html, timeout on solving the challenges, and a count of reboots the chrome driver, by default timeout is 30 seconds, and the count of reboots is 2.

    public string Url { get; set; }
    
    public int Timeout { get; set; }
    
    public int RebootsCount { get; set; }
 
 **Note** If you use it on linux you need to give access rights to chromedriver in the output directory. (`chmod a+x chromedriver`)
 And you can turn on the headless mode, cloudflare doesn't detected it. (SimpleCloudflareBypass/Utilities/ChromeDriverFactory 36 line)
