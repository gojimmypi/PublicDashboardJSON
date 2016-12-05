using System;
using System.Text;
using Newtonsoft.Json;
using Microsoft.ApplicationBlocks.Data;
using System.Data;
using System.IO;

namespace PublicDashboardJSON
{
    class Program
    {
        private static string ServerName = "localhost";
        private static string DatabaseName = "Public_Dashboard";
        private static string OutputFile = "./Public_Dashboard.json";

        private static bool errorFound = false;
        private static string CrLf = "\r\n";
        private static bool blnShowHelp = false;

        //***********************************************************************************************************************************
        static void initMain()
        //***********************************************************************************************************************************
        {
            string[] args = Environment.GetCommandLineArgs();
            string[] argumentOptions;
            foreach (string arg in args)
            {
                argumentOptions = arg.Split(new Char[] { '=' }, 2);
                switch (argumentOptions[0].ToUpper().Trim())
                {
                    case "/SERVER":
                        if (argumentOptions.GetUpperBound(0) > 0)
                        {
                            ServerName = argumentOptions[1];
                        }
                        else
                        {
                            errorFound = true;
                            throw new Exception("ERROR Server name not specified.");
                        }
                        break;
                    case "/DATABASE":
                        if (argumentOptions.GetUpperBound(0) > 0)
                        {
                            DatabaseName = argumentOptions[1];
                        }
                        else
                        {
                            errorFound = true;
                            throw new Exception("ERROR Database name not specified.");
                        }
                        break;
                    case "/OUTPUT":
                        if (argumentOptions.GetUpperBound(0) > 0)
                        {
                            OutputFile = argumentOptions[1];
                        }
                        else
                        {
                            errorFound = true;
                            throw new Exception("ERROR Database name not specified.");
                        }
                        break;
                    case "/?":
                        blnShowHelp = true;
                        Console.WriteLine("AD_Maint [/SERVER=localhost] [/DATABASE=common] [/OUTPUT=./Public_Dashboard.json]");
                        Console.WriteLine("");
                        Console.WriteLine("  SERVER:             SQL Server name (default is localhost)");
                        Console.WriteLine("");
                        Console.WriteLine("  DATABASE:           SQL Database name (default is common)");
                        Console.WriteLine("");
                        Console.WriteLine("example: PublicDashboardJSON /DATABASE:mylocalhostdb ");
                        Console.WriteLine("");
                        Console.WriteLine("  /?                 Show this help screen.");
                        break;
                    default:
                        // do other stuff...
                        break;
                }
            }
        }


        //***********************************************************************************************************************************
        static string ConnectionString(string strServerName, string strDatabaseName)
        //***********************************************************************************************************************************
        {
            // see http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpref/html/frlrfSystemDataSqlClientSqlConnectionClassConnectionStringTopic.asp
            //
            // there is some debate as to whether the Oledb provider is indeed faster than the native client!
            //  
            return "Workstation ID=PublishDashboardJSON" +
                   "packet size=8192;" +
                   "Persist Security Info=false;" +
                   "Server=" + strServerName + ";" +
                   "Database=" + strDatabaseName + ";" +
                   "Trusted_Connection=true; " +
                   "Network Library=dbmssocn;" +
                   "Pooling=True; " +
                   "Enlist=True; " +
                   "Connection Lifetime=14400; " +
                   "Max Pool Size=20; Min Pool Size=0";
        }

        static void Main(string[] args)
        {
            initMain();

            DataSet ds;
            string strSQL = "SELECT * FROM dbo.vw_json_main_dashboard";
            ds = SqlHelper.ExecuteDataset(ConnectionString(ServerName, DatabaseName), CommandType.Text,strSQL);

            string json = JsonConvert.SerializeObject(ds.Tables[0], Formatting.Indented);
            // System.ServiceModel.Web.WebOperationContext.Current.OutgoingResponse.ContentType = "application/json; charset=utf-8";

            File.WriteAllText(OutputFile, json);
        }
    }
}
