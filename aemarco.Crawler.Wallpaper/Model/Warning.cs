namespace aemarco.Crawler.Wallpaper.Model;
public record Warning(CrawlerInfo CrawlerInfo, string Uri, string Message, string? AdditionalContext = null)
{
    public override string ToString()
    {
        var suffix = AdditionalContext is null
            ? null
            : $"{Environment.NewLine}{AdditionalContext}";
        return $"[{CrawlerInfo}] at {Uri}{Environment.NewLine}warns with message: {Message}{suffix}";
    }
}
