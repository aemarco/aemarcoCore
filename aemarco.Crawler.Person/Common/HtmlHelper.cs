﻿using HtmlAgilityPack;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace aemarcoCommons.PersonCrawler.Common
{
    public static class HtmlHelper
    {
        private static readonly SemaphoreSlim Gate = new SemaphoreSlim(1);
        private static readonly Random Random = new Random();

        public static HtmlDocument GetHtmlDocument(Uri uri, int? minDelay = null, int? maxDelay = null)
        {
            Gate.Wait();

            if (minDelay.HasValue && maxDelay.HasValue)
                Task.Delay(Random.Next(minDelay.Value, maxDelay.Value)).GetAwaiter().GetResult();

            try
            {
                return TryGetHtmlDocument(uri);
            }
            finally
            {
                Gate.Release();
            }
        }

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
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
}
