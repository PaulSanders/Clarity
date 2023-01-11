
namespace CodeGenDataSource
{
    public interface IStructureProcessor
    {
        void Process(IDataStructure source, Template template, IDataWriter writer);
    }
}
