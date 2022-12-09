using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Newtonsoft.Json;
using SQApp.Models;
using System.Reflection;

namespace SQApp.Query_Library
{
    /*
     * Currently have it set to only query by branches and repos in a project.
     * No version options are selected. More filters can be created, see Microsoft
     * GitQueryCommitsCriteria class implementation for more possible options to filter by.
     * 
     * Notes:
     *  - Dictionary RepoBranches tells which Repo(s) (keys) in the project to pull from as well as which
     *    branches in the repo(s) (values).
     *      
     *    - If...
     *   
     *     - RepoBranches = null and DefaultBranchesOnly = false : queries all repos from all branches in project
     *     
     *     - RepoBranches = null and DefaultBranchesOnly = true : queries all repos from only default branches in project
     *     
     *     - RepoBranches has keys and a key has empty values and DefaultBranchesOnly = false : queries from specified repos in all branches in repos.
     *     
     *     - RepoBranches has keys and a key has filled values and DefaultBranchesOnly = false or true : queries from specified repos in specified branches in repos.
     *     
     *     - RepoBranches has keys and a key has empty values and DefaultBranchesOnly = true : queries from specified repos in default branches
     *                                                                                         for repos that don't specify branches.
     *                                                                                         
     *     - IncludeReleaseBranches = true : Will append all release branches in a repo to the defined list of branches or default branches to query results. Will
     *                                       only include release branches if no defined branches (no branches defined in RepoBranches & DefaultBranchesOnly = false).
     */
    public class CommitQuery : Query
    {
        private static GitHttpClient _gitClient;
        private const string _rawBranchPrefix = "Refs/heads/";
        private const string _releaseBranchPrefix = "Releases/";

        public string Project { get; set; }
        public int? MaxReturnsPerBranch { get; set; }
        public Dictionary<string, string[]> RepoBranches { get; set; }
        public bool DefaultBranchesOnly { get; set; }
        public bool IncludeReleaseBranches { get; set; }
        public int? DaysBackFrom { get; set; }
        public int? DaysBackTo { get; set; }

        public CommitQuery() 
        {
            Name = string.Empty;
            Project = string.Empty;
            MaxReturnsPerBranch = null;
            GetServices();
        }

        // Have to set maxReturns b/c default max returns (null) for query function is 100
        public CommitQuery(string name, string project, int maxReturnsPerBranch, Dictionary<string, string[]> repoBranches = null, 
            bool defaultBranchesOnly = false, int? daysBackFrom = null, int daysBackTo = 0, bool includeReleaseBranches = false)
        {
            Name = name;
            Project = project;
            MaxReturnsPerBranch = maxReturnsPerBranch;
            RepoBranches = repoBranches;
            DefaultBranchesOnly = defaultBranchesOnly;
            DaysBackFrom = daysBackFrom;
            DaysBackTo = daysBackTo;
            IncludeReleaseBranches = includeReleaseBranches;
            GetServices();
        }

        private static void GetServices()
        {
            _gitClient = _tfsInstance.GetClient<GitHttpClient>();
        }

        private GitVersionDescriptor SetDescriptor(string branchName)
        {
            GitVersionDescriptor descriptor = new GitVersionDescriptor();
            descriptor.VersionType = GitVersionType.Branch;
            descriptor.VersionOptions = 0;
            descriptor.Version = branchName;
            return descriptor;
        }

        private void FormatCommits(ref List<CommitFormat> formattedList, List<GitCommitRef> rawCommits, string repoName, string branchName)
        {
            foreach (var _rawCommit in rawCommits)
            {
                CommitFormat commit = new CommitFormat
                {
                    Author = _rawCommit.Author,
                    Branch = branchName,
                    Comment = _rawCommit.Comment,
                    CommitId = _rawCommit.CommitId,
                    Committer = _rawCommit.Committer,
                    ChangeCounts = _rawCommit.ChangeCounts,
                    Repository = repoName,
                    Url = _rawCommit.RemoteUrl
                };
                formattedList.Add(commit);
            }
        }

        private List<string> GetAllBranchNames(string repoName, GitVersionDescriptor versionDescriptor = null)
        {
            List<string> ret = new List<string>();
            var branches = _gitClient.GetBranchesAsync(Project, repoName, versionDescriptor).Result;
            foreach(var _branch in branches)
            {
                ret.Add(_branch.Name);
            }
            return ret;
        }

        private void AddReleaseBranches(ref List<string> branchNameList, string repoName)
        {
            List<string> allBranches = GetAllBranchNames(repoName);
            foreach(var branchName in allBranches)
            {
                if (string.Compare(branchName, 0, _releaseBranchPrefix, 0, _releaseBranchPrefix.Length, false) == 0
                    && !branchNameList.Contains(branchName))
                {
                    branchNameList.Add(branchName);
                }
            }
        }

        private void QueryCommits(ref List<CommitFormat> commitsList, GitRepository repo, GitQueryCommitsCriteria criteria)
        {
            List<string> branchNames = new List<string>();
            if (DefaultBranchesOnly) branchNames.Add(repo.DefaultBranch.Substring(_rawBranchPrefix.Length));
            if (IncludeReleaseBranches) AddReleaseBranches(ref branchNames, repo.Name);
            else if (!DefaultBranchesOnly) branchNames.AddRange(GetAllBranchNames(repo.Name));
            foreach (string _branchName in branchNames)
            {
                criteria.ItemVersion = SetDescriptor(_branchName);
                var result = _gitClient.GetCommitsAsync(Project, repo.Name, criteria, top: MaxReturnsPerBranch).Result;
                FormatCommits(ref commitsList, result, repo.Name, _branchName);
            }
        }

        public override string RunQuery()
        {
            List<CommitFormat> allCommits = new List<CommitFormat>();
            GitQueryCommitsCriteria criteria = new GitQueryCommitsCriteria();
            criteria.FromDate = DaysBackFrom == null ? default(DateTime).ToString() : DateTime.Today.AddDays(-(double)DaysBackFrom).ToString();
            criteria.ToDate = DateTime.Today.AddDays(-(double)DaysBackTo).ToString();

            // dict empty/null. Get all repos in project
            if (RepoBranches == null || RepoBranches.Count == 0)
            {
                var allRepos = _gitClient.GetRepositoriesAsync(Project).Result;
                foreach (var _repo in allRepos)
                {
                    // skip any repos with no code
                    if (_repo.DefaultBranch == null) continue;
                    QueryCommits(ref allCommits, _repo, criteria);
                }
            }

            // dict has at least repos
            else
            {
                foreach (KeyValuePair<string, string[]> pair in RepoBranches)
                {
                    GitRepository _repo = _gitClient.GetRepositoryAsync(Project, pair.Key).Result;
                    // skip any repos with no code
                    if (_repo.DefaultBranch == null) continue;

                    if (pair.Value.Length > 0)
                    {
                        // use user defined branches
                        List<string> branchNames = new List<string>(pair.Value);
                        if (IncludeReleaseBranches) AddReleaseBranches(ref branchNames, _repo.Name);
                        foreach (string _branchName in branchNames)
                        {
                            criteria.ItemVersion = SetDescriptor(_branchName);
                            var result = _gitClient.GetCommitsAsync(Project, _repo.Name, criteria, top: MaxReturnsPerBranch).Result;
                            FormatCommits(ref allCommits, result, _repo.Name, _branchName);
                        }
                    }
                    else
                    {
                        QueryCommits(ref allCommits, _repo, criteria);
                    }
                }
            }

            string json = JsonConvert.SerializeObject(allCommits);
            return json;
        }
    }
}