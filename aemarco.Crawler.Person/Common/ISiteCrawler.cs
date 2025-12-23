namespace aemarco.Crawler.Person.Common;

internal interface ISiteCrawler
{
    Task<PersonName[]> GetPersonNameEntries(CancellationToken token);
    Task<PersonInfo> GetPersonEntry(string firstName, string lastName, CancellationToken token);
}

internal abstract class SiteCrawlerBase : ISiteCrawler
{

    private readonly ICountryService _countryService;
    private readonly ILogger _logger;
    internal SiteCrawlerBase(
        ICountryService countryService,
        ILogger logger)
    {
        _countryService = countryService;
        _logger = logger;


        Result = new PersonInfo();
        Result.CrawlerInfos.Add(CrawlerInfo.FromCrawlerType(GetType()));
    }
    protected PersonInfo Result { get; }


    //IPersonCrawler
    public async Task<PersonName[]> GetPersonNameEntries(CancellationToken token)
    {
        var result = await HandlePersonNameEntries(token);
        return result;
    }
    public async Task<PersonInfo> GetPersonEntry(string firstName, string lastName, CancellationToken token)
    {
        var name = $"{firstName} {lastName}";
        var girlUri = GetGirlUri(name);
        _logger.LogDebug("GetGirlUri resolved {uri} for {name}", girlUri.Uri.AbsoluteUri, name);

        var ci = Result.CrawlerInfos.First();
        ci.Url = girlUri.Uri.AbsoluteUri;

        var girlPage = await girlUri.NavigateAsync(token: token);
        await HandlePersonEntry(girlPage, token);
        return Result;
    }



    protected virtual Task<PersonName[]> HandlePersonNameEntries(CancellationToken token)
    {
        PersonName[] result = [];
        return Task.FromResult(result);
    }


    protected abstract PageUri GetGirlUri(string name);
    protected abstract Task HandlePersonEntry(PageDocument girlPage, CancellationToken token);




    #region Update Result

    protected void UpdateName(PageNode? node, params string[] removals)
    {
        if (node is null)
            return;

        var text = node.GetText().Except(removals);

        var (firstName, lastName) = PersonParser.FindNameInText(text);

        Result.FirstName = firstName;
        Result.LastName = lastName;
    }

    protected void UpdateCountry(string? text)
    {
        if (text is null)
            return;

        if (_countryService.FindCountry(text) is not { } country)
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
    protected void UpdateMeasurements(MeasurementDetails? details)
    {
        if (details is null)
            return;

        Result.MeasurementDetails = Result.MeasurementDetails.Combine(details);
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
