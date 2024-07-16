namespace Gdn.Web.Fluentblazor.Extensions;

public static class HttpClientFactoryExtensions
{
    private const string CLIENT_NAME = "GdnWebApi";

    public static HttpClient CreateApiClient(this IHttpClientFactory httpClientFactory)
    {
        return httpClientFactory.CreateClient(CLIENT_NAME);
    }
}
