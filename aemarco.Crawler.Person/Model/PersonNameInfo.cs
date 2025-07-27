namespace aemarco.Crawler.Person.Model;

public record PersonNameInfo(string FirstName, string LastName)
{

    internal static PersonNameInfo? FromPersonInfo(PersonInfo personInfo)
    {
        if (string.IsNullOrWhiteSpace(personInfo.FirstName) ||
            string.IsNullOrWhiteSpace(personInfo.LastName))
        {
            return null;
        }
        return new PersonNameInfo(personInfo.FirstName, personInfo.LastName);
    }



}
