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

        public static string InputValue { get; set; }
        public static string InputWhereColumn { get; set; }

        public static List<Model.DependencyModel> DependencyModelList { get; set; }
        public static List<Model.TablePathModel> TablePathModelList{ get; set; }

        public static List<Model.TablePathModel> RequestedTablePathModelList { get; set; }

        #endregion properties

        static void Main(string[] args)
        {
            //commenting for now
            ValidateParameter(args);

            //start the process
            Process();
        }

        /// <summary>
        /// Process 
        /// </summary>
        private static void Process()
        {
            //define pre requisite 
            PreRequest preReq = new PreRequest();
            //get all depedency from database
            Console.WriteLine("Getting Dependency - started");
            DependencyModelList = preReq.GetDependency();
            Console.WriteLine("Getting Dependency - completed");
            
            //get all unique path (tree) from database
            Console.WriteLine("Getting All Table Path - started");
            TablePathModelList = preReq.GetTablePath();
            Console.WriteLine("Getting All Table Path - completed");
            
            //start processing 
            ProcessRequest req = new ProcessRequest(DependencyModelList, TablePathModelList);
            RequestedTablePathModelList  = req.GetRequestedPath();
            List<Model.TableMainModel>  objMainTableModel = req.GenerateQueries(RequestedTablePathModelList);

        }

        private static void ValidateParameter(string[] args)
        {
            InputTable = args[0];
            InputWhereCondition = args[1];

            InputWhereColumn = args[2];
            InputValue = args[3];
            //throw new NotImplementedException();
        }
    }
}
