namespace aemarco.Crawler.Person.Common;

public static class Extensions
{
    public static string TextWithout(this HtmlNode node, params string[] except)
    {
        var str = node.InnerText;
        str = WebUtility.HtmlDecode(str);
        foreach (var removal in except)
            str = str.Replace(removal, string.Empty, StringComparison.OrdinalIgnoreCase);
        str = str.Trim();
        str = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(str);
        return str;
    }

    public static string TextWithoutBeginning(this string text, string removeAtStart)
    {
        return text.StartsWith(removeAtStart, StringComparison.OrdinalIgnoreCase)
            ? text[removeAtStart.Length..].TrimStart()
            : text;
    }
}