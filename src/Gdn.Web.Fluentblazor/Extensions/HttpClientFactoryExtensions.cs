namespace Gdn.Web.Fluentblazor.Extensions;

public static class HttpClientFactoryExtensions
{
    private const string CLIENT_NAME = "GdnWebApi";
    private const string CLIENT_NAME_VS = "GdnWebApiVs";

    public static HttpClient CreateApiClient(this IHttpClientFactory httpClientFactory)
    {
        return httpClientFactory.CreateClient(CLIENT_NAME);
    }

    public static HttpClient CreateApiClientVs(this IHttpClientFactory httpClientFactory)
    {
        return httpClientFactory.CreateClient(CLIENT_NAME_VS);
    }
}
