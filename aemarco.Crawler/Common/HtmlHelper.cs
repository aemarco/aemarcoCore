using System.Threading.Tasks;

namespace aemarco.Crawler.Common;

public static class HtmlHelper
{

    private static readonly SemaphoreSlim Gate = new(1);
    private static readonly Random Random = new();


    public static HtmlDocument GetHtmlDocument(Uri uri, int? minDelay = null, int? maxDelay = null) =>
        GetHtmlDocumentAsync(uri, minDelay, maxDelay, CancellationToken.None).GetAwaiter().GetResult();

    public static async Task<HtmlDocument> GetHtmlDocumentAsync(Uri uri, int? minDelay = null, int? maxDelay = null, CancellationToken token = default)
    {
        await Gate.WaitAsync(token);

        if (minDelay.HasValue && maxDelay.HasValue)
            await Task.Delay(Random.Next(minDelay.Value, maxDelay.Value), token);

        try
        {
            return TryGetHtmlDocument(uri);
        }
        finally
        {
            Gate.Release();
        }
    }

    private static HtmlDocument TryGetHtmlDocument(Uri uri, int retry = 0)
    {
        try
        {
            var web = new HtmlWeb()
            {
                PreRequest = request =>
                {
                    request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                    return true;
                }
            };
            return web.Load(uri);
        }
        catch (WebException ex)
        {
            if (retry >= 5)
            {
                throw;
            }

            if (ex.Status == WebExceptionStatus.TrustFailure)
            {
                return TryGetHtmlDocument(new Uri(uri.AbsoluteUri.Replace("https", "http")));
            }
            if (ex.Status == WebExceptionStatus.Timeout)
            {
                return TryGetHtmlDocument(uri, ++retry);
            }
            throw;
        }
    }

}