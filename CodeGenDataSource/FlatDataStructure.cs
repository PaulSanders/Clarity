using System;
using System.Collections.Generic;

namespace CodeGenDataSource
{
    public class FlatDataStructure : IDataStructure
    {
        public FlatDataStructure(string data)
        {
            if (string.IsNullOrEmpty(data)) throw new ArgumentNullException("data");
            ParseData(data);
        }

        private void ParseData(string data)
        {
            var separators = new[] { ',', '\t', ';', ':', ' ' };

            foreach (var s in separators)
                if (data.IndexOf(s) != -1)
                {
                    var parts = data.Split(s);
                    foreach (var part in parts)
                    {
                        _items.Add(new FlatDataItem(part));
                    }
                    break;
                }
        }

        public string Name { get; set; }

        private List<IDataItem> _items = new List<IDataItem>();
        public IEnumerable<IDataItem> GetItems(IDataItem parent = null)
        {
            return _items;
        }
    }

    public class FlatDataItem : BaseDataItem
    {
        public FlatDataItem(string data)
        {
            Name = data;
            Properties.Add("Name", data);
        }
    }

}
