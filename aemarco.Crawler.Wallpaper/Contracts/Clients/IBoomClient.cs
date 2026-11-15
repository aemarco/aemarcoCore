namespace aemarco.Crawler.Wallpaper.Contracts.Clients;

/// <summary>
/// the json details of boom.boombo.biz (what its page loads for a wallpaper), implemented by the typed BoomClient
/// </summary>
internal interface IBoomClient
{
    /// <returns>null when the wallpaper is gone (404)</returns>
    Task<BoomWallpaper?> GetWallpaper(int id, CancellationToken token);
}
