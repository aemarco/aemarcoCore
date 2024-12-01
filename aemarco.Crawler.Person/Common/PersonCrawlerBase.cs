using aemarco.Crawler.Services;

namespace aemarco.Crawler.Person.Common;

internal abstract class PersonCrawlerBase : IPersonCrawler
{
    private static readonly CountryService CountryService = new();
    internal PersonCrawlerBase()
    {
        Result = new PersonInfo();
        Result.CrawlerInfos.Add(CrawlerInfo.FromCrawlerType(GetType()));
    }

    public async Task<PersonInfo> GetPersonEntry(string name, CancellationToken token)
    {
        var girlUri = GetGirlUri(name);
        var girlPage = await girlUri.NavigateAsync(token: token);
        await HandleGirlPage(girlPage, token);
        return Result;
    }

    public async Task<PersonInfo> GetPersonEntry(string firstName, string lastName, CancellationToken token)
    {
        var name = $"{firstName} {lastName}";
        return await GetPersonEntry(name, token);
    }




    protected PersonInfo Result { get; }
    protected abstract PageUri GetGirlUri(string name);
    protected abstract Task HandleGirlPage(PageDocument girlPage, CancellationToken token);

    #region Update Result

    protected void UpdateName(PageNode? node)
    {
        if (node is null)
            return;

        var text = node.GetText();
        var (firstName, lastName) = PersonParser.FindNameInText(text);

        Result.FirstName = firstName;
        Result.LastName = lastName;
    }

    protected void UpdateCountry(string? text)
    {
        if (text is null)
            return;

        if (CountryService.FindCountry(text) is not { } country)
            return;

        Result.Country = country;
    }


    protected bool UpdateProfilePictures(PageUri? uri, int? suggestedMinAdultLevel = null, int? suggestedMaxAdultLevel = null)
    {
        if (uri is null)
            return false;

        var url = uri.WithoutQuery().Uri.AbsoluteUri;
        var profilePicture = new ProfilePicture(
            url,
            suggestedMinAdultLevel,
            suggestedMaxAdultLevel);

        Result.ProfilePictures.Add(profilePicture);
        return true;
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
        Result.SocialLinks.Add(link);
    }

    protected void UpdateWellKnownSocial(PageUri? link)
    {
        if (link is null)
            return;

        var social = SocialLink.FromUri(link.Uri);
        if (social.Kind == SocialLinkKind.Unknown)
            return;

        Result.SocialLinks.Add(social);
    }


    #endregion

}