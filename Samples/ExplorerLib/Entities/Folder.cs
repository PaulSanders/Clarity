using Clarity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ExplorerLib.Entities
{
    public class Folder : PropertyChangedBase
    {
        public Folder()
        {
            OnChangeOf(() => Path).Execute(UpdateInfo);
        }

        private void UpdateInfo()
        {
            Folders.Clear();
            Files.Clear();

            if (string.IsNullOrEmpty(Path))
            {
                Name = string.Empty;
                DateModified = null;
            }
            else
            {
                Name = Path.Split('\\').Last();
                DateModified = System.IO.Directory.GetCreationTime(Path);
            }
        }

        private string _path;
        public virtual string Path
        {
            get
            {
                return _path;
            }
            set
            {
                SetValue(ref _path, value, () => Path);
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

        private DateTime? _dateModified;
        public virtual DateTime? DateModified
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

        public void Refresh()
        {
            Folders.Clear();
            Files.Clear();
            if (System.IO.Directory.Exists(Path))
            {
                try
                {
                    Folders.AddRange(System.IO.Directory.EnumerateDirectories(Path).Select(f => new Folder() { Path = f }));

                    Files.AddRange(System.IO.Directory.EnumerateFiles(Path).Select(f => new File() { Filename = f }));
                }
                catch (Exception ex)
                {
                    ServiceManager.Default.Resolve<IWindowManager>().GetAnswer("Error", ex.Message, new OkResult());
                }
            }

            NotifyPropertyChanged(() => Children);
        }

        private ObservableCollection<Folder> _folders = new ObservableCollection<Folder>();
        public virtual ObservableCollection<Folder> Folders
        {
            get
            {
                return _folders;
            }
        }

        private ObservableCollection<File> _files = new ObservableCollection<File>();
        public virtual ObservableCollection<File> Files
        {
            get
            {
                return _files;
            }
        }

        public IEnumerable<PropertyChangedBase> Children
        {
            get
            {
                return _folders.Union<PropertyChangedBase>(Files);
            }
        }
    }
}
