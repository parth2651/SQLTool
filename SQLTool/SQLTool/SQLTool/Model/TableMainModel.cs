﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLTool.Model
{
    public class TableMainModel
    {
        public string TableName { get; set; }
        public List<SelectStatementModel> SelectStatements { get; set; }

    }
    public class SelectStatementModel
    {
        public string Query { get; set; }
        public int Sequence { get; set; }
        public string ResultXML { get; set; }
    }

}
