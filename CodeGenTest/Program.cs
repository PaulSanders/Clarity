using System;
using System.Collections.Generic;
using System.Diagnostics;
using CodeGenDataSource;
using System.Data.SqlClient;

namespace CodeGenTest
{
    class Program
    {
        static void Main(string[] args)
        {
            TestSimpleData();
			//TestDB();
            Console.ReadLine();            
        }

        static void TestSimpleData()
        {
            var processor = new FlatStructureProcessor();
            var source = new FlatDataStructure("Access,Sql,Informix,Oracle,Ingres,Sybase,Db2,DBase");
            source.Name = "DataBaseTypes";
            processor.Process(source, new EnumTemplate(), new DebugWriter());
        }

		//static void TestDB()
		//{
		//	var processor = new DatabaseStructureProcessor();
		//	var conn = new SqlConnection("Server=localhost;Database=ReportServer;Trusted_Connection=True;");
		//	conn.Open();
		//	var source = new DatabaseStructure(conn);
		//	processor.Process(source, new DatabaseTemplate(), new DebugWriter());
		//}
    }

    class EnumTemplate : Template
    {
        public override string Data
        {
            get
            {   
                return @"public enum @structurename
{
@each    #item,
@endeach}";
            }
        }

        public override IEnumerable<string> Tags
        {
            get
            {
                return new[] { "#item" };
            }
        }
    }

    class DebugWriter : IDataWriter
    {
        public void WriteLine(string line)
        {
            Debug.WriteLine(line);
        }


        public void Write(string data)
        {
            Console.Write(data);
            Console.WriteLine("");
        }
    }

}
