namespace aemarco.Crawler.Person.Common;

internal interface IPersonCrawler
{
    Task<PersonInfo> GetPersonEntry(string firstName, string lastName, CancellationToken token);
}