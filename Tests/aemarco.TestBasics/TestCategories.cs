namespace aemarco.TestBasics;

public static class TestCategories
{
    /// <summary>
    /// tests which hit the live sites, CI excludes them (sites block or reset calls from datacenter IPs)
    /// </summary>
    public const string LiveSite = "LiveSite";
}
