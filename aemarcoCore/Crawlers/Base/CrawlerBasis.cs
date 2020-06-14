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

        protected abstract int MinDelay { get; }
        protected abstract int MaxDelay { get; }

        public HtmlDocument GetDocument(Uri uri, int retry = 0)
        {
            _gate.Wait();

            if (MinDelay > 0 && MaxDelay > 0 && MaxDelay > MinDelay)
                Task.Delay(_random.Next(MinDelay, MaxDelay)).GetAwaiter().GetResult();

            try
            {
                return TryGetHtmlDocument(uri, retry);
            }
            finally
            {
                _gate.Release();
            }
        }



        public static HtmlDocument GetHtmlDocument(Uri uri, int retry = 0)
        {
            _gate.Wait();

            try
            {
                return TryGetHtmlDocument(uri, retry);
            }
            finally
            {
                _gate.Release();
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
}
