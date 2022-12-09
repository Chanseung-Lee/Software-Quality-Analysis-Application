using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Web;
using Dapper;
using SQApp.Models;
using System.IO;
using GroupDocs.Parser.Data;
using GroupDocs.Parser.Options;
using GroupDocs.Parser;

namespace SQApp.DataBase
{
    public class SqliteDataAccess
    {
        private static string LoadConnectionString(string id = "DataBaseConnection")
        {
            ConnectionStringSettings dbConnectionSetting = ConfigurationManager.ConnectionStrings[id];
            if (dbConnectionSetting == null)
            {
                throw new ArgumentException("Connection string setting " + id + " does not exist.");
            }
            string dbConnectionString = dbConnectionSetting.ConnectionString.Replace("{Dir}", HttpRuntime.AppDomainAppPath);
            return dbConnectionString;
        }
        
        //Add all fields from a BuildFormat object into the Build table in db, specifying the table to be inserted within
        public static void AddToBuildTable(BuildFormat b, string table)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute($"replace into {table} " +
                    "(Id, BuildNumber, BuildDefinitionName, Result, Status, Url, CodeCoverage, FinishedDate, Branch, Repository, RunTime, StartDate, TestsExecuted) " +
                    "values (@Id, @BuildNumber, @BuildDefinitionName, @Result, @Status, @Url, @CodeCoverage, @FinishedDate, @Branch, @Repository, @RunTime, @StartDate, @TestsExecuted)", b);
            }
        }

        public static void AddToAvgCodeCoverageTable(CodeCoverageAvgFormat dataPoint, string table)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute($"replace into {table} " +
                    "(AvgCodeCoverage, BuildDefinitionName, BuildCount, Month) " +
                    "values (@AvgCodeCoverage, @BuildDefinitionName, @BuildCount, @Month)", dataPoint);
            }
        }

        public static List<CodeCoverageAvgFormat> PullAvgCodeCoverageTable(string table)
        {
            List<CodeCoverageAvgFormat> ImportedAverages = new List<CodeCoverageAvgFormat>();
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                using (SQLiteConnection connect = new SQLiteConnection(LoadConnectionString()))
                {
                    connect.Open();
                    using (SQLiteCommand fmd = connect.CreateCommand())
                    {
                        fmd.CommandText = $@"SELECT * FROM {table}";
                        fmd.CommandType = CommandType.Text;
                        SQLiteDataReader r = fmd.ExecuteReader();
                        do
                        {
                            while (r.Read())
                            {
                                var temp = new CodeCoverageAvgFormat 
                                { 
                                    AvgCodeCoverage = Convert.ToDouble(r.GetValue(0)),
                                    BuildDefinitionName = Convert.ToString(r.GetValue(1)),
                                    BuildCount = Convert.ToInt32(r.GetValue(2)),
                                    Month = DateTime.Parse(Convert.ToString(r.GetValue(3)))
                                };
                                ImportedAverages.Add(temp);
                            }
                        } while (r.NextResult());
                    }
                    connect.Close();
                }
            }
            return ImportedAverages;
        }

        public static List<BuildFormat> PullBuildTable(string table)
        {
            List<BuildFormat> ImportedBuilds = new List<BuildFormat>();
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                using (SQLiteConnection connect = new SQLiteConnection(LoadConnectionString()))
                {
                    connect.Open();
                    using (SQLiteCommand fmd = connect.CreateCommand())
                    {
                        fmd.CommandText = $@"SELECT * FROM {table}";
                        fmd.CommandType = CommandType.Text;
                        SQLiteDataReader r = fmd.ExecuteReader();
                        do
                        {
                            while (r.Read())
                            {
                                BuildFormat temp = new BuildFormat();

                                temp.Id = Convert.ToInt32(r.GetValue(0));
                                temp.BuildNumber = Convert.ToString(r.GetValue(1));
                                temp.BuildDefinitionName = Convert.ToString(r.GetValue(2));
                                temp.Result = Convert.ToString(r.GetValue(3));
                                temp.Status = Convert.ToString(r.GetValue(4));
                                temp.Url = Convert.ToString(r.GetValue(5));
                                if (r.GetValue(6) != System.DBNull.Value)
                                {
                                    temp.CodeCoverage = (float)Convert.ToDouble(r.GetValue(6));
                                }
                                temp.FinishedDate = DateTime.Parse(Convert.ToString(r.GetValue(7)));
                                temp.Branch = Convert.ToString(r.GetValue(8));
                                temp.Repository = Convert.ToString(r.GetValue(9));
                                temp.RunTime = TimeSpan.Parse(Convert.ToString(r.GetValue(10)));
                                temp.StartDate = DateTime.Parse(Convert.ToString(r.GetValue(11)));
                                temp.TestsExecuted = Convert.ToInt32(r.GetValue(12));

                                ImportedBuilds.Add(temp);
                            }
                        } while (r.NextResult());
                    }
                    connect.Close();
                }
            }
            return ImportedBuilds;
        }

        public static void ClearTable(string table)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute($"DELETE FROM {table}");
            }
        }

        // returns all build defs that have over 30 builds in table
        public static Dictionary<string, int> GetOverBuildDefs(string table)
        {
            Dictionary<string, int> buildDefs = new Dictionary<string, int>();
            using (SQLiteConnection connect = new SQLiteConnection(LoadConnectionString()))
            {
                connect.Open();
                using (SQLiteCommand fmd = connect.CreateCommand())
                {
                    fmd.CommandText = $@"SELECT BuildDefinitionName, BuildCount FROM (SELECT BuildDefinitionName, " +
                                      $@"COUNT(*) AS BuildCount FROM {table} GROUP BY BuildDefinitionName) WHERE BuildCount > 30;";
                    fmd.CommandType = CommandType.Text;
                    SQLiteDataReader r = fmd.ExecuteReader();
                    do
                    {
                        while (r.Read())
                        {
                            // key = build def name, value = # of builds for that build def in the table
                            buildDefs.Add(r.GetString(0), r.GetInt32(1));
                        }
                    } while (r.NextResult());
                }
                connect.Close();
            }
            return buildDefs;
        }

        //To be called when we want to delete rows from the db that are older than x days
        public static void DeleteOldBuildRows(string table, bool byDate)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                if (byDate)
                {
                    //Edit the number of days within the sql query statement inside the quote to set how far you want it to go back
                    cnn.Execute($"DELETE FROM {table} WHERE StartDate <= date('now', '-460 day')");
                }
                else
                {
                    // delete any extra rows in build definition sub-tables that have more than 30 rows
                    Dictionary<string, int> buildDefs = GetOverBuildDefs(table);
                    foreach(KeyValuePair<string, int> def in buildDefs)
                    {
                        string numOver = (def.Value - 30).ToString();
                        cnn.Execute($"DELETE FROM {table} WHERE Id IN (SELECT Id FROM {table} WHERE BuildDefinitionName = \"{def.Key}\" ORDER BY Id ASC LIMIT {numOver})");
                    }
                }
            }
        }
    }
}