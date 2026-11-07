using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace aemarco.Crawler.Wallpaper.Clients;

internal static class WallhavenClientExtensions
{
    internal static IServiceCollection AddWallhavenClient(this IServiceCollection services)
    {
        services
            .AddHttpClient<IWallhavenClient, WallhavenClient>((sp, client) =>
            {
                var settings = sp.GetRequiredService<WallCrawlerSettings>();

                client.BaseAddress = WallhavenClient.BaseUri;
                client.DefaultRequestHeaders.UserAgent.ParseAdd("aemarcoWallpaperCrawler");
                if (!string.IsNullOrWhiteSpace(settings.WallhavenApiKey))
                    client.DefaultRequestHeaders.Add("X-API-Key", settings.WallhavenApiKey);
            })
            .AddCrawlerRateLimit();
        return services;
    }
}

/// <summary>
/// wallhaven.cc api v1 as typed HttpClient (base address, user agent and api key come with the registration)
/// https://wallhaven.cc/help/api
/// </summary>
internal class WallhavenClient : IWallhavenClient
{
    public static readonly Uri BaseUri = new("https://wallhaven.cc/api/v1/");

    /// <summary>
    /// categories: general/anime/people, purity: sfw/sketchy/nsfw, each as 0/1 flag (e.g. "010")
    /// </summary>
    public static Uri GetSearchUri(string categories, string purity) =>
        new(BaseUri, $"search?categories={categories}&purity={purity}&sorting=date_added&order=desc");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    private readonly HttpClient _httpClient;
    public WallhavenClient(
        HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<WallhavenSearchResult> Search(Uri searchUri, CancellationToken token)
    {
        var result = await Get<WallhavenSearchResult>(searchUri, token).ConfigureAwait(false);
        return result;
    }

    public async Task<WallhavenWallpaper> GetWallpaper(string id, CancellationToken token)
    {
        var result = await Get<WallhavenWallpaperResult>(new Uri(BaseUri, $"w/{id}"), token).ConfigureAwait(false);
        return result.Data;
    }

    private async Task<T> Get<T>(Uri uri, CancellationToken token)
    {
        using var response = await _httpClient.GetAsync(uri, token).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<T>(JsonOptions, token).ConfigureAwait(false);
        return result ?? throw new HttpRequestException($"Empty response from {uri}");
    }
}
