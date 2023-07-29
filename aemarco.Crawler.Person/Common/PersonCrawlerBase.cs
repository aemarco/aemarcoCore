namespace aemarco.Crawler.Person.Common;

internal abstract class PersonCrawlerBase
{
    internal PersonCrawlerBase()
    {
        Result.CrawlerInfos.Add(CrawlerInfo.FromCrawlerType(GetType()));
    }

    protected PersonInfo Result { get; } = new();

    internal async Task<PersonInfo> GetPersonEntry(string nameToCrawl, CancellationToken token)
    {
        var girlUri = GetGirlUri(nameToCrawl);
        var girlPage = await girlUri.NavigateAsync(token: token);

        await HandleGirlPage(girlPage, token);

        return Result;
    }


    protected abstract PageUri GetGirlUri(string nameToCrawl);
    protected abstract Task HandleGirlPage(PageDocument girlPage, CancellationToken token);



    #region Update Result

    protected void UpdateName(PageNode? node)
    {
        if (node is null)
            return;

        var text = node.GetText();
        var (firstName, lastName) = PersonParser.FindNameInText(text);

        Result.FirstName = firstName ?? Result.FirstName;
        Result.LastName = lastName ?? Result.LastName;
    }

    protected void UpdateProfilePictures(PageUri? uri, int? suggestedMinAdultLevel = null, int? suggestedMaxAdultLevel = null)
    {
        if (uri is null)
            return;

        Result.ProfilePictures.Add(new ProfilePicture(
            uri.Uri.AbsoluteUri,
            suggestedMinAdultLevel,
            suggestedMaxAdultLevel));
    }

    protected void UpdateMeasurements(string? text, bool isInches = false)
    {
        if (text is null)
            return;

        var format = isInches
            ? new MeasurementFormatProvider(MeasurementSystem.Imperial)
            : null;
        if (MeasurementDetails.TryParse(text, format, out var parsed))
        {
            Result.MeasurementDetails = Result.MeasurementDetails.Combine(parsed);
        }
    }

    protected void UpdateSocial(PageUri? socialLink, SocialLinkKind kind = SocialLinkKind.Unknown)
    {
        if (socialLink is null)
            return;

        var link = SocialLink.FromUri(socialLink.Uri);

        //override when specified
        if (kind != SocialLinkKind.Unknown)
            link = link with { Kind = kind };

        //add only known kinds
        if (link.Kind != SocialLinkKind.Unknown)
            Result.SocialLinks.Add(link);
    }


    #endregion
}