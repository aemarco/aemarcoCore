using System.Collections.Generic;

namespace aemarcoCore.Crawlers
{
    public interface IWallEntry
    {
        string Url { get; }
        string FileName { get; }
        string Extension { get; }
        string Kategorie { get; }
        string SeitenKategorie { get; }
        List<string> Tags { get; }


        string getJSON();
    }
}
