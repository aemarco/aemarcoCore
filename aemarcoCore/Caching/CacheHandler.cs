using aemarcoCore.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

namespace aemarcoCore.Caching
{
    public class CacheHandler
    {

        #region ctor

        private readonly DirectoryInfo _cachefolder;
        private readonly string _cacheStoreFile;
        private CacheEntryStore _store;
        private readonly HttpClient _client;
        private readonly Random _rand;
        private readonly char[] _alphanum = "ABCDEFG".ToCharArray();
        private readonly Regex _regex;
        private readonly Timer _cleanupTimer;
        private long _lastKnownCacheSize;
        private readonly CacheMode _cacheMode;
        public CacheHandler(HttpClient client, CacheMode mode)
        {
            var saveDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                System.Diagnostics.Process.GetCurrentProcess().ProcessName
                );
            _cachefolder = new DirectoryInfo(Path.Combine(saveDir, "CacheFolder"));

            if (!_cachefolder.Exists) _cachefolder.Create();
            _cacheStoreFile = Path.Combine(_cachefolder.FullName, "CacheStore.dat");
            _store = CacheEntryStore.Load(_cacheStoreFile);
            _client = client;
            _rand = new Random();
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            _regex = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            _lastKnownCacheSize = _store.GetAllValues().Sum(x => x.Size);
            _cacheMode = mode;

            _cleanupTimer = new Timer
            {
                AutoReset = true,
                Interval = TimeSpan.FromMinutes(1).TotalMilliseconds
            };
            _cleanupTimer.Elapsed += _cleanupTimer_Elapsed;
            _cleanupTimer.Start();
        }

        #endregion

        #region props/settings

        public int FolderDepth { get; set; } = 6;

        public long CacheSize { get; set; } = 262144000;
        public int CacheSizePercentage { get; set; } = 5;
        public double CacheThreshold { get; set; } = 0.9;

        #endregion

        #region Cleanup

        private void _cleanupTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _cleanupTimer.Stop();
            Cleanup();
            _cleanupTimer.Start();
        }

        public void Cleanup()
        {
            //determine CacheSize
            if (_cacheMode == CacheMode.Auto)
            {
                DriveInfo di = new DriveInfo(_cachefolder.FullName);
                CacheSize = (di.AvailableFreeSpace + _lastKnownCacheSize) / 100 * CacheSizePercentage;
            }

            CleanupKnownEntries();
            DeleteObsoleteFiles();
            //determine next occurance of time based cleanup
            lock (_store)
            {
                var earliest = _store.GetAllValues()
                                .OrderBy(x => x.ValidUntil)
                                .FirstOrDefault();
                if (earliest != null)
                {
                    _cleanupTimer.Interval = Math.Max(
                        (earliest.ValidUntil - DateTimeOffset.Now).TotalMilliseconds,
                        TimeSpan.FromHours(1).TotalMilliseconds);
                }
            }
        }
        private void CleanupKnownEntries()
        {
            lock (_store)
            {
                bool changed = false;

                //remove all items which are to old
                var toold = _store.GetAllValues()
                    .Where(x => x.ValidUntil < DateTimeOffset.Now)
                    .ToList();
                toold.ForEach(x =>
                {
                    _store.RemoveEntry(x);
                    changed = true;
                });

                //remove oldest items until thres. Cache is reached
                var toobig = _store.GetAllValues()
                    .OrderBy(x => x.ValidUntil)
                    .ToList();
                if (toobig.Sum(x => x.Size) > CacheSize)
                {
                    while (toobig.Sum(x => x.Size) > CacheSize * CacheThreshold)
                    {
                        _store.RemoveEntry(toobig[0]);
                        toobig.RemoveAt(0);
                        changed = true;
                    }
                    _lastKnownCacheSize = toobig.Sum(x => x.Size);
                }

                //save changes if any
                if (changed)
                {
                    if (!_store.Save())
                    {
                        _store = CacheEntryStore.Load(_cacheStoreFile);
                    }
                }
            }
        }

        private void DeleteObsoleteFiles()
        {
            List<FileInfo> files = null;
            List<string> known = null;

            lock (_store)
            {
                files = _cachefolder
                    .GetFiles("*.*", SearchOption.AllDirectories)
                    .ToList();

                known = _store.GetAllValues()
                    .Select(x => x.FileAdress)
                    .ToList();
            }

            files.Remove(files.First(x => x.FullName == _cacheStoreFile));
            files.ForEach(x =>
            {
                if (!known.Contains(x.FullName))
                {
                    x.Delete();
                }
            });

            DeleteEmptySubfolders(_cachefolder.FullName);
        }



        private void DeleteEmptySubfolders(string path, bool isHome = true)
        {
            try
            {
                //für Unterordner
                foreach (String strSubDir in Directory.GetDirectories(path))
                {
                    //für jeden Unterordner eine Instanz öffnen
                    if (!strSubDir.Contains("System Volume Information") &&
                        !strSubDir.Contains("$RECYCLE.BIN") &&
                        !strSubDir.Contains("#recycle"))
                        DeleteEmptySubfolders(strSubDir, false);
                }
                // von hinten nach vorne löschen
                if (!isHome)
                {
                    DirectoryInfo di = new DirectoryInfo(path);
                    if (di.GetFiles("*", SearchOption.AllDirectories).Length == 0)
                        Directory.Delete(path);
                }

            }
            catch { }
        }





        #endregion

        #region Entryhandling

        public Stream GetEntryFromCache(string url)
        {
            byte[] bytes = null;

            //check url
            var resp = _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead).Result;
            resp.EnsureSuccessStatusCode();

            //cache duration
            var expire = resp.Content.Headers.Expires ?? DateTimeOffset.Now.AddDays(7);

            //etag
            var etag = resp.Headers.ETag?.ToString().Replace("\"", "");

            lock (_store)
            {
                CacheEntry entry = null;
                bool needsFile = true;

                //get entry
                if (_store.EntryExist(url))
                {//some entry in store
                    entry = _store.GetEntry(url);
                    if (etag == null || entry.Etag == etag)
                    {//entry still valid
                        needsFile = false;
                    }
                }
                else
                {//need to add new entry to store
                    entry = new CacheEntry();
                }

                //update entry
                entry.ValidUntil = expire;
                entry.Etag = etag;
                //adds or replaces entry in store                
                if (needsFile)
                {
                    AddNewEntryOrUpdate(url, entry, resp);
                }
                bytes = File.ReadAllBytes(entry.FileAdress);

                if (!_store.Save())
                {
                    _store = CacheEntryStore.Load(_cacheStoreFile);
                }
            }

            //maybe cleanup stuff
            if (_lastKnownCacheSize > CacheSize)
            {
                Task.Factory.StartNew(() => Cleanup());
            }

            return new MemoryStream(bytes);
        }

        private void AddNewEntryOrUpdate(string key, CacheEntry entry, HttpResponseMessage resp)
        {
            //determine cache target
            var subfolder = CreateSubfolderstring();
            var sub = string.Join("\\", subfolder.ToCharArray());
            DirectoryInfo folder = new DirectoryInfo($"{_cachefolder.FullName}\\{sub}");
            if (!folder.Exists) folder.Create();
            string filename = _regex.Replace(Path.GetFileNameWithoutExtension(key), "");
            string target = Path.Combine(folder.FullName, $"{filename}.dat");

            //download
            using (Stream stream = resp.Content.ReadAsStreamAsync().Result)
            {
                using (var fs = File.Create(target))
                {
                    stream.CopyTo(fs);
                    entry.Size = fs.Length;
                }
            }

            //add or update Cache 
            entry.FileAdress = target;
            _store.AddOrUpdateEntry(key, entry);

            //remember added size
            _lastKnownCacheSize += entry.Size;
        }

        private string CreateSubfolderstring()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < FolderDepth; i++)
            {
                sb.Append(_alphanum[_rand.Next(0, _alphanum.Length)]);
            }
            return sb.ToString();
        }

        public void ClearCache()
        {
            lock (_store)
            {
                //clear entries
                _store.ClearEntries();
                //save changes
                if (!_store.Save())
                {
                    _store = CacheEntryStore.Load(_cacheStoreFile);
                }
            }
            DeleteObsoleteFiles();
        }

        #endregion

    }

}
