using System.Collections.Generic;

namespace CodeGenDataSource
{
    public interface IDataItem
    {
        string Name { get; set; }
        string ItemType { get; set; }

        Dictionary<string, string> Properties { get; }

        IDataItem Parent { get; set; }
        IList<IDataItem> Children { get; }
    }
}
