using Clarity;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ExplorerLib.Entities
{
    public class File : PropertyChangedBase
    {
        public File()
        {
            OnChangeOf(() => Filename).Execute(UpdateInfo);
            OnChangeOf(() => FileSize).Validate(() => FileSizeDescription);
        }

        private void UpdateInfo()
        {
            if (string.IsNullOrEmpty(Filename))
            {
                Name=string.Empty;
                Extension = string.Empty;
                FileSize = 0;
            }
            else
            {
                Name = System.IO.Path.GetFileName(Filename);
                Extension = System.IO.Path.GetExtension(Filename);
                var info = new System.IO.FileInfo(Filename);
                FileSize = info.Length;
                DateModified = info.LastWriteTime;
            }
        }

        private string _filename;
        public virtual string Filename
        {
            get
            {
                return _filename;
            }
            set
            {
                SetValue(ref _filename, value, () => Filename);
            }
        }

        private string _name;
        public virtual string Name
        {
            get
            {
                return _name;
            }
            set
            {
                SetValue(ref _name, value, () => Name);
            }
        }

        private string _extension;
        public virtual string Extension
        {
            get
            {
                return _extension;
            }
            set
            {
                SetValue(ref _extension, value, () => Extension);
            }
        }

        private long _fileSize;
        public virtual long FileSize
        {
            get
            {
                return _fileSize;
            }
            set
            {
                SetValue(ref _fileSize, value, () => FileSize);
            }
        }

        public string FileSizeDescription
        {
            get
            {
                var units = new List<string>() { "Bytes", "KB", "MB", "GB", "TB" };

                double pow = Math.Floor((FileSize > 0 ? Math.Log(FileSize) : 0) / Math.Log(1024));
                pow = Math.Min(pow, units.Count - 1);

                double value = (double)FileSize / Math.Pow(1024, pow);
                return value.ToString(pow == 0 ? "F0" : "F2") + " " + units[(int)pow];
            }
        }

        private DateTime _dateModified;
        public virtual DateTime DateModified
        {
            get
            {
                return _dateModified;
            }
            set
            {
                SetValue(ref _dateModified, value, () => DateModified);
            }
        }

        public void Start()
        {
            if (System.IO.File.Exists(Filename))
            {
                try
                {
                    Process.Start(new ProcessStartInfo(Filename) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    ServiceManager.Default.Resolve<IWindowManager>().GetAnswer("Execution Error", ex.Message, new OkResult());
                }
            }
        }
    }
}
