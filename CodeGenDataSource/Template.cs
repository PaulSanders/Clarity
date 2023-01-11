using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeGenDataSource
{
    /*
     Tags=@name,@item
     */

    public class Template
    {
        private string[] _templateLines;
        public Template()
        {

        }

        public Template(string filename)
        {
            _templateLines = File.ReadAllLines(filename);
        }

        private string _data;
        public virtual string Data
        {
            get
            {
                if (_data == null)
                {
                    var sb = new StringBuilder();
                    foreach (var line in _templateLines)
                        sb.AppendLine(line);

                    _data = sb.ToString();
                }

                return _data;
            }
        }

        public virtual IEnumerable<string> Tags
        {
            get
            {
                foreach (var line in _templateLines)
                    if (line.StartsWith("Tags="))
                    {
                        return line.Substring("Tags=".Length).Split(',');
                    }

                return new[] { "" };
            }
        }
    }
}
