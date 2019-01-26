using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace aemarcoCore.Caching
{
    [Serializable]
    public class CacheEntryStore
    {
        #region ctor

        private readonly string _fileAdress = string.Empty;
        private readonly Dictionary<string, CacheEntry> entries = new Dictionary<string, CacheEntry>();
        
        public CacheEntryStore(string file)
        {
            _fileAdress = file;
        }

        #endregion

        #region Handling entries

        public void AddOrUpdateEntry(string key, CacheEntry entry)
        {
            if (!entries.ContainsKey(key))
            {
                entries.Add(key, entry);
            }
            else
            {
                entries[key] = entry;
            }
        }

        public bool EntryExist(string key)
        {
            return entries.ContainsKey(key);
        }
        
        public CacheEntry GetEntry(string key)
        {
            if (entries.ContainsKey(key))
            {
                return entries[key];
            }
            throw new KeyNotFoundException();
        }
        
        public List<string> GetAllKeys()
        {
            return entries.Keys.ToList();
        }

        public List<CacheEntry> GetAllValues()
        {
            return entries.Values.ToList();
        }

        public void RemoveEntry(string key)
        {
            if (entries.ContainsKey(key))
            {
                entries.Remove(key);
            }
        }

        public void RemoveEntry(CacheEntry entry)
        {
            var key = entries.FirstOrDefault(x => x.Value == entry).Key;
            RemoveEntry(key);
        }

        public void ClearEntries()
        {
            entries.Clear();
        }

        #endregion

        #region save/load

        /// <summary>
        /// Saves the current object
        /// </summary>
        /// <returns>true if save was successfull</returns>
        public bool Save()
        {
            try
            {
                using (FileStream fs = new FileStream(_fileAdress, FileMode.Create))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fs, this);
                }
                return true;
            }
            catch
            {
                try { File.Delete(_fileAdress); } catch { }
                return false;
            }
        }
        
        /// <summary>
        /// gets a storage instance, new one or from saved one
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static CacheEntryStore Load(string file)
        {
            if (!File.Exists(file))
            {
                return new CacheEntryStore(file);
            }

            try
            {
                using (FileStream fs = new FileStream(file, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    return (CacheEntryStore)bf.Deserialize(fs);
                }
            }
            catch
            {
                return new CacheEntryStore(file);
            }
        }

        #endregion

    }

}
