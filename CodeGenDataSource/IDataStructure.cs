using System.Collections.Generic;

namespace CodeGenDataSource
{
    public interface IDataStructure
    {
        string Name { get; set; }
        IEnumerable<IDataItem> GetItems(IDataItem parent = null);
    }
}
