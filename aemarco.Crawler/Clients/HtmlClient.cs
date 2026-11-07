namespace aemarco.Crawler.Clients;

/// <summary>
/// typed HttpClient for crawled sites, HtmlAgilityPack only parses
/// </summary>
internal class HtmlClient : IHtmlClient
{

    private readonly HttpClient _httpClient;
    public HtmlClient(
        HttpClient httpClient)
    {
        _httpClient = httpClient;

        //the user agent HtmlWeb sent before, the crawled sites know us by it
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:x.x.x) Gecko/20041107 Firefox/x.x");
        //SiteRequestHandler times out each attempt itself
        _httpClient.Timeout = Timeout.InfiniteTimeSpan;
    }

    public async Task<PageDocument?> Navigate(PageUri? pageUri, CancellationToken token)
    {
        if (pageUri is null)
            return null;

        //like HtmlWeb, a non success status still delivers its page (crawlers recognize a not found page by its content)
        using var response = await _httpClient.GetAsync(pageUri.Uri, token).ConfigureAwait(false);
        var html = await response.Content.ReadAsStringAsync(token).ConfigureAwait(false);

        var document = new HtmlDocument();
        document.LoadHtml(html);
        return new PageDocument(pageUri, document);
    }
}
