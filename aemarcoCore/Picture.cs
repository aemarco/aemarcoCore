using System;
using System.IO;
using System.Net;

namespace aemarcoCore
{
    public class Picture
    {
        #region field

        private int _id;

        private Func<int, byte[]> _load;
        private Action<int> _delete;
        private Action<Picture, byte[]> _save;

        private string _extension;
        private DateTime _lastModified;

        #endregion

        #region ctor

        //No Picture
        public Picture(Func<int, byte[]> load, Action<int> delete, Action<Picture, byte[]> save)
        {
            _id = -1;

            _load = load;
            _delete = delete;
            _save = save;

            _extension = string.Empty;
            _lastModified = DateTime.MinValue;
        }

        //from DB
        public Picture(int id, string extension, DateTime lastModified,
            Func<int, byte[]> load, Action<int> delete, Action<Picture, byte[]> save)
        {
            _id = id;

            _load = load;
            _delete = delete;
            _save = save;

            _extension = extension;
            _lastModified = lastModified;
        }


        #endregion

        #region props

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public string Extension
        {
            get { return _extension; }
        }
        public DateTime LastModified
        {
            get { return _lastModified; }
        }

        #endregion

        #region methods

        public void Delete()
        {
            //noPicture
            if (_id <= 0)
            {
                return;
            }

            _delete(_id);

            _id = -1;
            _extension = string.Empty;
            _lastModified = DateTime.MinValue;
        }
        public void SetNewPicture(string argFilepath)
        {
            //muss ein filePath geben
            if (String.IsNullOrEmpty(argFilepath))
            {
                return;
            }

            byte[] newBytes = GetBytes(argFilepath);
            //muss ein neues Bild geben
            if (newBytes == null)
            {
                return;
            }

            //altes Bild laden
            byte[] oldBytes = GetBytes(GetLocalFile());

            //alt und neu vergleichen falls ein altes vorhanden
            if (oldBytes != null && IsSamePicture(newBytes, oldBytes))
            {
                return;
            }

            //neue Infos setzen            
            _extension = Path.GetExtension(argFilepath).ToLower();
            _lastModified = DateTime.UtcNow;

            //speichern
            _save(this, newBytes);
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Objekte nicht mehrmals verwerfen")]
        public string GetLocalFile()
        {
            //noPicture
            if (_id <= 0)
            {
                return null;
            }

            //auf lokale Datei prüfen die aktuell ist
            FileInfo fiContent = new FileInfo($"{Environment.CurrentDirectory}\\Pictures\\{_id}{_extension}");
            FileInfo fiInfo = new FileInfo($"{fiContent.FullName}.info");
            if (fiContent.Exists && fiInfo.Exists &&
                File.ReadAllText(fiInfo.FullName) == _lastModified.ToString())
            {
                return fiContent.FullName;
            }

            //Bild aus DB laden falls nötig
            var pictureBytes = _load(_id);
            //noPicture
            if (pictureBytes == null)
            {
                return null;
            }

            //Falls kein lokales Bild oder nicht aktuell, DB Bild speichern
            if (!fiContent.Directory.Exists)
            {
                fiContent.Directory.Create();
            }
            using (var fs = new FileStream(fiContent.FullName, FileMode.Create))
            {
                using (var bw = new BinaryWriter(fs))
                {
                    bw.Write(pictureBytes);
                }
            }
            File.WriteAllText(fiInfo.FullName, _lastModified.ToString());

            return fiContent.FullName;
        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Objekte nicht mehrmals verwerfen")]
        private byte[] GetBytes(string argFilepath)
        {

            byte[] content = null;

            try
            {
                if (argFilepath.StartsWith("http"))
                {
                    using (Stream source = new WebClient().OpenRead(argFilepath))
                    {
                        using (var ms = new MemoryStream())
                        {
                            source.CopyTo(ms);
                            content = ms.ToArray();
                        }
                    }
                }
                else
                {
                    using (var fs = new FileStream(argFilepath, FileMode.Open))
                    {
                        using (var br = new BinaryReader(fs))
                        {
                            content = br.ReadBytes((int)fs.Length);
                        }
                    }
                }
            }
            catch { }


            return content;
        }
        private bool IsSamePicture(byte[] input1, byte[] input2)
        {
            if (input1 == null || input2 == null)
                return false;

            if (input1.Length != input2.Length)
                return false;

            for (int i = 0; i < input1.Length; i++)
                if (input1[i] != input2[i])
                    return false;

            return true;
        }

        #endregion
    }
}
