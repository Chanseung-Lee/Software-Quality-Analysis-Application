using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.Git.Client;
using Microsoft.TeamFoundation.Framework.Client;
using System.Text.Json;
using Newtonsoft.Json;
using SQApp.Models;
using SQApp.DataBase;

namespace SQApp.Query_Library
{
    public class BuildQuery : Query
    {
        protected static BuildHttpClient _httpClient;
        protected static ITestManagementService _testRunClient;
        private static GitRepositoryService _repoClient;
        private readonly CoverageQueryFlags _flag = CoverageQueryFlags.Modules;
        private const string _buildUrlFormat = "http://pdxdevops.esi.com:8080/tfs/ESI/{0}/_build/results?buildId={1}&view=results";
        private const string _testRunQuery = "SELECT   [State] " +
                                                "FROM     TestRun " +
                                                "WHERE    [System.TeamProject] = '{0}' " +
                                                "AND      [System.BuildUri] = '{1}' " +
                                                "AND      [System.IsAutomated] = 1";

        protected ITestManagementTeamProject _teamProject;
        protected string _filter;

        public string Project { get; set; }
        public int DaysBack { get; set; }
        public bool AllTime { get; set; }
        public BuildResult? ResultFilter { get; set; }
        public BuildStatus? StatusFilter { get; set; }
        public bool CalcCodeCoverage { get; set; }
        public BuildQueryOrder? QueryOrder { get; set; }
        public int? MaxReturnsPerRepo { get; set; }
        public List<string> FilterBy { get; set; }
        public int? DaysBackDefs { get; set; }
        public bool MasterBranchesOnly { get; set; }
        public bool ManageDbByDate { get; set; }

        public BuildQuery()
        {
            Name = string.Empty;
            Project = string.Empty;
            DaysBack = 0;
            GetServices();
        }

        public BuildQuery(string name, string project, int daysBack = 0, bool allTime = false, BuildResult? resultFilter = null,
            BuildStatus? statusFilter = null, bool masterBranchesOnly = false, bool calcCodeCoverage = true, BuildQueryOrder? queryOrder = null, 
            int? maxReturnsPerRepo = null, List<string> filterBy = null, int? daysBackDefs = null, bool manageDbByDate = true)
        {
            Name = name;
            Project = project;
            AllTime = allTime;
            DaysBack = daysBack;
            ResultFilter = resultFilter;
            StatusFilter = statusFilter;
            MasterBranchesOnly = masterBranchesOnly;
            CalcCodeCoverage = calcCodeCoverage;
            QueryOrder = queryOrder;
            MaxReturnsPerRepo = maxReturnsPerRepo;
            FilterBy = filterBy;
            DaysBackDefs = daysBackDefs;
            ManageDbByDate = manageDbByDate;

            if (FilterBy != null)
            {
                _filter = FilterBy[0];
                FilterBy.RemoveAt(0);
            }
            else _filter = null;

            GetServices();
        }

        protected static void GetServices()
        {
            // Works for getting build definitions and builds related to a certain project
            _httpClient = _tfsInstance.GetClient<BuildHttpClient>();
            // for calculating code coverage of a build
            _testRunClient = (ITestManagementService)_tfsInstance.GetService(typeof(ITestManagementService));
            // for filtering builds by branch and repo
            _repoClient = (GitRepositoryService)_tfsInstance.GetService(typeof(GitRepositoryService));
        }

        private int GetTestsExecuted(string buildUri)
        {
            // need to query by buildUri since it contains build Id which is only unique field for a build
            IEnumerable<ITestRun> testRuns = _testRunClient.QueryTestRuns(String.Format(_testRunQuery, Project, buildUri));
            int totalTestsExecuted = 0;
            foreach (ITestRun _run in testRuns)
            {
                totalTestsExecuted += _run.Statistics.TotalTests - _run.Statistics.InconclusiveTests; // removes tests not executed
            }
            return totalTestsExecuted;
        }

        // Note: Returns 0% code coverage for builds with no test coverage modules.
        protected float CalculateCodeCoverage(string buildUri)
        {
            IBuildCoverage[] buildCoverage = _teamProject.CoverageAnalysisManager.QueryBuildCoverage(buildUri, _flag);
            float totalLinesCovered = 0, totalLinesNotCovered = 0, totalLinesPartCovered = 0;
            foreach (IBuildCoverage _build in buildCoverage)
            {
                foreach (IModuleCoverage _module in _build.Modules)
                {
                    totalLinesCovered += _module.Statistics.LinesCovered;
                    totalLinesNotCovered += _module.Statistics.LinesNotCovered;
                    totalLinesPartCovered += _module.Statistics.LinesPartiallyCovered;
                }
            }
            float coveragePercentage = (totalLinesCovered / (totalLinesCovered + totalLinesNotCovered + totalLinesPartCovered)) * 100;
            return float.IsNaN(coveragePercentage) ? 0 : coveragePercentage;
        }

        private BuildFormat FormatBuild(Build rawBuild)
        {
            BuildFormat build = new BuildFormat
            {
                Id = rawBuild.Id,
                BuildNumber = rawBuild.BuildNumber,
                BuildDefinitionName = rawBuild.Definition.Name,
                Result = rawBuild.Result.ToString(),
                Status = rawBuild.Status.ToString(),
                Url = String.Format(_buildUrlFormat, Project, rawBuild.Id),
                FinishedDate = (DateTime)rawBuild.FinishTime,
                Branch = rawBuild.SourceBranch,
                Repository = rawBuild.Repository.Name,
                StartDate = (DateTime)rawBuild.StartTime,
                RunTime = (DateTime)rawBuild.FinishTime - (DateTime)rawBuild.StartTime,
                TestsExecuted = GetTestsExecuted(rawBuild.Uri.AbsoluteUri),
                Links = rawBuild.Links
            };
            if (CalcCodeCoverage) build.CodeCoverage = CalculateCodeCoverage(rawBuild.Uri.AbsoluteUri);
            else build.CodeCoverage = Single.NaN;
            return build;
        }

        // (can update by using githttpClient from CommitQuery to use GetRepositoryAsync() function)
        protected List<GitRepository> GetWantedRepos()
        {
            IList<GitRepository> allRepos = _repoClient.QueryRepositories(Project);
            if (FilterBy.Count == 0) return (List<GitRepository>)allRepos;

            List<GitRepository> wantedRepos = new List<GitRepository>();
            foreach (var _r in allRepos)
            {
                if (FilterBy.Contains(_r.Name))
                {
                    wantedRepos.Add(_r);
                }
            }
            return wantedRepos;
        }

        protected void QueryByRepos(ref List<Build> buildList, DateTime cutoffDate)
        {
            string branch;
            List<GitRepository> wantedRepos = GetWantedRepos();
            foreach (GitRepository _repo in wantedRepos)
            {
                if (MasterBranchesOnly) branch = _repo.DefaultBranch;
                else branch = null;
                buildList.AddRange(_httpClient.GetBuildsAsync(
                    Project,
                    minFinishTime: cutoffDate,
                    statusFilter: StatusFilter,
                    resultFilter: ResultFilter,
                    repositoryId: _repo.Id.ToString(),
                    top: MaxReturnsPerRepo,
                    queryOrder: QueryOrder,
                    repositoryType: "TfsGit", // all except one build on all projects were TfsGit (other was just Git)
                    branchName: branch,
                    deletedFilter: QueryDeletedOption.IncludeDeleted
                ).Result);
            }
        }

        protected List<BuildDefinitionReference> GetWantedBuildDefs()
        {
            List<BuildDefinitionReference> buildDefs = new List<BuildDefinitionReference>();
            if (FilterBy.Count == 0)
            {
                // get all build definitions in Project by last builds
                // Can't set to null b/c target-type conditional expression not available in C# 7.3
                DateTime builtAfterDate = DaysBackDefs != null ? DateTime.Today.AddDays(-(int)DaysBackDefs) : default(DateTime);
                buildDefs.AddRange(_httpClient.GetDefinitionsAsync(
                    Project,
                    builtAfter: builtAfterDate
                ).Result);
                buildDefs.RemoveAll(_build => _build.DefinitionQuality == DefinitionQuality.Draft);
            }
            else
            {
                // get build definition(s) by name
                foreach (string _defName in FilterBy)
                {
                    buildDefs.AddRange(_httpClient.GetDefinitionsAsync(
                        Project,
                        name: _defName
                    ).Result);
                }
            }
            return buildDefs;
        }

        protected void QueryByBuildDefs(ref List<Build> buildList, DateTime cutoffDate)
        {
            var buildDefs = GetWantedBuildDefs();
            List<Int32> buildDefIds = new List<Int32>();
            // get ids for all build definitions
            foreach (BuildDefinitionReference _def in buildDefs)
            {
                buildDefIds.Add(_def.Id);
            }
            buildList.AddRange(_httpClient.GetBuildsAsync(
                Project,
                definitions: buildDefIds,
                minFinishTime: cutoffDate,
                statusFilter: StatusFilter,
                resultFilter: ResultFilter,
                maxBuildsPerDefinition: MaxReturnsPerRepo,
                queryOrder: QueryOrder,
                deletedFilter: QueryDeletedOption.IncludeDeleted
            ).Result);
        }

        public override string RunQuery()
        {
            _teamProject = _testRunClient.GetTeamProject(Project);
            DateTime cutoffDate;
            if (AllTime) cutoffDate = default(DateTime);
            else cutoffDate = DateTime.Today.AddDays(-DaysBack);
            List<Build> allBuilds = new List<Build>();

            // Note: switch case won't work - Filter values are not constant
            if (_filter == null)
            {
                allBuilds.AddRange(_httpClient.GetBuildsAsync(
                    Project,
                    minFinishTime: cutoffDate,
                    statusFilter: StatusFilter,
                    resultFilter: ResultFilter,
                    top: MaxReturnsPerRepo,
                    queryOrder: QueryOrder,
                    deletedFilter: QueryDeletedOption.IncludeDeleted
                ).Result);
            }

            else if (_filter == Filter.Repositories)
            {
                QueryByRepos(ref allBuilds, cutoffDate);
            }

            else if (_filter == Filter.BuildDefinitions)
            {
                QueryByBuildDefs(ref allBuilds, cutoffDate);
            }

            else
            { 
                throw new ArgumentException("Unknown filter type in first index of FilterBy"); 
            }

            List<BuildFormat> buildList = new List<BuildFormat>();
            foreach(Build _build in allBuilds)
            {
                buildList.Add(FormatBuild(_build));
            }

            foreach(BuildFormat _b in buildList)
            {
                SqliteDataAccess.AddToBuildTable(_b, Name);
            }
            SqliteDataAccess.DeleteOldBuildRows(Name, ManageDbByDate);
            List<BuildFormat> pulledList = SqliteDataAccess.PullBuildTable(Name);

            // need to have uniform return type b/c c# 7.3 doesn't support covariant returns. SqliteDataAccess.GetOverBuildDefs(Name)
            string json = JsonConvert.SerializeObject(pulledList);
            return json;
        }
    }


    // calculate code coverage always on.
    // Only filters by build definitions.
    // overwrite current month in db with new calculated average for a build definition. 
    public class TestCoverageAvgQuery : BuildQuery
    {
        // month and year used retrieve old data for specific uses.
        public int Month { get; set; }
        public int Year { get; set; }
        public bool CurrentMonth { get; set; }

        public TestCoverageAvgQuery(string name, string project, List<string> filterBy, BuildResult? resultFilter = null, 
            BuildStatus? statusFilter = null, bool masterBranchesOnly = false, BuildQueryOrder? queryOrder = null, 
            int? maxReturnsPerRepo = null, int? daysBackDefs = null, int month = 1, int year = 1, bool currentMonth = true)
        {
            Name = name;
            Project = project;
            ResultFilter = resultFilter;
            StatusFilter = statusFilter;
            MasterBranchesOnly = masterBranchesOnly;
            QueryOrder = queryOrder;
            MaxReturnsPerRepo = maxReturnsPerRepo;
            FilterBy = filterBy;
            DaysBackDefs = daysBackDefs;
            Month = month;
            Year = year;
            CurrentMonth = currentMonth;

            if (FilterBy.Count == 0)
            {
                throw new ArgumentException("Invalid size of filterBy. Must be at least length of 1");
            }
            _filter = FilterBy[0];
            FilterBy.RemoveAt(0);

            GetServices();
        }

        // returns values for minFinishTime and maxFinishTime for build query
        private (DateTime startDate, DateTime endDate) GetMonthRange()
        {
            DateTime startDate;
            if (CurrentMonth) startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            else startDate = new DateTime(Year, Month, 1);
            DateTime endDate = new DateTime(startDate.Year, startDate.Month, DateTime.DaysInMonth(startDate.Year, startDate.Month));
            return (startDate, endDate);
        }

        // returns -1 if no builds in rawBuilds
        private float CalcAvgCodeCoverage(List<Build> rawBuilds)
        {
            float codeCoverageAvg = 0;
            foreach (var _build in rawBuilds)
            {
                codeCoverageAvg += CalculateCodeCoverage(_build.Uri.AbsoluteUri);
            }
            codeCoverageAvg /= rawBuilds.Count;
            return float.IsNaN(codeCoverageAvg) ? -1 : codeCoverageAvg;
        }

        private void QueryByBuildDefs(ref List<CodeCoverageAvgFormat> dbDataList)
        {
            var finishTimes = GetMonthRange();
            var buildDefs = GetWantedBuildDefs();
            foreach (var _def in buildDefs)
            {
                var defBuilds = _httpClient.GetBuildsAsync(
                    Project,
                    definitions: new List<int> {_def.Id},
                    minFinishTime: finishTimes.startDate,
                    maxFinishTime: finishTimes.endDate,
                    statusFilter: StatusFilter,
                    resultFilter: ResultFilter,
                    maxBuildsPerDefinition: MaxReturnsPerRepo,
                    queryOrder: QueryOrder,
                    deletedFilter: QueryDeletedOption.IncludeDeleted
                ).Result;

                dbDataList.Add(new CodeCoverageAvgFormat
                {
                    AvgCodeCoverage = CalcAvgCodeCoverage(defBuilds),
                    BuildDefinitionName = _def.Name,
                    BuildCount = defBuilds.Count,
                    Month = finishTimes.startDate
                });
            }
        }

        public override string RunQuery()
        {
            _teamProject = _testRunClient.GetTeamProject(Project);

            // retrieve updated average for current month or specified month
            List<CodeCoverageAvgFormat> newData = new List<CodeCoverageAvgFormat>();
            if (_filter == Filter.BuildDefinitions)
            {
                QueryByBuildDefs(ref newData);
            }
            else
            {
                throw new ArgumentException("Unknown filter type in first index of FilterBy. TestCoverageAvgQuery can only query by BuildDefinitions");
            }
            
            // add to db, pull previous months from db to return
            foreach (var _avg in newData)
            {
                SqliteDataAccess.AddToAvgCodeCoverageTable(_avg, Name);
            }
            var dbData = SqliteDataAccess.PullAvgCodeCoverageTable(Name);

            string json = JsonConvert.SerializeObject(dbData);
            return json;
        }
    }

}