using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Build.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

using System.Dynamic;

using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.Git.Client;
using Microsoft.TeamFoundation.Server;
using SQApp.Query_Library;
using SQApp.Models;

namespace SQApp.Models
{
    public class TestConnection
    {
        public static string testDomains()
        {
            return "CurrentDomain.BaseDirectory path: " + AppDomain.CurrentDomain.BaseDirectory + " -- " + HttpRuntime.AppDomainAppPath;
        }
        public static string testGetCommits(TfsTeamProjectCollection _tfsInstance)
        {
            GitHttpClient _gitClient = _tfsInstance.GetClient<GitHttpClient>();
            GitQueryCommitsCriteria _criteria = new GitQueryCommitsCriteria();
            GitVersionDescriptor _descriptor = new GitVersionDescriptor();
            _descriptor.VersionType = GitVersionType.Branch;
            _descriptor.Version = "refs/heads/develop"; //might need full path name
            _descriptor.VersionOptions = 0;
            _criteria.ItemVersion = _descriptor;
            _criteria.FromDate = DateTime.Now.AddDays(-365).ToString();
            List<GitRepository> repo = _gitClient.GetRepositoriesAsync("Flex").Result;
            return repo.Count.ToString();
            //List<GitCommitRef> _commits = _gitClient.GetCommitsAsync("Flex", "Flex", _criteria).Result;
            //return _commits.Count.ToString();
        }
        public static string authenticateCreds(TFSCredentials creds)
        {
            const string uri_name = "http://pdxdevops.esi.com:8080/tfs";
            NetworkCredential netCred = new NetworkCredential(new string(creds.Username), new string(creds.Password));
            Microsoft.VisualStudio.Services.Common.WindowsCredential winAuth = new Microsoft.VisualStudio.Services.Common.WindowsCredential(netCred);
            VssCredentials vssAuth = new VssCredentials(winAuth);
            UriBuilder uri = new UriBuilder(uri_name);
            TfsTeamProjectCollection tfs = new TfsTeamProjectCollection(uri.Uri, vssAuth);
            
            try
            {
                tfs.Authenticate();
                return tfs.AuthorizedIdentity.DisplayName;
            }
            catch (Microsoft.TeamFoundation.TeamFoundationServerUnauthorizedException)
            {
                return "Connection failed. Attempted to authenticate with user: " + new string(creds.Username) + " with password: " + new string(creds.Password);
            }
            catch
            {
                return "Unknown error";
            }
        }
        // connects to WorkItemStore. Used for making wiql queries for anything item-related (bugs, tasks, etc.) - Works
        public static string connectWorkItems()
        {
            // connect to on-premise tfs. Doesn't work: says need secure connection to server for basic authentication to work.
            /*const string c_collectionUri = "http://pdxdevops.esi.com:8080/tfs/ESI";
            const string c_projectName = "Flex";
            const string c_repoName = "Flex";
            const string pat = "a pat";
            var creds = new VssBasicCredential("ryan.roberts", pat); // add user name instead of empty string?
            // connect to service?
            var connection = new VssConnection(new Uri(c_collectionUri), creds);
            var gitClient = connection.GetClient<GitHttpClient>();
            var repo = gitClient.GetRepositoryAsync(c_projectName, c_repoName).Result;
            Console.WriteLine(repo);*/

            const string uri_name = "http://pdxdevops.esi.com:8080/tfs";
            string username, password;
            username = Environment.GetEnvironmentVariable("DEVOPS_USR", EnvironmentVariableTarget.User);
            password = Environment.GetEnvironmentVariable("DEVOPS_PSWD", EnvironmentVariableTarget.User);
            if (String.IsNullOrEmpty(username)) username = Environment.GetEnvironmentVariable("DEVOPS_USR", EnvironmentVariableTarget.Process);
            if (String.IsNullOrEmpty(password)) password = Environment.GetEnvironmentVariable("DEVOPS_PSWD", EnvironmentVariableTarget.Process);
            NetworkCredential netCred = new NetworkCredential(username, password);
            Microsoft.VisualStudio.Services.Common.WindowsCredential winAuth = new Microsoft.VisualStudio.Services.Common.WindowsCredential(netCred);
            VssCredentials vssAuth = new VssCredentials(winAuth); 
            UriBuilder uri = new UriBuilder(uri_name);
            TfsTeamProjectCollection tfs = new TfsTeamProjectCollection(uri.Uri, vssAuth);
            try
            {
                tfs.Authenticate();
                return tfs.AuthorizedIdentity.DisplayName;
            }
            catch (Microsoft.TeamFoundation.TeamFoundationServerUnauthorizedException) 
            {
                return "Connection failed. Attempted to authenticate with user: " + username + " with password: " + password;
            }
            catch
            {
                return "Unknown error";
            }

            string activenewQuery = "SELECT   [System.Id] " +
                          "FROM     WorkItems " +
                          "WHERE    [System.TeamProject] = '{0}' " +
                          "AND      ([System.State] = 'Active' OR [System.State] = 'New') " +
                          "AND       [System.CreatedDate] >= @today-14 " +
                          /*"AND      [System.IterationPath] UNDER '{1}' " +*/
                          "AND      [System.AreaPath] UNDER '{1}' ";
            
            //format query with dynamic properties
            string wiqlQuery = String.Format(activenewQuery, "Flex", "Flex\\Software Quality Team");

            // make query and get back WorkItemCollection object
            WorkItemStore workItems = (WorkItemStore)tfs.GetService(typeof(WorkItemStore));
            WorkItemCollection tfs_list = workItems.Query(wiqlQuery);

            // parse object (only title for now)
            // TODO: need to create a struct with attributes to hold all work item information
            string[] tmp = new string[tfs_list.Count];
            int i = 0;
            foreach(WorkItem _item in tfs_list)
            {
                tmp[i] = _item.Title;
                i++;
            }
            /*int i = 0;
            foreach(WorkItem _item in tfs_list)
            {
                tmp[i] = _item.Copy();
                i++;
            }*/

            //WIQLQuery test = new WIQLQuery();
            //WorkItemFormat[] testing = test.formatWorkItemList(tfs_list);

          //  return tmp;

        }

        // connects to on-premise server for retrieving build information
        public static string connectBuilds()
        {
            string[] tmp =  new string[6];
            const string uri_name = "http://pdxdevops.esi.com:8080/tfs/ESI";
            UriBuilder uri = new UriBuilder(uri_name);
            //VssCredentials creds = new VssAadCredential("ryan.roberts", ""); // can add to constructor - works but not necessary
            TfsTeamProjectCollection tfs = new TfsTeamProjectCollection(uri.Uri);
            tfs.Authenticate();
            IBuildServer build_client = (IBuildServer)tfs.GetService(typeof(IBuildServer)); //exists! uses Build.Client namespace
            //string[] summary = build_client.GetBuildQualities("Flex");

            GitRepositoryService repo_client = (GitRepositoryService)tfs.GetService(typeof(GitRepositoryService)); // must have ESI at end of uri to work
            IList<GitRepository> repos = repo_client.QueryRepositories("Flex"); // works!
            BuildHttpClient http_client = tfs.GetClient<BuildHttpClient>(); // Works for getting build definitions and builds related to a certain project
            IList<Build> builds = http_client.GetBuildsAsync("Flex").Result; // builds
            IList<BuildDefinitionReference> summary = http_client.GetDefinitionsAsync(
                                                                 "Flex",
                                                                 builtAfter: DateTime.Now.AddDays(-30),
                                                                 queryOrder : DefinitionQueryOrder.DefinitionNameAscending
                                                                 ).Result; // build definitions
            //List<string> summary = new List<string>();

            // for testing expired build
            //summary.Add((http_client.GetBuildAsync("Flex", buildId: 62757).Result).BuildNumber); // 62757 is an expired build - cannot be found!

            // haven't found use for build definitions yet.
            /*foreach(BuildDefinitionReference _def in defs)
            {
                //summary.Add(_build.Result.ToString());
                //summary.Add(_build.Deleted.ToString()); // none are deleted...
                //summary.Add(_build.Definition.Name);
                summary.Add(_def.Name);
            }*/

            /*foreach (Build _build in builds)
            {
                //summary.Add(_build.Repository.Type);
                summary.Add(_build.Properties.Count.ToString()); // no properties in any build!
                PropertiesCollection props = _build.Properties;
                foreach(var _test in props.Values)
                {
                    summary.Add(_test.ToString());
                }
                
                //summary.Add(_build.Deleted.ToString()); // none are deleted...
                //summary.Add(_build.Definition.Name);
            }*/


            ITestManagementService test_client = (ITestManagementService)tfs.GetService(typeof(ITestManagementService));
            ITestManagementTeamProject proj = test_client.GetTeamProject("Flex");
            IList<ITestCaseQuery> queries = proj.Queries;
            tmp[1] = "proj is valid: " + proj.IsValid.ToString();
            tmp[2] = "test_client supported: " + test_client.IsSupported().ToString();
            /*IEnumerable<ITestRun> testRuns = test_client.QueryTestRuns("SELECT   [State] " +
                                                                          "FROM     TestRun " +
                                                                          "WHERE    [System.TeamProject] = 'Flex' "  +
                                                                          "AND      [System.BuildNumber] = '3.0.2-alpha.1116' " +
                                                                          "AND      [System.IsAutomated] = 1"); // can query by BuildNumber!*/

            // can be one or more test runs. gets code coverage for build using tied test runs
            /*CoverageQueryFlags flag = CoverageQueryFlags.Modules; // must be modules
            float runLinesCovered = 0, runLinesNotCovered = 0, runLinesPartCovered = 0;
            foreach (ITestRun _run in testRuns)
            {
                //float passPercentage = ((float)_run.PassedTests / _run.TotalTests) * 100; //doesn't consider not executed tests in total tests
                summary.Add("Total tests executed in run " + _run.Title + " : " + ( _run.Statistics.TotalTests - _run.Statistics.InconclusiveTests) + " yes: " + _run.BuildUri.AbsoluteUri);
                ITestRunCoverage[] testRunCoverages = proj.CoverageAnalysisManager.QueryTestRunCoverage(_run.Id, flag);
                foreach(ITestRunCoverage _rCov in testRunCoverages)
                {
                    foreach (IModuleCoverage _module in _rCov.Modules)
                    {
                        runLinesCovered += _module.Statistics.LinesCovered;
                        runLinesNotCovered += _module.Statistics.LinesNotCovered;
                        runLinesPartCovered += +_module.Statistics.LinesPartiallyCovered;
                    }
                }
            }*/
         //   summary.Add(runLinesCovered.ToString());
         //   summary.Add(runLinesNotCovered.ToString());
            // total coverage percentage
            //summary.Add("Total Lines covered percentage from Run: " + ((runLinesCovered / (runLinesCovered + runLinesNotCovered + runLinesPartCovered)) * 100).ToString());

            // gets code coverage for build using build uri (returns same values as from test run calculations)
            //CoverageQueryFlags flag = CoverageQueryFlags.Modules; // must be modules
            
            /*IBuildCoverage[] buildCoverage = proj.CoverageAnalysisManager.QueryBuildCoverage(builds[10].Uri.AbsoluteUri, flag); // get builds uri for build Flex_4.2_Experimental_99.0.20220407.02
            float totalLinesCovered = 0, totalLinesNotCovered = 0, totalLinesPartCovered = 0;
            foreach (IBuildCoverage _build in buildCoverage)
            {
              //  summary.Add(_build.State.ToString());
                foreach (IModuleCoverage _module in _build.Modules)
                {
                    totalLinesCovered += _module.Statistics.LinesCovered;
                    totalLinesNotCovered += _module.Statistics.LinesNotCovered;
                    totalLinesPartCovered += _module.Statistics.LinesPartiallyCovered;
                }
            }*/

           // summary.Add(totalLinesCovered.ToString());
           // summary.Add(totalLinesNotCovered.ToString());
           // summary.Add(totalLinesPartCovered.ToString());
            // total coverage percentage
            //summary.Add("Total Lines covered percentage: " + ((totalLinesCovered/(totalLinesCovered + totalLinesNotCovered + totalLinesPartCovered))*100).ToString());


            /*DateTime cutoffDate = DateTime.Now.AddDays(-1);
            foreach (ITestRun _item in testRuns)
            {
                //summary[k] = _item.BuildNumber; // get a name of the test run
                //summary[k] = _item.State.ToString();
                //summary[k] = _item.DateStarted.ToString();
                if (_item.DateCompleted <= cutoffDate)
                {
                    continue;
                }
                // gets build stats (failed/succeeded/etc.) using testRun item and its BuildUri for the build that the test run used...
                // find a way to get all current build uris to check. Oldest runs have build Uri's that aren't valid
                //summary[k] = "Total Tests: " + _item.TotalTests + " Passed Tests: " + _item.PassedTests + " Build status: " + build_client.GetAllBuildDetails(_item.BuildUri).Status;

                //summary[k] = _item.Statistics.FailedTests.ToString(); // times out server, don't need since can get passedtests + totaltests

                //summary.Add("Build Number for Test run: " + _item.Id + " Build Abs path: " + _item.BuildUri.GetLeftPart(UriPartial.Scheme));

                // can use getminimalBuildDetails for quicker runtime
                summary.Add("Test run: "  + _item.BuildNumber + " -- Build Status: " + build_client.GetMinimalBuildDetails(_item.BuildUri).Status + " -- Date Completed: " + _item.DateCompleted);
            }*/

            // for code coverage of each test run
            /*CoverageQueryFlags flag = CoverageQueryFlags.Modules;
            List<string> summary = new List<string>();
            int n = 0;
            foreach(ITestRun _run in testRuns)
            {
                ITestRunCoverage[] testRunCoverages = proj.CoverageAnalysisManager.QueryTestRunCoverage(_run.Id, flag);
                if(testRunCoverages.Length > 0)
                {
                    //summary[n] = testRunCoverages[0].Modules[2].Statistics.LinesNotCovered.ToString(); // modules list = 11. for [0] lines covered = 142, [2] = 178. lines not covered for [2] = 3838, [0] = 31 
                    summary.Add(testRunCoverages[0].Modules[2].Name);
                }
                else summary.Add(testRunCoverages.Length.ToString());
                n++;
                //for (int m = 0; m < testRunCoverages.Length; m++)
                //{
                //    summary[m] = testRunCoverages[m].Modules[0].Statistics.LinesCovered.ToString(); // see how many modules for the first test run there are
                //   if (m == 100) break;
                //}
                //break; //ony go through first run for testing
                if (n == 45) break;
            }*/

            //ITestRunCoverage[] testRunCoverages = proj.CoverageAnalysisManager.QueryTestRunCoverage(testRuns.GetEnumerator().Current.Id, flag);
            /*string[] summary = new string[testRunCoverages.Length];
            for(int m = 0; m < testRunCoverages.Length; m++)
            {
                summary[m] = testRunCoverages[m].Modules.Length.ToString(); // see how many modules for the first test run there are
            }*/

            // can be used to get WIQL query for existing metrics...
            List<string> allQueries = new List<string>();
            foreach (ITestCaseQuery _item in queries)
            {
                allQueries.Add(_item.Name);
                //summary[k] = _item.QueryText; // gets WIQL query
            }

            //IBuildController current_build = build_client.GetBuildController("Flex_4.2_Experimental");
            /*IBuildController[] all_builds = build_client.QueryBuildControllers(false); //dont include build agents, only 4 total
            int j = 1;
            foreach(IBuildController _build in all_builds)
            {
                tmp[j] = _build.Name;
                j++;
            }*/

            // 4 build controllers,
            // 0 builds,
            // 0 build definitions
            // tfs.Authenticate() uses Azure Active Directory authentication (AAD)
            // cant display default workItem, ForwardEnd property nested too deeply

            //UriBuilder buildsUri = new UriBuilder("vstfs:///Build/Build/69986");
            IBuildDetail[] test = build_client.QueryBuilds("Flex"); // returns 0!
            IBuildDefinition[] sum = build_client.QueryBuildDefinitions("Flex"); // returns 0!
            //IBuildDetail[] test2 = build_client.QueryBuildsByUri(new Uri[1] { buildsUri.Uri}, new string[1] { InformationTypes.BuildError }, QueryOptions.All);
            //summary.Add(test2.Length.ToString());
            //summary.Add(sum.Length.ToString());
            //tmp[5] = test.Length.ToString();
            //tmp[5] = repos.Count.ToString();
            //summary.Add(test.Length.ToString());

            //var _networkCreds = new NetworkCredential("MKS\\Ryan.Roberts", "pswd");
            //var _winCreds = new Microsoft.VisualStudio.Services.Common.WindowsCredential(_networkCreds);
            //VssCredentials creds = new VssClientCredentials(_winCreds); // can try VssBasicCredentials w/ pat - doesnt work?
            VssCredentials creds = new VssAadCredential("MKS\\Ryan.Roberts", System.Environment.GetEnvironmentVariable("MKS_PSWD"));
             //VssHttpMessageHandler vssHandler = new VssHttpMessageHandler(creds, VssClientHttpRequestSettings.Default.Clone());
            VssConnection connection = new VssConnection(uri.Uri, creds); // maybe works? need to find ways to use it...
            // IBuildServer client = (IBuildServer)connection.GetClient(typeof(IBuildServer));
            //tmp[3] = client.QueryBuilds("Flex").Length.ToString();

            /*if (http_client == null)
            {
                summary.Add("http_client fail");
            }
            else
            {
                summary.Add("http_client success");
            }
            if (connection.HasAuthenticated == false)
            {
                tmp[1] = "connection fail";
            }
            else
            {
                tmp[1] = "connection success";
            }*/

            return JsonConvert.SerializeObject(summary);
        }

        public static string GetAlreadyMadeQueries()
        {

            const string uri_name = "http://pdxdevops.esi.com:8080/tfs/ESI";
            UriBuilder uri = new UriBuilder(uri_name);
            TfsTeamProjectCollection tfs = new TfsTeamProjectCollection(uri.Uri);
            tfs.Authenticate();

            ITestManagementService test_client = (ITestManagementService)tfs.GetService(typeof(ITestManagementService));
            ITestManagementTeamProject proj = test_client.GetTeamProject("Flex");
            IList<ITestCaseQuery> queries = proj.Queries;

            // Open Bug trends query
            WorkItemStore workItems = (WorkItemStore)tfs.GetService(typeof(WorkItemStore));

            // don't need any of the selects, get all of it anyways.
            WorkItemCollection workItemList = workItems.Query("select [System.Id]" +
                                                               "from WorkItems where [System.TeamProject] = 'Flex' " +
                                                               "and [System.WorkItemType] = 'Bug' " +
                                                               "and [System.State] <> 'Closed' " +
                                                               "and [System.State] <> 'Removed'");
            
            List<string> allQueries = new List<string>();
            /*var queryResults = new List<dynamic>();
            foreach (WorkItem _item in workItemList)
            {
               // dynamic itemLayout = new ExpandoObject();
                // get all values for all fields.
                // Fields in a work item contain all information for the specific work item that otherwise aren't available in the workItem class.
                foreach (Field _field in _item.Fields)
                {
                    allQueries.Add(_field.Name);
                    if (_field.Value != null) allQueries.Add(_field.Value.GetType().ToString());
                    else allQueries.Add("null");
                    //((IDictionary<string, Object>)itemLayout).Add(_field.Name, _field.Value);
                }
                //queryResults.Add(itemLayout);
                break;
            }*/

            // can be used to get WIQL query for existing metrics...
            foreach (ITestCaseQuery _item in queries)
            {
                allQueries.Add(_item.Name);
                allQueries.Add(_item.QueryText); // gets WIQL query
            }

            return JsonConvert.SerializeObject(allQueries);
        }

    }
}