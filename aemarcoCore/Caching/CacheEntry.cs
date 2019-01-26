using System;

namespace aemarcoCore.Caching
{
    [Serializable]
    public class CacheEntry
    {
        public DateTimeOffset ValidUntil { get; set; }

        public string Etag { get; set; }
        public string FileAdress { get; set; }
        public long Size { get; internal set; }
    }

}
