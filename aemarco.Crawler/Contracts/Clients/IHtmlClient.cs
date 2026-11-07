namespace aemarco.Crawler.Contracts.Clients;

/// <summary>
/// loads the pages of crawled sites, through the crawler rate limiting
/// </summary>
public interface IHtmlClient
{
    /// <summary>
    /// null in, null out, so a link which was not found needs no extra check (e.g. Navigate(node?.GetHref()))
    /// </summary>
    Task<PageDocument?> Navigate(PageUri? pageUri, CancellationToken token);
}
