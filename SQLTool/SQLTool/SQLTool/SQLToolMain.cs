using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SQLTool.Helper;
//using SQLToolConfiguration = SQLTool.Configuration;

namespace SQLTool
{

    class SQLToolMain
    {

        #region properties
        public static string InputTable { get; set; }
        public static string InputWhereCondition { get; set; }

        public static List<Model.DependencyModel> DependencyModelList { get; set; }
        public static List<Model.TablePathModel> TablePathModelList{ get; set; }

        #endregion properties

        static void Main(string[] args)
        {
            //commenting for now
            ValidateParameter(args);
            Process();
        }

        private static void Process()
        {
            ProcessRequest req = new ProcessRequest();
            DependencyModelList = req.GetDependency();
            TablePathModelList = req.GetTablePath();
        }

        private static void ValidateParameter(string[] args)
        {
            InputTable = args[0];
            InputWhereCondition = args[1]; 
            //throw new NotImplementedException();
        }
    }
}
