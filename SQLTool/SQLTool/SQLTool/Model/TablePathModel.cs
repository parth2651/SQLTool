using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLTool.Model
{
    public class TablePathModel
    {
        public int PathID{ get; set; }
        public string TableName { get; set; }
        public int Sequence { get; set; }
        public string EntirePath { get; set; }

    }
    
}
