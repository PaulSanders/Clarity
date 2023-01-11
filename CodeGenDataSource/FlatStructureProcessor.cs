using System.Linq;

namespace CodeGenDataSource
{
    public class FlatStructureProcessor : IStructureProcessor
    {
        public void Process(IDataStructure source, Template template, IDataWriter writer)
        {
            writer.WriteLine(source.Name);
            var tags = template.Tags.ToList();
            var data = template.Data.Replace("@structurename", source.Name);

            data = ExpandRepeatSections(data, source, template);

            foreach (var tag in template.Tags)
            {
                foreach (var item in source.GetItems())
                {
                    data = data.Replace(tag, item.Name);
                }
            }
            writer.Write(data);
        }

        private string ExpandRepeatSections(string data, IDataStructure source, Template template)
        {
            var text = data;
            var tags = template.Tags.ToList();

            while (data.IndexOf("@each") != -1)
            {
                string prefix, suffix, repeatData;
                GetRepeatSection(text, out prefix, out suffix, out repeatData);
                var fixedData = repeatData;
                var genData = "";
                foreach (var item in source.GetItems())
                {
                    repeatData = fixedData;
                    foreach (var tag in tags)
                    {
                        if (repeatData.Contains(tag))
                            genData += repeatData.Replace(tag, item.Name);
                    }
                }
                data = prefix + genData + suffix;
            }

            return data;
        }

        private void GetRepeatSection(string data, out string prefix, out string suffix, out string repeatData)
        {
            var index = data.IndexOf("@each");
            if (index == -1)
            {
                prefix = data;
                suffix = "";
                repeatData = "";
            }
            else
            {
                prefix = data.Substring(0, index);

                var endpos = data.IndexOf("@endeach", index);
                suffix = data.Substring(endpos + "@endeach".Length);

                var length = endpos - index - "@each".Length;
                repeatData = data.Substring(index + "@each".Length, length);
            }
        }
    }
}
