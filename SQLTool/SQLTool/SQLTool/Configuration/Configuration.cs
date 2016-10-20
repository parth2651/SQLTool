using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
namespace SQLTool.Configuration
{
    public class Configuration
    {

        private static string _connectionString = ConfigurationManager.ConnectionStrings[ToolConstants.Connection.MainConnection].ConnectionString;

        public static string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        public static string GetScriptDirectory()
        {
            return ConfigurationManager.AppSettings[ToolConstants.AppSettings.ScriptDirectory].ToString();
        }
    }
}
