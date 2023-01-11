using System.Collections.Generic;
using System.Data;

namespace CodeGenDataSource
{
	public class DatabaseStructure : IDataStructure
	{
		private IDbConnection _connection;

		public DatabaseStructure(IDbConnection connection)
		{
			_connection = connection;
			Name = _connection.Database;
		}

		public string Name { get; set; }

		public IEnumerable<IDataItem> GetItems(IDataItem parent = null)
		{
			if (parent == null)
				return GetTables();
			else if (parent is TableDataItem)
				return parent.Children;

			return null;
		}

		private IEnumerable<IDataItem> GetTables()
		{
			var sql = "select table_name from information_schema.tables order by table_name";
			using (var cmd = _connection.CreateCommand())
			{
				cmd.CommandText = sql;

				using (var reader = cmd.ExecuteReader())
				{
					var list = new List<IDataItem>();
					while (reader.Read())
					{
						var cdi = new TableDataItem(_connection);
						cdi.Name = reader.GetString(0);
						cdi.Properties["Name"] = cdi.Name;

						list.Add(cdi);
					}

					return list;
				}
			}
		}
	}

	public class TableDataItem : BaseDataItem
	{
		private IDbConnection _connection;
		public TableDataItem(IDbConnection connection)
		{
			_connection = connection;
		}


		protected override IList<IDataItem> OnGetChildren()
		{
			var sql = "select column_name, data_type, is_nullable from information_schema.columns where table_name = @p1";
			using (var cmd = _connection.CreateCommand())
			{
				cmd.CommandText = sql;
				var p = cmd.CreateParameter();
				p.ParameterName = "@p1";
				p.Value = Name;
				cmd.Parameters.Add(p);

				using (var reader = cmd.ExecuteReader())
				{
					var list = new List<IDataItem>();
					while (reader.Read())
					{
						var cdi = new ColumnDataItem(this);
						cdi.Name = reader.GetString(0);
						cdi.Properties["#column_name"] = cdi.Name;
						cdi.Properties["#column_type"] = reader.GetString(1);
						cdi.Properties["#nullable"] = reader.GetString(2).Replace("YES", "true").Replace("NO", "false");

						list.Add(cdi);
					}

					return list;
				}
			}
		}
	}

	public class ColumnDataItem : BaseDataItem
	{
		public ColumnDataItem(TableDataItem parent)
		{
			Parent = parent;
		}
	}
}
