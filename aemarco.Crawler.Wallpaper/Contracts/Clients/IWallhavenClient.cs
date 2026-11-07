namespace aemarco.Crawler.Wallpaper.Contracts.Clients;

/// <summary>
/// wallhaven.cc api v1, implemented by the typed WallhavenClient
/// </summary>
internal interface IWallhavenClient
{
    Task<WallhavenSearchResult> Search(Uri searchUri, CancellationToken token);
    Task<WallhavenWallpaper> GetWallpaper(string id, CancellationToken token);
}
