using System.Collections.Generic;
using System.Linq;

namespace CodeGenDataSource
{
	public class DatabaseStructureProcessor : IStructureProcessor
	{
		public DatabaseStructureProcessor()
		{
		}

		public void Process(IDataStructure source, Template template, IDataWriter writer)
		{
			writer.WriteLine(source.Name);
			var tags = template.Tags.ToList();
			var data = template.Data.Replace("@structurename", source.Name);

			data = ExpandRepeatSections(data, source.GetItems(), template);

			foreach (var tag in template.Tags)
			{
				foreach (var item in source.GetItems())
				{
					data = data.Replace(tag, item.Name);
				}
			}

			writer.Write(data);
		}

		private string ExpandRepeatSections(string data, IEnumerable<IDataItem> items, Template template)
		{
			var text = data;
			var tags = template.Tags.ToList();

			while (data.IndexOf("@eachtable") != -1)
			{
				string prefix, suffix, repeatData;
				GetRepeatSection("@eachtable", text, out prefix, out suffix, out repeatData);
				var genData = "";

				if (repeatData.IndexOf("@eachcolumn") != -1)
				{
					repeatData = ExpandRepeatSections(repeatData, items.SelectMany(i => i.Children), template);
				}

				var fixedData = repeatData;
				foreach (var item in items)
				{
					repeatData = fixedData;
					foreach (var tag in tags)
					{
						if (repeatData.Contains(tag))
						{
							genData += repeatData.Replace(tag, item.Name);
						}
						else
						{
							foreach(var child in item.Children)
							{
								foreach(var prop in child.Properties)
								{
									if(repeatData.Contains(prop.Key))
									{
										genData += repeatData.Replace(tag, prop.Value);
									}
								}
							}
						}
					}
				}
				data = prefix + genData + suffix;
			}

			return data;
		}

		private void GetRepeatSection(string each, string data, out string prefix, out string suffix, out string repeatData)
		{
			var index = data.IndexOf(each);
			if (index == -1)
			{
				prefix = data;
				suffix = "";
				repeatData = "";
			}
			else
			{
				prefix = data.Substring(0, index);

				var endpos = data.LastIndexOf("@endeach");
				suffix = data.Substring(endpos + "@endeach".Length);

				var length = endpos - index - each.Length;
				repeatData = data.Substring(index + each.Length, length);
			}
		}
	}
}
