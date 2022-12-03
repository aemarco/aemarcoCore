using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace aemarco.Crawler.Wallpaper.Common
{
    public static class HtmlHelper
    {
        private static readonly SemaphoreSlim Gate = new SemaphoreSlim(1);
        private static readonly Random Random = new Random();

        private static readonly Dictionary<string, HtmlDocumentCacheEntry> DocumentsCache = new Dictionary<string, HtmlDocumentCacheEntry>();

        private static void CacheCleanup()
        {
            foreach (var key in DocumentsCache.Keys.ToList())
            {
                if (DocumentsCache[key].Timestamp.AddHours(1) < DateTimeOffset.Now)
                {
                    DocumentsCache.Remove(key);
                }
            }
        }



        public static HtmlDocument GetHtmlDocument(Uri uri, int? minDelay = null, int? maxDelay = null)
        {
            Gate.Wait();

            try
            {
                CacheCleanup();
                if (DocumentsCache.ContainsKey(uri.AbsoluteUri))
                    return DocumentsCache[uri.AbsoluteUri].HtmlDocument;

                if (minDelay.HasValue && maxDelay.HasValue)
                    Task.Delay(Random.Next(minDelay.Value, maxDelay.Value)).GetAwaiter().GetResult();


                var result = TryGetHtmlDocument(uri);

                DocumentsCache[uri.AbsoluteUri] = new HtmlDocumentCacheEntry(result);

                return result;
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
                else if (ex.Status == WebExceptionStatus.Timeout)
                {
                    return TryGetHtmlDocument(uri, ++retry);
                }
                throw;
            }
        }

    }

    public class HtmlDocumentCacheEntry
    {
        public HtmlDocumentCacheEntry(HtmlDocument htmlDocument)
        {
            Timestamp = DateTimeOffset.Now;
            HtmlDocument = htmlDocument;
        }

        public DateTimeOffset Timestamp { get; }
        public HtmlDocument HtmlDocument { get; }
    }


}
