// ReSharper disable NotAccessedPositionalProperty.Global
namespace aemarco.Crawler.Wallpaper.Model.Clients;

//json shapes of the wallhaven api, snake_case on the wire

internal record WallhavenSearchResult(
    WallhavenListItem[] Data,
    WallhavenMeta Meta);
internal record WallhavenMeta(
    int CurrentPage,
    int LastPage,
    int PerPage);
internal record WallhavenListItem(
    string Id,
    string Path,
    WallhavenThumbs Thumbs);
internal record WallhavenThumbs(
    string Small,
    string Large,
    string Original);

internal record WallhavenWallpaperResult(
    WallhavenWallpaper Data);
internal record WallhavenWallpaper(
    string Id,
    string Path,
    WallhavenThumbs Thumbs,
    WallhavenTag[] Tags);
internal record WallhavenTag(
    int Id,
    string Name,
    string? Alias,
    int? CategoryId,
    string? Category,
    string? Purity);
