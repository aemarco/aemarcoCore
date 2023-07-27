

namespace aemarco.Crawler.Person.Common;

internal abstract class PersonCrawlerBase
{

    private readonly Uri _siteUri;
    internal PersonCrawlerBase(Uri siteUri)
    {
        _siteUri = siteUri;
        Result.CrawlerInfos.Add(CrawlerInfo.FromCrawlerType(GetType()));
    }

    internal async Task<PersonInfo> GetPersonEntry(string nameToCrawl, CancellationToken cancellationToken)
    {
        var href = GetSiteHref(nameToCrawl);
        var target = new Uri(_siteUri, href);
        var document = await HtmlHelper.GetHtmlDocumentAsync(target, token: cancellationToken);
        await HandleDocument(document, cancellationToken);
        return Result;
    }
    protected abstract string GetSiteHref(string nameToCrawl);
    protected abstract Task HandleDocument(HtmlDocument document, CancellationToken cancellationToken);

    protected PersonInfo Result { get; } = new();


    #region Update Result

    protected void UpdateName(HtmlNode? node)
    {
        if (node is null)
            return;

        var name = node.TextWithout();
        var (firstName, lastName) = DataParser.FindNameInText(name);
        Result.FirstName = firstName ?? Result.FirstName;
        Result.LastName = lastName ?? Result.LastName;
    }

    protected void UpdateProfilePictures(HtmlNodeCollection? nodes, string attributeName, int? suggestedMinAdultLevel = null, int? suggestedMaxAdultLevel = null)
    {
        if (nodes is null)
            return;
        foreach (var node in nodes)
        {
            UpdateProfilePictures(node, attributeName, suggestedMinAdultLevel, suggestedMaxAdultLevel);
        }
    }

    protected void UpdateProfilePictures(HtmlNode? node, string attributeName, int? suggestedMinAdultLevel = null, int? suggestedMaxAdultLevel = null)
    {
        if (node?.Attributes[attributeName]?.Value is not { } imageRef)
            return;

        UpdateProfilePictures(
            DataParser.FindPictureUri(imageRef, _siteUri),
            suggestedMinAdultLevel,
            suggestedMaxAdultLevel);
    }

    protected void UpdateProfilePictures(Uri uri, int? suggestedMinAdultLevel = null, int? suggestedMaxAdultLevel = null)
    {
        Result.ProfilePictures.Add(new ProfilePicture(
            uri.AbsoluteUri,
            suggestedMinAdultLevel,
            suggestedMaxAdultLevel));
    }

    protected void UpdateMeasurements(string? text, bool isInches = false)
    {
        Result.MeasurementDetails = Result.MeasurementDetails.Combine(DataParser.FindMeasurementDetailsFromText(text, isInches));
    }


    #endregion
}