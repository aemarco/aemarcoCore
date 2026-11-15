using System.Net.Http.Json;
using System.Text.Json;

namespace aemarco.Crawler.Wallpaper.Clients;

internal static class BoomClientExtensions
{
    internal static IServiceCollection AddBoomClient(this IServiceCollection services)
    {
        services
            .AddHttpClient<IBoomClient, BoomClient>(client =>
            {
                client.BaseAddress = BoomClient.BaseUri;
                client.DefaultRequestHeaders.UserAgent.ParseAdd("aemarcoWallpaperCrawler");
            })
            .AddCrawlerPipeline();
        return services;
    }
}

/// <summary>
/// the json details of boom.boombo.biz as typed HttpClient (base address and user agent come with the registration)
/// </summary>
internal class BoomClient : IBoomClient
{
    public static readonly Uri BaseUri = new("https://boom.boombo.biz/");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    public BoomClient(
        HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    //z.B. "https://boom.boombo.biz/wallpaper/127309/?ajax=1"
    public async Task<BoomWallpaper?> GetWallpaper(int id, CancellationToken token)
    {
        var uri = new Uri(BaseUri, $"wallpaper/{id}/?ajax=1");
        using var response = await _httpClient.GetAsync(uri, token).ConfigureAwait(false);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<BoomWallpaper>(JsonOptions, token).ConfigureAwait(false);
        return result ?? throw new HttpRequestException($"Empty response from {uri}");
    }
}
