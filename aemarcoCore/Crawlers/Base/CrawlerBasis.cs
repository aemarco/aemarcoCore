using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace aemarcoCore.Crawlers.Base
{
    internal abstract class CrawlerBasis
    {
        public static HtmlDocument GetDocument(Uri uri, int retry = 0)
        {
            var web = new HtmlWeb()
            {
                PreRequest = request =>
                {
                    request.AutomaticDecompression =
                        DecompressionMethods.Deflate |
                        DecompressionMethods.GZip;
                    return true;
                }
            };
            try
            {
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

        }
       
    }
}
