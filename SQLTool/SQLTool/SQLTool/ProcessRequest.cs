using SQLTool.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace SQLTool
{
    public class ProcessRequest
    {
        FileHelper fHelper;
        public ProcessRequest()
        {
            fHelper = new FileHelper();
        }

        public List<Model.DependencyModel> GetDependency()
        {
            string commandText = fHelper.ReadFile(ToolConstants.Script.DependencyColumns);
            DataSet dsDependency = SqlHelper.ExecuteDataset(Configuration.Configuration.ConnectionString, System.Data.CommandType.Text, commandText);
            return FillDependencyModel(dsDependency);
        }

        private List<Model.DependencyModel> FillDependencyModel(DataSet dsDependency)
        {
            List<Model.DependencyModel> Dependency = new List<Model.DependencyModel>();
            if(dsDependency!=null && dsDependency.Tables[0]!=null && dsDependency.Tables[0].Rows.Count>0)
            {
                for(int i =0;i<dsDependency.Tables[0].Rows.Count;i++)
                {
                    Dependency.Add(new Model.DependencyModel
                    {
                        RERERENCING_TABLE_NAME = Convert.ToString(dsDependency.Tables[0].Rows[i][ToolConstants.Variables.colRERERENCING_TABLE_NAME]),
                        RERERENCING_COLUMN_NAME = Convert.ToString(dsDependency.Tables[0].Rows[i][ToolConstants.Variables.colRERERENCING_COLUMN_NAME]),
                        REFERENCED_TABLE_NAME = Convert.ToString(dsDependency.Tables[0].Rows[i][ToolConstants.Variables.colREFERENCED_TABLE_NAME]),
                        REFERENCED_COLUMN_NAME = Convert.ToString(dsDependency.Tables[0].Rows[i][ToolConstants.Variables.colREFERENCED_COLUMN_NAME])
                    });
                }
            }
            return Dependency;
        }

        public List<Model.TablePathModel> GetTablePath()
        {
            string commandText = fHelper.ReadFile(ToolConstants.Script.TablePath);
            DataSet dsTablePath = SqlHelper.ExecuteDataset(Configuration.Configuration.ConnectionString, System.Data.CommandType.Text, commandText);
            return FillTablePathModel(dsTablePath);
            
        }

        private List<Model.TablePathModel> FillTablePathModel(DataSet dsTablePath)
        {
            List<Model.TablePathModel> TablePath = new List<Model.TablePathModel>();
            if (dsTablePath != null && dsTablePath.Tables[0] != null && dsTablePath.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsTablePath.Tables[0].Rows.Count; i++)
                {
                    TablePath.Add(new Model.TablePathModel
                    {
                        TableName = Convert.ToString(dsTablePath.Tables[0].Rows[i][ToolConstants.Variables.colTHEPATH]),
                    });
                }
            }
            return TablePath;
        }
    }
}
