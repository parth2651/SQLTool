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

        public List<Model.TablePathModel> GetRequestedPath()
        {

            //List<Model.TablePathModel> uniqTablePath = SQLToolMain.TablePathModelList.Where(x => x.PathID == 
            //find uniq path with input table name
            //i.e. all the path with table name is x
            List<Model.TablePathModel> uniqTablePath = SQLToolMain.TablePathModelList.Where(y => y.TableName == SQLToolMain.InputTable).ToList();
            
            //use list of path found in above line
            //get all the path where pathID from above line
            //retun all table from the path
            List<Model.TablePathModel> requestPath= SQLToolMain.TablePathModelList.Join(uniqTablePath,x=>x.PathID,y=>y.PathID,(x,y)=>x).ToList();
            return requestPath;
        }
        
    }
}
