using aemarcoCore.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
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
        private readonly Timer _cleanupTimer;
        private long _lastKnownCacheSize;
        private CacheMode _cacheMode;
        private int _cacheSizePercentage = 5;
        private long _cacheSize = 262144000;


        public CacheHandler(HttpClient client, CacheMode mode)
        {
            //cachefolder
            var saveDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                System.Diagnostics.Process.GetCurrentProcess().ProcessName
                );
            _cachefolder = new DirectoryInfo(Path.Combine(saveDir, "CacheFolder"));
            if (!_cachefolder.Exists) _cachefolder.Create();

            //cachefile
            _cacheStoreFile = Path.Combine(_cachefolder.FullName, "CacheStore.dat");

            //caching
            _store = CacheEntryStore.Load(_cacheStoreFile);
            _client = client;
            _rand = new Random();
            _lastKnownCacheSize = _store.GetAllValues().Sum(x => x.Size);
            CacheMode = mode;

            //cleanup
            _cleanupTimer = new Timer
            {
                AutoReset = true,
                Interval = TimeSpan.FromMinutes(1).TotalMilliseconds
            };
            _cleanupTimer.Elapsed += _cleanupTimer_Elapsed;
            _cleanupTimer.Start();
        }

        #endregion

        #region props

        //settings
        public CacheMode CacheMode
        {
            get
            {
                if (_cacheSize > 0)
                {
                    return _cacheMode;
                }
                else
                {
                    if (_cacheMode != CacheMode.None && _lastKnownCacheSize > 0)
                        ClearCache();

                    return CacheMode.None;
                }
            }
            set
            {
                if (value != _cacheMode)
                {
                    _cacheMode = value;
                    if (_cacheMode == CacheMode.Auto) UpdateCacheSize();
                }
            }
        }
        public int CacheSizePercentage
        {
            get { return _cacheSizePercentage; }
            set
            {
                if (value != _cacheSizePercentage)
                {
                    _cacheSizePercentage = value;
                    if (_cacheMode == CacheMode.Auto) UpdateCacheSize();
                }
            }
        }
        private void UpdateCacheSize()
        {
            DriveInfo di = new DriveInfo(_cachefolder.FullName);
            _cacheSize = (di.AvailableFreeSpace + _lastKnownCacheSize) / 100 * _cacheSizePercentage;
        }
        public long CacheSize
        {
            get { return _cacheSize; }
            set
            {
                if (value != _cacheSize)
                {
                    _cacheSize = value;
                }
            }
        }

        //handling
        public int FolderDepth { get; set; } = 6;
        public double CacheThreshold { get; set; } = 0.9;

        //infos
        public CacheStats CacheStats
        {
            get
            {
                CacheStats stats;
                lock (_store)
                {
                    stats = _store.CacheStats;
                }
                stats.CurrentCacheSize = _lastKnownCacheSize;
                stats.MaxCacheSize = _cacheSize;

                return stats;
            }
        }

        #endregion

        #region Cleanup

        private async void _cleanupTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _cleanupTimer.Stop();
            await Task.Run(() => Cleanup(System.Threading.CancellationToken.None));
            _cleanupTimer.Start();
        }

        public void Cleanup(System.Threading.CancellationToken ct, bool clearAll = false)
        {
            //determine CacheSize
            if (_cacheMode == CacheMode.Auto) UpdateCacheSize();

            CleanupKnownEntries(ct, clearAll);
            DeleteObsoleteFiles(ct);

            //determine next occurance of time based cleanup
            if (!clearAll)
            {
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
        }
        private void CleanupKnownEntries(System.Threading.CancellationToken ct, bool clearAll)
        {
            lock (_store)
            {
                bool changed = false;

                if (clearAll)
                {
                    //clear entries
                    _store.ClearEntries();
                    changed = true;
                }
                else
                {
                    ////remove all items which are to old
                    //var toold = _store.GetAllValues()
                    //    .Where(x => x.ValidUntil < DateTimeOffset.Now)
                    //    .ToList();
                    //toold.ForEach(x =>
                    //{
                    //    _store.RemoveEntry(x);
                    //    changed = true;
                    //});

                    //remove oldest items until thres. Cache is reached
                    var toobig = _store.GetAllValues()
                        .OrderBy(x => x.ValidUntil)
                        .ToList();
                    if (toobig.Sum(x => x.Size) > _cacheSize)
                    {
                        while (toobig.Sum(x => x.Size) > _cacheSize * CacheThreshold && !ct.IsCancellationRequested)
                        {
                            _store.RemoveEntry(toobig[0]);
                            toobig.RemoveAt(0);
                            changed = true;
                        }
                    }
                }

                _lastKnownCacheSize = _store.GetAllValues().Sum(x => x.Size);
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

        private void DeleteObsoleteFiles(System.Threading.CancellationToken ct)
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
            foreach (var file in files)
            {
                if (ct.IsCancellationRequested) break;
                if (!known.Contains(file.FullName)) file.Delete();
            }

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

        public async Task<Stream> GetEntryFromCache(string url)
        {
            byte[] bytes = null;

            using (var resp = await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
            {
                resp.EnsureSuccessStatusCode();
                //expires
                var expire = (resp.Headers.CacheControl?.MaxAge is TimeSpan maxAge) ?
                    DateTimeOffset.Now.Add(maxAge) : DateTimeOffset.Now.AddDays(7);
                //etag
                var etag = resp.Headers.ETag?.Tag;

                CacheEntry entry = null;
                lock (_store)
                {
                    //get entry
                    if (_store.EntryExist(url))
                    {//some entry in store
                        entry = _store.GetEntry(url);

                        if (etag == null || entry.Etag == null || entry.Etag == etag)
                        {//entry still valid
                            bytes = File.ReadAllBytes(entry.FileAdress);
                            _store.ReportEntryUse();
                        }
                    }
                    else
                    {//need to add new entry to store
                        entry = new CacheEntry();
                    }

                    //update entry
                    entry.ValidUntil = expire;
                    entry.Etag = etag;
                }

                //adds or replaces entry in store                
                if (bytes == null)
                {
                    await AddNewEntryOrUpdate(url, entry, resp);
                    bytes = File.ReadAllBytes(entry.FileAdress);
                }

                lock (_store)
                {
                    if (!_store.Save())
                    {
                        _store = CacheEntryStore.Load(_cacheStoreFile);
                    }
                }

                //maybe cleanup stuff
                if (_lastKnownCacheSize > _cacheSize)
                {
                    _ = Task.Run(() => Cleanup(System.Threading.CancellationToken.None));
                }

                return new MemoryStream(bytes);
            }
        }

        private async Task AddNewEntryOrUpdate(string key, CacheEntry entry, HttpResponseMessage resp)
        {
            //determine cache target
            DirectoryInfo folder = new DirectoryInfo($"{_cachefolder.FullName}\\{CreateSubfolderstring()}");
            if (!folder.Exists) folder.Create();
            string filename = Regex.Replace(CalculateMD5Hash(key), @"[^A-Za-z0-9_\.~]+", "-");
            string target = Path.Combine(folder.FullName, $"{filename}.dat");

            entry.FileAdress = target;

            lock (_store)
            {
                //add or update Cache 
                _store.AddOrUpdateEntry(key, entry);
            }


            //download
            using (Stream stream = await resp.Content.ReadAsStreamAsync())
            {
                using (var fs = File.Create(target))
                {
                    stream.CopyTo(fs);
                    entry.Size = fs.Length;
                    //remember added size
                    _lastKnownCacheSize += entry.Size;
                }
            }
        }

        private string CreateSubfolderstring()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < FolderDepth; i++)
            {
                sb.Append(_alphanum[_rand.Next(0, _alphanum.Length)]);
            }
            var sub = string.Join("\\", sb.ToString().ToCharArray());
            return sub;
        }

        private string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)

            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();

        }

        private async void ClearCache()
        {
            await ClearCache(System.Threading.CancellationToken.None);
        }

        public async Task ClearCache(System.Threading.CancellationToken ct)
        {
            _cleanupTimer.Stop();
            await Task.Run(() => Cleanup(ct, true)).ConfigureAwait(false);
            _cleanupTimer.Start();
        }

        #endregion

    }

}
