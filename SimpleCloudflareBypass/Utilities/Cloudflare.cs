namespace SimpleCloudflareBypass.Utilities;

public static class Cloudflare
{
    public static IReadOnlyList<string> ChallengeSelectors = new List<string>()
    {
        "#challenge-stage", "#challenge-running", "#challenge-spinner",
        "#cf-challenge-running", "#cf-please-wait",  "#trk_jschal_js",
        "#title", "#description", "#link-ddg"
    };

    public static IReadOnlyList<string> AccessDeniedSelectors = new List<string>()
    {
        ".cf-main-wrapper",
        ".cf-header cf-section",
        ".cf-error-title",
        ".cf-code-label",
        ".cf-code-label",
        ".cf-error-description",
        ".cf-error-details"
    };

    public static IReadOnlyList<string> ChallengeTitles = new List<string>()
    {       
        "Just a moment",
        "Один момент",
        "DDOS-GUARD",
        "DDoS-Guard",
    };

    public static IReadOnlyList<string> AccessDeniedTitles = new List<string>()
    {
        "Access denied",
        "Attention Required! | Cloudflare"
    };

    public static IReadOnlyList<string> ChallengeDescriptions = new List<string>()
    {
        "Проверка браузера перед переходом на сайт",
        "Проверка безопасности подключения к сайту",
        "Checking if the site connection is secure"
    };
}
