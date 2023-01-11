using System.Collections.Generic;

namespace CodeGenDataSource
{
    public abstract class BaseDataItem : IDataItem
    {
        public BaseDataItem()
        {
            _properties = new Dictionary<string, string>();
        }

        public string Name { get; set; }
        public string ItemType { get; set; }
        public IDataItem Parent { get; set; }

        private Dictionary<string, string> _properties;
        public Dictionary<string, string> Properties
        {
            get { return _properties; }
        }

        private IList<IDataItem> _children;
        public IList<IDataItem> Children
        {
            get
            {
                if (_children == null)
                    _children = OnGetChildren();

                return _children;
            }
        }

        protected virtual IList<IDataItem> OnGetChildren()
        {
            return null;
        }
    }
}
