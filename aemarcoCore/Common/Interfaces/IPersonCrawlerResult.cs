
using System;
using System.Collections.Generic;

namespace aemarcoCore.Common
{
    public interface IPersonCrawlerResult
    {
        List<IPersonEntry> Entries { get; }
        Exception Exception { get; set; }
        bool HasBeenAborted { get; set; }
        string JSON { get; }
        string ResultName { get; set; }
    }
}