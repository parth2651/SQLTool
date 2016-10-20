using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLTool
{
    public static class ToolConstants
    {
        public static class Connection
        {
            public static string MainConnection = "MainConnection";
        }
        public static class AppSettings {
            public static string ScriptDirectory = "ScriptDirectory";
        }
        public static class Script
        {
            public static string TablePath = "TablePath.sql";
            public static string DependencyColumns = "IT_Depends_Column.sql";
        }

        public static class Variables
        {
            public static string colRERERENCING_TABLE_NAME = "RERERENCING_TABLE_NAME";
            public static string colRERERENCING_COLUMN_NAME ="RERERENCING_COLUMN_NAME";
            public static string colREFERENCED_TABLE_NAME ="REFERENCED_TABLE_NAME";
            public static string colREFERENCED_COLUMN_NAME ="REFERENCED_COLUMN_NAME";

            public static string colTHEPATH = "ThePath";
        }
    }
}
