namespace aemarco.Crawler.Common;

public record PageUri(Uri Uri)
{

    /// <summary>
    ///
    /// - pageXy --> relative to current
    /// - /pageXy --> relative to root
    /// - ../ --> relative to parent
    /// </summary>
    /// <param name="relativeUri"></param>
    /// <returns></returns>
    public PageUri WithHref(string relativeUri)
    {
        var uri = new Uri(Uri, relativeUri);
        return new PageUri(uri);
    }

    public PageUri WithParam(string key, string value)
    {
        // Get the original query part of the URI (without the '?')
        var originalQuery = Uri.Query.TrimStart('?');

        // Encode the parameter name and value to ensure proper formatting
        var encodedParamName = HttpUtility.UrlEncode(key);
        var encodedParamValue = HttpUtility.UrlEncode(value);

        // Create the new query string
        var newQuery = string.IsNullOrEmpty(originalQuery)
            ? $"{encodedParamName}={encodedParamValue}"
            : $"{originalQuery}&{encodedParamName}={encodedParamValue}";

        // Combine the new query with the existing URI parts
        var updatedUri = $"{Uri.GetLeftPart(UriPartial.Path)}?{newQuery}";
        var newUri = new Uri(updatedUri);
        return new PageUri(newUri);
    }


    public PageDocument Navigate(int? minDelay = null, int? maxDelay = null)
    {
        var document = HtmlHelper.GetHtmlDocument(Uri, minDelay, maxDelay);
        return new PageDocument(this, document);
    }


    public static implicit operator Uri(PageUri x) => x.Uri;
    public static implicit operator PageUri?(Uri? x) => x is null ? null : new PageUri(x);
}