using HtmlAgilityPack;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace aemarcoCore.Crawlers.Base
{
    internal abstract class CrawlerBasis
    {

        private static readonly SemaphoreSlim _gate = new SemaphoreSlim(1);
        private static readonly Random _random = new Random();

        public static HtmlDocument GetDocument(Uri uri, int retry = 0)
        {
            _gate.Wait();
            Task.Delay(_random.Next(500, 1500)).GetAwaiter().GetResult();

            try
            {
                var web = new HtmlWeb()
                {
                    PreRequest = request =>
                    {
                        request.AutomaticDecompression =  DecompressionMethods.Deflate | DecompressionMethods.GZip;
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
                    return GetDocument(new Uri(uri.AbsoluteUri.Replace("https", "http")));
                }
                else if (ex.Status == WebExceptionStatus.Timeout)
                {
                    return GetDocument(uri, ++retry);
                }

                throw;
            }
            finally
            {
                _gate.Release();
            }
        }

    }
}
