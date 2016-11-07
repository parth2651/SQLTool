using SQLTool.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Xml.Serialization;
using System.Diagnostics;

namespace SQLTool
{
    public class ProcessRequest
    {
        FileHelper fHelper;
        List<Model.DependencyModel> DependencyModelList;
        List<Model.TablePathModel> TablePathModelList;
        List<Model.TableMainModel> objMainTableModel;
        AdjacencyList<string> adjacencyList = null;
        List<string> TraversedPath = new List<string>();
        List<Model.TablePathModel> PendingPathtoTravers = new List<Model.TablePathModel>();
        bool ContinueTraversingCheck = false;

        public ProcessRequest(List<Model.DependencyModel> DependencyModelListParent, List<Model.TablePathModel> TablePathModelListParent)
        {
            fHelper = new FileHelper();
            this.TablePathModelList = TablePathModelListParent;
            this.DependencyModelList = DependencyModelListParent;
            this.objMainTableModel = new List<Model.TableMainModel>();
        }

        public List<Model.TablePathModel> GetRequestedPath()
        {

            //List<Model.TablePathModel> uniqTablePath = SQLToolMain.TablePathModelList.Where(x => x.PathID == 
            //find uniq path with input table name
            //i.e. all the path with table name is x
            List<Model.TablePathModel> uniqTablePath = this.TablePathModelList.Where(y => y.TableName == SQLToolMain.InputTable).ToList();

            //use list of path found in above line
            //get all the path where pathID from above line
            //retun all table from the path
            List<Model.TablePathModel> requestedPaths = this.TablePathModelList.Join(uniqTablePath, x => x.PathID, y => y.PathID, (x, y) => x).ToList();
            return requestedPaths;
        }

        public List<Model.TableMainModel> GenerateQueries(List<Model.TablePathModel> requestedPath)
        {
            InitiateMainObject(requestedPath);

            RegExHelper re = new RegExHelper(@"(?<left>.*)\/"+SQLToolMain.InputTable.Replace(".","\\.")+"\\/(?<right>.*)");
            List<Model.TablePathModel> dividedPath;
            List<int> uniquePath = requestedPath.Select(x => x.PathID).Distinct().ToList();

            


            for (int i = 0; i < uniquePath.Count; i++)
            {


                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine("Looping " +(i + 1).ToString() + "/" + uniquePath.Count);
                Console.WriteLine("");

                dividedPath = new List<Model.TablePathModel>();
                dividedPath = this.TablePathModelList.Where(x => x.PathID == uniquePath[i]).OrderBy(y => y.Sequence).ToList();
                this.PendingPathtoTravers = dividedPath;

                this.ContinueTraversingCheck = i == 0 ? true : false; //stop
                if (dividedPath.Count > 0)
                {

                    Vertex<string> startingVertex = new Vertex<string>(SQLToolMain.InputTable);
                    this.adjacencyList = new AdjacencyList<string>(startingVertex, this.OnVertexTraverse);
                    

                    List<string> l = re.GetMatchList(dividedPath[0].EntirePath);
                    if (l.Count == 2)
                    {
                        string[] leftTableNamesArray = l[0].Split(new char[] { '/' });

                        // Reverse the left table names to convert from right-to-left to left-to-right
                        // which UpdateGraph uses.
                        var leftTableNamesList = leftTableNamesArray.Reverse().ToList();

                        string[] rightTableNamesArray = l[1].Split(new char[] { '/' });

                        var rightableNamesList = rightTableNamesArray.ToList();

                        List<string> combinedList = new List<string>(leftTableNamesList.Count + rightableNamesList.Count);

                        combinedList.AddRange(rightableNamesList);
                        combinedList.AddRange(leftTableNamesList);

                        this.adjacencyList.AddPath(combinedList);
                    }
                    
                    this.adjacencyList.Traverse();
                     

                     
                    //list<string> with parent chilf
                    //start with last parent chhild in current path
                    //see if is already exists in traversed path
                    //if exists then skip and continue 
                    //if not add value in traversed path
                    //set some flag so traversed again 

                    //List<string> TraversedTable = new List<string>();
                    //TraversedTable.AddRange(TraversedPath.Select(x=>x.TableName).Except(dividedPath.Select(y=>y.TableName).ToList()));

                    ////find table from divided path
                    ////get max index of table
                    ////and start divided path from there

                    //TraversedPath.AddRange(dividedPath);


                   
                    CreateQueries(this.PendingPathtoTravers);
                    
                }
                Console.WriteLine("--------------------------------------------------");

            }
#if DEBUG
            string xmlOutput = XMLConversionHelper.Serialize(this.objMainTableModel);

#endif

            return this.objMainTableModel;

        }


        private void OnVertexTraverse(List<string> path)
        {
            string newpath = string.Join("/", path);
            //path
            //     .ForEach(
            //         (s) =>
            //         {
            //             Debug.Write(@"/" + s);
                         
            //         }
            //    );
            if (!this.TraversedPath.Exists(x => x == newpath))
            {
                
                this.TraversedPath.Add(newpath);
                if (!this.ContinueTraversingCheck)
                {
                    Model.TablePathModel modal = this.PendingPathtoTravers.Where(x => x.TableName == path[path.Count - 2]).FirstOrDefault();
                    List<Model.TablePathModel> PathsToRemove = this.PendingPathtoTravers.Where(x => x.Sequence > modal.Sequence).ToList();
                    PathsToRemove.ForEach(
                     (m) =>
                     {
                         this.PendingPathtoTravers.Remove(m);

                     });
                    this.ContinueTraversingCheck = !this.ContinueTraversingCheck; //change the flag
                }
            }
            
        }
        private void InitiateMainObject(List<Model.TablePathModel> requestedPath)
        {
            List<string> listOFuniqueTable = requestedPath.Select(x => x.TableName).Distinct().ToList();
            for (int i = 0; i < listOFuniqueTable.Count; i++)
            {
                this.objMainTableModel.Add(new Model.TableMainModel { TableName = listOFuniqueTable[i] });

            }
        }

        private void CreateQueries(List<Model.TablePathModel> dividedPath)
        {
            //traverse till input table
            //use passed parameter to generate query
            //generate sucessesor query first and then traverse to top
            Model.TablePathModel tPath = dividedPath.FirstOrDefault(x => x.TableName == SQLToolMain.InputTable);
            object[] objInputvalue = { SQLToolMain.InputValue };
            //execute if tpath is not null (do not execute if input table not exists in path)
            if (tPath != null)
            {
                #region traverse right side of input

                for (int i = tPath.Sequence - 1; i < dividedPath.Count; i++) //sequence start from 1
                {
                    string[] spliter = { "p" };

                    if (dividedPath[i].TableName == SQLToolMain.InputTable)
                        upsertMainTableObject(dividedPath[i].TableName, string.Format("select * from {0} {1}", dividedPath[i].TableName, SQLToolMain.InputWhereCondition), SQLToolMain.InputWhereColumn, objInputvalue);
                    else
                    {
                        //get preivous table
                        //use current table
                        //get dependency between tables
                        //run select depedency column from previous table query 
                        //use value in current query

                        string ParentTable = dividedPath[i - 1].TableName;
                        string CurrentTable = dividedPath[i].TableName;
                        List<Model.DependencyModel> currentDepedency = this.DependencyModelList.Where(x => (x.RERERENCING_TABLE_NAME == ParentTable) && (x.REFERENCED_TABLE_NAME == CurrentTable)).ToList();
                        for (int j = 0; j < currentDepedency.Count; j++)
                        {
                            string selectColumnFromParent = currentDepedency[j].RERERENCING_COLUMN_NAME;
                            string selectColumnFromParentDataType = currentDepedency[j].RERERENCING_COLUMN_DATA_TYPE;
                            string whereColumnFromChild = currentDepedency[j].REFERENCED_COLUMN_NAME;
                            Model.TableMainModel parentModel = this.objMainTableModel.Find(x => x.TableName == ParentTable);
                            if (parentModel.SelectStatements.Count > 0)
                            {
                                for (int k = 0; k < parentModel.SelectStatements.Count; k++)
                                {
                                    string query = parentModel.SelectStatements[k].Query;
                                    query = query.Replace("*", selectColumnFromParent + " AS OUTPUTCOLUMN");
                                    //////execute query
                                    //////to do get multiple value with comma seperated (check type and use comma seperation)
                                    ////string columnValue = GetValuefromQuery(query, selectColumnFromParentDataType);
                                    //////string columnValue = Convert.ToString(SqlHelper.ExecuteScalar(Configuration.Configuration.ConnectionString, CommandType.Text, query));
                                    ////if (!string.IsNullOrEmpty(columnValue)) // do not add query if return value is 0
                                    ////    upsertMainTableObject(CurrentTable, string.Format("select * from {0} (nolock) where {1} in ({2})", CurrentTable, whereColumnFromChild, columnValue));

                                    //execute query
                                    //to do get multiple value with comma seperated (check type and use comma seperation)
                                    object[] columnValue = GetValuefromQuery(query, selectColumnFromParentDataType);
                                    //string columnValue = Convert.ToString(SqlHelper.ExecuteScalar(Configuration.Configuration.ConnectionString, CommandType.Text, query));
                                    if (columnValue != null && !(columnValue.Length == 1 && columnValue[0] == null)) // do not add query if return value is 0
                                        upsertMainTableObject(CurrentTable, string.Format("select * from {0} (nolock) where {1} in ({2})", CurrentTable, whereColumnFromChild, string.Join(",", columnValue)), whereColumnFromChild, columnValue);
                                }
                            }
                        }
                    }
                }

                #endregion
            }

            //execute if tpath is null (fill tpath as last table in the dividedPath list )
            if (tPath == null)
            {
                tPath = dividedPath.LastOrDefault();
            }

            #region traverse Left side of input

            for (int i = tPath.Sequence - 1; i > 0; i--) //reverse loop input table -1 to top one
            {

                //get the input table and its ID again
                //get the parent table 
                //get the relationship 
                //generate the query 
                string CurrentTable = dividedPath[i].TableName;
                string ParentTable = dividedPath[i - 1].TableName;
                List<Model.DependencyModel> currentDepedency = this.DependencyModelList.Where(x => (x.RERERENCING_TABLE_NAME == ParentTable) && (x.REFERENCED_TABLE_NAME == CurrentTable)).ToList();

                for (int j = 0; j < currentDepedency.Count; j++)
                {
                    string selectColumnFromParent = currentDepedency[j].RERERENCING_COLUMN_NAME;
                    string selectColumnFromParentDataType = currentDepedency[j].RERERENCING_COLUMN_DATA_TYPE;
                    string whereColumnFromChild = currentDepedency[j].REFERENCED_COLUMN_NAME;
                    Model.TableMainModel childModel = this.objMainTableModel.Find(x => x.TableName == CurrentTable);
                    //expecting childmodel will not be null 
                    if (childModel.SelectStatements == null)
                    {
                        continue;
                    }
                    if (childModel.SelectStatements.Count > 0)
                    {
                        for (int k = 0; k < childModel.SelectStatements.Count; k++)
                        {
                            string query = childModel.SelectStatements[k].Query;
                            query = query.Replace("*", whereColumnFromChild + " AS OUTPUTCOLUMN");
                            //////execute query
                            ////string columnValue = GetValuefromQuery(query, selectColumnFromParentDataType);
                            //////string columnValue = Convert.ToString(SqlHelper.ExecuteScalar(Configuration.Configuration.ConnectionString, CommandType.Text, query));
                            ////if (!string.IsNullOrEmpty(columnValue)) // do not add query if return value is 0
                            ////    upsertMainTableObject(ParentTable, string.Format("select * from {0} (nolock) where {1} in ({2})", ParentTable, selectColumnFromParent, columnValue),);

                            //execute query
                            object[] columnValue = GetValuefromQuery(query, selectColumnFromParentDataType);
                            //string columnValue = Convert.ToString(SqlHelper.ExecuteScalar(Configuration.Configuration.ConnectionString, CommandType.Text, query));
                            if (columnValue != null)  // do not add query if return value is 0
                                upsertMainTableObject(ParentTable, string.Format("select * from {0} (nolock) where {1} in ({2})", ParentTable, selectColumnFromParent, string.Join(",", columnValue)), whereColumnFromChild, columnValue);
                        }
                    }
                }


                //if (dividedPath[i].TableName == SQLToolMain.InputTable)
                //    upsertMainTableObject(dividedPath[i].TableName, string.Format("select * from {0} {1}", dividedPath[i].TableName, SQLToolMain.InputWhereCondition));
                //else
                //upsertMainTableObject(dividedPath[i].TableName, string.Format("select * from {0} {1}", dividedPath[i].TableName, SQLToolMain.InputWhereCondition));
            }

            #endregion

        }

        private object[] GetValuefromQuery(string query, string selectColumnFromParentDataType)
        {
            DataSet dsOutPutValue = new DataSet();
            string result = string.Empty;
            object[] outPutlist = null;
            string[] str = { "OUTPUT" };
            SqlHelper.FillDataset(Configuration.Configuration.ConnectionString, CommandType.Text, query, dsOutPutValue, str);
            if (dsOutPutValue != null && dsOutPutValue.Tables[0] != null && dsOutPutValue.Tables[0].Rows.Count > 0)
            {
                //if (selectColumnFromParentDataType.ToLower().Contains("int"))
                //{
                outPutlist = dsOutPutValue.Tables[0].AsEnumerable().Select(r => r.Field<object>("OUTPUTCOLUMN")).ToArray();
                //result = string.Join(",", outPutlist);
                //}
                //else
                //{
                //    //to do need to test
                //    var outPutlist = dsOutPutValue.Tables[0].AsEnumerable().Select(r => "'"+r.Field<string>("OUTPUTCOLUMN") +"'").ToArray();
                //    result = string.Join(",", outPutlist);
                //}

            }

            //return outPutlist;
            return outPutlist;
        }


        private void upsertMainTableObject(string TableName, string Query, string whereColumn, object[] whereValues)
        {

            Model.TableMainModel model = this.objMainTableModel.Find(x => x.TableName == TableName);

            //see if model is null
            //  add table name and data in model
            //if model is not null
            //  add the sql statements to the model
            //  add the max of sequence
            if (model == null) //add table
            {
                model = new Model.TableMainModel { TableName = TableName };
                this.objMainTableModel.Add(model);
            }

            //adding ColumnvaluePair
            if (model.ColumnValuePair == null)
            {
                model.ColumnValuePair = new List<Model.ColumnValuePair>();
                model.ColumnValuePair.Add(new Model.ColumnValuePair { ColumnName = whereColumn, WhereValues = whereValues.ToList() });
            }
            else
            {
                Model.ColumnValuePair existingPair = model.ColumnValuePair.Find(x => x.ColumnName == whereColumn);
                if (existingPair != null)
                {
                    //model.ColumnValuePair.Any(x => x.WhereValues == whereValues)
                    //var result = model.ColumnValuePair.Intersect(whereValues);
                    List<object> obj = whereValues.Except(existingPair.WhereValues).ToList();
                    if (obj != null && obj.Count > 0)
                    {
                        existingPair.WhereValues.AddRange(obj);
                    }
                }
                else
                {
                    model.ColumnValuePair.Add(new Model.ColumnValuePair { ColumnName = whereColumn, WhereValues = whereValues.ToList() });
                }
            }


            if (model.SelectStatements == null)
            {
                model.SelectStatements = new List<Model.SelectStatementModel>();
            }

            //validate same query already exists or not
            if (model.SelectStatements.Count(x => x.Query == Query) > 0)
                return;

            int maxSequence = model.SelectStatements.Count == 0 ? 0 : model.SelectStatements.Max(t => t.Sequence);
            model.SelectStatements.Add(new Model.SelectStatementModel { Query = Query, Sequence = maxSequence + 1 });


        }


    }
}
