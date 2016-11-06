using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLTool.Model
{
    public class DependencyModel
    {
        public string RERERENCING_TABLE_NAME { get; set; }
        public string RERERENCING_COLUMN_NAME { get; set; }
        public string RERERENCING_COLUMN_DATA_TYPE { get; set; }
        public string REFERENCED_TABLE_NAME { get; set; }
        public string REFERENCED_COLUMN_NAME { get; set; }
        public string REFERENCED_COLUMN_DATA_TYPE { get; set; }
        public string ResultXML { get; set; }

    }
}
