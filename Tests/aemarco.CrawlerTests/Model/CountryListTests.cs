using aemarco.Crawler.Model;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

namespace aemarco.CrawlerTests.Model;
internal class CountryListTests : TestBase
{

    [TestCase(null, null)]
    [TestCase("Canada", "Canada")]
    [TestCase("Hungary", "Hungary")]
    [TestCase("United States", "United States")]
    [TestCase("American", "United States")]
    [TestCase("USA", "United States")]
    [TestCase("US", "United States")]
    [TestCase("Russian Federation", "Russia")]
    [TestCase("Russia", "Russia")]
    [TestCase("Russian", "Russia")]
    public void FindCountry(string? text, string? expected)
    {
        var result = CountryList.FindCountry(text);

        result.Should().Be(expected);

        PrintJson(result);
    }

}


internal abstract class TestBase
{
    protected static void NothingExpected(object? found)
    {
        if (found is null)
            return;

        Assert.Warn($"""
                     Expected Nothing but found: {GetTypeName(found)}
                     {JsonConvert.SerializeObject(found, Formatting.Indented)}
                     """);
    }

    protected static void PrintJson(object? obj)
    {
        if (obj is null)
        {
            TestContext.Out.WriteLine("Passed with: null");
            return;
        }

        TestContext.Out.WriteLine($"""
                                   Passed with: {GetTypeName(obj)}
                                    {JsonConvert.SerializeObject(obj, Formatting.Indented)}
                                   """);
    }


    private static string GetTypeName(object obj)
    {
        var type = obj.GetType();
        if (obj is not IEnumerable or string)
            return type.Name;

        var elementType = GetCollectionElementType(type);
        return $"{elementType?.Name ?? "UnknownType"}[]";
    }
    private static Type? GetCollectionElementType(Type collectionType)
    {
        if (collectionType.IsArray)
            return collectionType.GetElementType();


        // Handle collections implementing IEnumerable<T>
        var genericArguments = collectionType.GetGenericArguments();
        if (genericArguments.Length > 0)
            return genericArguments[0];


        // Handle non-generic collections implementing IEnumerable
        if (typeof(IEnumerable).IsAssignableFrom(collectionType))
        {
            foreach (var interfaceType in collectionType.GetInterfaces())
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    return interfaceType.GetGenericArguments()[0];
                }
            }
        }
        return null;
    }


}
