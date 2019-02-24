using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aemarcoCore.Caching
{
    public class CacheStats
    {
        public long EntryAccessCount { get; internal set; }
        public long EntryUseCount { get; internal set; }
        public long EntryNotFoundCount { get; internal set; }

        public string HitRate
        {
            get
            {
                return ((1.0 * EntryUseCount) / (EntryAccessCount + EntryNotFoundCount) * 100).ToString("N1");
            }
        }


        public long CurrentCacheSize { get; internal set; }
        public long MaxCacheSize { get; internal set; }

        public double CacheUsage
        {
            get { return Math.Min((1.0 * CurrentCacheSize / MaxCacheSize) * 100, 100); }
        }


    }
}
