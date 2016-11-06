using SQLTool.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace SQLTool
{
    public class PreRequest
    {
        FileHelper fHelper;

        public PreRequest()
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
                        RERERENCING_COLUMN_DATA_TYPE = Convert.ToString(dsDependency.Tables[0].Rows[i][ToolConstants.Variables.colRERERENCING_COLUMN_DATA_TYPE]),
                        REFERENCED_TABLE_NAME = Convert.ToString(dsDependency.Tables[0].Rows[i][ToolConstants.Variables.colREFERENCED_TABLE_NAME]),
                        REFERENCED_COLUMN_NAME = Convert.ToString(dsDependency.Tables[0].Rows[i][ToolConstants.Variables.colREFERENCED_COLUMN_NAME]),
                        REFERENCED_COLUMN_DATA_TYPE = Convert.ToString(dsDependency.Tables[0].Rows[i][ToolConstants.Variables.colREFERENCED_COLUMN_DATA_TYPE]),
                    });
                }
            }
            return Dependency;
        }

        //old method changed because of timeout issue for large database
        //public List<Model.TablePathModel> GetTablePath()
        //{
        //    string commandText = fHelper.ReadFile(ToolConstants.Script.TablePath);
        //    DataSet dsTablePath = SqlHelper.ExecuteDataset(Configuration.Configuration.ConnectionString, System.Data.CommandType.Text, commandText);
        //    return FillTablePathModel(dsTablePath);

        //}

        public List<Model.TablePathModel> GetTablePath()
        {
            string commandText = fHelper.ReadFile(ToolConstants.Script.TablePath);
            DataSet dsIteration = SqlHelper.ExecuteDataset(Configuration.Configuration.ConnectionString, System.Data.CommandType.Text, commandText);
            List<Model.TableIteration> IterationList = CreateIterationList(dsIteration);
            DataSet dsTablePath = CreatePathDataSet(IterationList);
            return FillTablePathModel(dsTablePath);

        }

        private DataSet CreatePathDataSet(List<Model.TableIteration> IterationList)
        {
            List<string> ThePathList = new List<string>();
            DataSet dsTablePath = new DataSet();
            DataTable dt = new DataTable();
            dt.Columns.Add(ToolConstants.Variables.colTHEPATH);
            dsTablePath.Tables.Add(dt);

            int maxIteration = IterationList.Max(x => x.Iteration);
            while (maxIteration >= 1)
            {
               List<Model.TableIteration> selectedIteration =  IterationList.Where(x => x.Iteration == maxIteration).ToList();
               for (int i = 0; i < selectedIteration.Count; i++)
               {
                   if(!ThePathList.Exists(x=>x.Contains(selectedIteration[i].ThePath)))
                   {
                       ThePathList.Add(selectedIteration[i].ThePath);
                   }
               }
               maxIteration--;

            }

            for (int i = 0; i < ThePathList.Count; i++)
            {
                DataRow row = dt.NewRow();
                row[ToolConstants.Variables.colTHEPATH] = ThePathList[i];
                dt.Rows.Add(row);
            }
            return dsTablePath;
        }

        private List<Model.TableIteration> CreateIterationList(DataSet dsIteration)
        {
            List<Model.TableIteration> IterationList = new List<Model.TableIteration>();
            if (dsIteration != null && dsIteration.Tables[0] != null && dsIteration.Tables[0].Rows.Count > 0)
            {
                
                for (int i = 0; i < dsIteration.Tables[0].Rows.Count; i++)
                {
                    IterationList.Add(new Model.TableIteration 
                    {
                        Iteration = Convert.ToInt32(dsIteration.Tables[0].Rows[i][ToolConstants.Variables.coliteration]),
                        TheFullEntityName = Convert.ToString(dsIteration.Tables[0].Rows[i][ToolConstants.Variables.colTheFullEntityName]),
                        ThePath = Convert.ToString(dsIteration.Tables[0].Rows[i][ToolConstants.Variables.colTHEPATH]),
                    });


                }
            }
            return IterationList;
        }

        private List<Model.TablePathModel> FillTablePathModel(DataSet dsTablePath)
        {
            List<Model.TablePathModel> TablePath = new List<Model.TablePathModel>();
            if (dsTablePath != null && dsTablePath.Tables[0] != null && dsTablePath.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsTablePath.Tables[0].Rows.Count; i++)
                {
                    int PathID = i + 1;//same as data row of unique path
                    string tablePath = Convert.ToString(dsTablePath.Tables[0].Rows[i][ToolConstants.Variables.colTHEPATH]);
                    List<string> splittedList = tablePath.Split('/').ToList();

                    for (int j = 0; j < splittedList.Count; j++)
                    {
                        TablePath.Add(new Model.TablePathModel
                        {
                            PathID = PathID,
                            Sequence = j + 1,
                            TableName = splittedList[j],
                            EntirePath = tablePath,
                        });
                    }

                }
            }
            return TablePath;
        }
    }
}
