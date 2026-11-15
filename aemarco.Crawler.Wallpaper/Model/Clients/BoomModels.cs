// ReSharper disable NotAccessedPositionalProperty.Global
namespace aemarco.Crawler.Wallpaper.Model.Clients;

//json shapes of the boom.boombo.biz details, only what the crawler uses

internal record BoomWallpaper(
    int Id,
    string Title,
    BoomTag[] Tags);
internal record BoomTag(
    int Id,
    string Name,
    string Slug);
