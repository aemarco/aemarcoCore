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
    }

    public async Task<PageDocument?> Navigate(PageUri? pageUri, CancellationToken token)
    {
        if (pageUri is null)
            return null;

        //a not found page is delivered (crawlers recognize it by its content), every other failure throws:
        //a 429, 403 or challenge page would parse as "no entries" and read as the end of the pages
        using var response = await _httpClient.GetAsync(pageUri.Uri, token).ConfigureAwait(false);
        if (response.StatusCode != HttpStatusCode.NotFound)
            response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync(token).ConfigureAwait(false);

        var document = new HtmlDocument();
        document.LoadHtml(html);
        return new PageDocument(pageUri, document);
    }
}
