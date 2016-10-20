using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLTool.Helper
{
    class FileHelper
    {

        public string ScriptDirectory { get; private set; }

        public FileHelper()
        {
            ScriptDirectory = Configuration.Configuration.GetScriptDirectory();
        }

        public string ReadFile(string FileName)
        {
            DirectoryInfo di = new DirectoryInfo(ScriptDirectory);
            //          FileInfo[] rgFiles = di.GetFiles("*.sql");
            //foreach (FileInfo fi in rgFiles)
            //{
            //      FileInfo fileInfo = new FileInfo(fi.FullName);
            //      string script = fileInfo.OpenText().ReadToEnd();
            //      using (SqlConnection connection = new SqlConnection(sqlConnectionString))
            //      {
            //          Server server = new Server(new ServerConnection(connection));
            //          server.ConnectionContext.ExecuteNonQuery(script);
            //      }
            // }
            string fileContent= String.Empty;

            using (StreamReader sr = File.OpenText(ScriptDirectory + "/" + FileName))
            {

                fileContent = sr.ReadToEnd();
                //while ((s = sr.ReadLine()) != null)
                //{
                //    //we're just testing read speeds
                //}
            }

            return fileContent;
        }

    }
}