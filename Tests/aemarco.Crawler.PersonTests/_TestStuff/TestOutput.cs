using System.Text.Json;

namespace aemarco.Crawler.PersonTests._TestStuff;

internal static class TestOutput
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };

    /// <summary>
    /// the site delivered something, the test sample does not expect, so we warn instead of failing
    /// </summary>
    public static void NothingExpected(object? obj)
    {
        if (obj is null)
            return;

        Assert.Warn($"""
                     Expected Nothing but found: <{obj.GetType().Name}>:
                     {JsonSerializer.Serialize(obj, Options)}
                     """);
    }
}
