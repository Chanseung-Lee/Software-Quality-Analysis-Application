using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SQApp.Models;
using Microsoft.TeamFoundation.Build.WebApi;

namespace SQApp.Query_Library
{
    public static class QueryLists
    {
        public static Query[] Queries = new Query[]
        {

            /* for retrieving previous monthly averages, ran manually */
            /*
             new TestCoverageAvgQuery("mlcc_test_coverage_monthly", "MLCC",
                                     filterBy: new List<string> { Filter.BuildDefinitions},
                                     resultFilter: BuildResult.Succeeded,
                                     statusFilter: BuildStatus.Completed,
                                     currentMonth: false,
                                     month: 4,
                                     year: 2022),

             new TestCoverageAvgQuery("geode_test_coverage_monthly", "Geode",
                                     filterBy: new List<string> { Filter.BuildDefinitions},
                                     resultFilter: BuildResult.Succeeded,
                                     statusFilter: BuildStatus.Completed,
                                     currentMonth: false,
                                     month: 4,
                                     year: 2022),

            new TestCoverageAvgQuery("flex_test_coverage_monthly", "Flex",
                                     filterBy: new List<string> { Filter.BuildDefinitions},
                                     resultFilter: BuildResult.Succeeded,
                                     statusFilter: BuildStatus.Completed,
                                     currentMonth: false,
                                     month: 4,
                                     year: 2022),
            */

            /* for updating current monthly average */
            new TestCoverageAvgQuery("mlcc_test_coverage_monthly", "MLCC",
                                     filterBy: new List<string> { Filter.BuildDefinitions},
                                     resultFilter: BuildResult.Succeeded,
                                     statusFilter: BuildStatus.Completed,
                                     currentMonth: true),

            new TestCoverageAvgQuery("geode_test_coverage_monthly", "Geode",
                                     filterBy: new List<string> { Filter.BuildDefinitions},
                                     resultFilter: BuildResult.Succeeded,
                                     statusFilter: BuildStatus.Completed,
                                     currentMonth: true),

            new TestCoverageAvgQuery("flex_test_coverage_monthly", "Flex",
                                     filterBy: new List<string> { Filter.BuildDefinitions},
                                     resultFilter: BuildResult.Succeeded,
                                     statusFilter: BuildStatus.Completed,
                                     currentMonth: true),
            
            new CommitQuery("fusion-allRepos-commits", "Fusion",
                            maxReturnsPerBranch: 10000,
                            daysBackFrom: 460,
                            defaultBranchesOnly: true,
                            includeReleaseBranches: true),

            new CommitQuery("fusion-mainRepo-commits", "Fusion",
                            repoBranches: new Dictionary<string, string[]> { { "Fusion", new string[] {} }
                                                                           },
                            maxReturnsPerBranch: 10000,
                            daysBackFrom: 365,
                            defaultBranchesOnly: true,
                            includeReleaseBranches: true),

            new CommitQuery("mlcc-allRepos-commits", "MLCC",
                            maxReturnsPerBranch: 10000,
                            daysBackFrom: 460,
                            defaultBranchesOnly: true,
                            includeReleaseBranches: true),

            new CommitQuery("mlcc-mainRepo-commits", "MLCC",
                            repoBranches: new Dictionary<string, string[]> { { "MLCC", new string[] {"Fu/4.16"} }
                                                                           },
                            maxReturnsPerBranch: 10000,
                            daysBackFrom: 365,
                            defaultBranchesOnly: true,
                            includeReleaseBranches: true),

            new CommitQuery("geode-allRepos-commits", "Geode",
                            maxReturnsPerBranch: 10000,
                            daysBackFrom: 460,
                            defaultBranchesOnly: true,
                            includeReleaseBranches: true),

            new CommitQuery("geode-mainRepo-commits", "Geode",
                            repoBranches: new Dictionary<string, string[]> { { "Geode", new string[] {} }
                                                                           },
                            maxReturnsPerBranch: 10000,
                            daysBackFrom: 365,
                            defaultBranchesOnly: true,
                            includeReleaseBranches: true),

            new CommitQuery("flex-allRepos-commits", "Flex",
                            maxReturnsPerBranch: 10000,
                            daysBackFrom: 460,
                            defaultBranchesOnly: true,
                            includeReleaseBranches: true),

            new CommitQuery("flex-mainRepo-commits", "Flex",
                            repoBranches: new Dictionary<string, string[]> { { "Flex", new string[] {} }
                                                                           },
                            maxReturnsPerBranch: 10000,
                            daysBackFrom: 365,
                            defaultBranchesOnly: true,
                            includeReleaseBranches: true),

            new BuildQuery("mlcc_all_buildDefs_failures", "MLCC",
                            filterBy: new List<string> { Filter.BuildDefinitions},
                            daysBack: 460, // for builds in each definition retrieved
                            statusFilter: BuildStatus.Completed,
                            resultFilter: BuildResult.Failed,
                            calcCodeCoverage: false),

            new BuildQuery("geode_all_buildDefs_failures", "Geode",
                            filterBy: new List<string> { Filter.BuildDefinitions},
                            daysBack: 460, // for builds in each definition retrieved
                            statusFilter: BuildStatus.Completed,
                            resultFilter: BuildResult.Failed,
                            calcCodeCoverage: false),

            new BuildQuery("flex_all_buildDefs_failures", "Flex",
                            filterBy: new List<string> { Filter.BuildDefinitions},
                            //daysBackDefs: 30, // only get definitions that have had a build in last 30 days, getting all works w/ good build filters
                            daysBack: 460, // for builds in each definition retrieved
                            statusFilter: BuildStatus.Completed,
                            resultFilter: BuildResult.Failed,
                            calcCodeCoverage: false),

            new BuildQuery("mlcc_last_20_builds", "MLCC",
                            filterBy: new List<string> {Filter.BuildDefinitions, "35xx_5.2"},
                            allTime: true,
                            maxReturnsPerRepo: 20,
                            statusFilter: BuildStatus.Completed,
                            queryOrder: BuildQueryOrder.FinishTimeDescending,
                            calcCodeCoverage: false),

            new BuildQuery("geode_last_20_builds", "Geode",
                            filterBy: new List<string> {Filter.BuildDefinitions, "Geode_develop_3.0"},
                            allTime: true,
                            maxReturnsPerRepo: 20,
                            statusFilter: BuildStatus.Completed,
                            queryOrder: BuildQueryOrder.FinishTimeDescending,
                            calcCodeCoverage: false),

            new BuildQuery("flex_last_20_builds", "Flex",
                            filterBy: new List<string> {Filter.BuildDefinitions, "Atlas_2017_master"},
                            allTime: true,
                            maxReturnsPerRepo: 20,
                            statusFilter: BuildStatus.Completed,
                            queryOrder: BuildQueryOrder.FinishTimeDescending,
                            calcCodeCoverage: false),

            new BuildQuery("flex_test_coverage", "Flex",
                            filterBy: new List<string> {Filter.BuildDefinitions},
                            allTime: true,
                            resultFilter: BuildResult.Succeeded,
                            statusFilter: BuildStatus.Completed,
                            maxReturnsPerRepo: 30,
                            queryOrder: BuildQueryOrder.FinishTimeDescending,
                            manageDbByDate: false),

            new BuildQuery("geode_test_coverage", "Geode",
                            filterBy: new List<string> {Filter.BuildDefinitions},
                            allTime: true,
                            resultFilter: BuildResult.Succeeded,
                            statusFilter: BuildStatus.Completed,
                            maxReturnsPerRepo: 30,
                            queryOrder: BuildQueryOrder.FinishTimeDescending,
                            manageDbByDate: false),

            new BuildQuery("mlcc_test_coverage", "MLCC",
                            filterBy: new List<string> {Filter.BuildDefinitions},
                            allTime: true,
                            resultFilter: BuildResult.Succeeded,
                            statusFilter: BuildStatus.Completed,
                            maxReturnsPerRepo: 30,
                            queryOrder: BuildQueryOrder.FinishTimeDescending,
                            manageDbByDate: false),

            new BuildQuery("flex_build_failures", "Flex",
                            filterBy: new List<string> {Filter.BuildDefinitions, "Atlas_2017_master"},
                            daysBack: 460,
                            statusFilter: BuildStatus.Completed,
                            resultFilter: BuildResult.Failed,
                            calcCodeCoverage: false),

            new BuildQuery("geode_build_failures", "Geode",
                            filterBy: new List<string> {Filter.BuildDefinitions, "Geode_develop_3.0"},
                            daysBack: 460,
                            statusFilter: BuildStatus.Completed,
                            resultFilter: BuildResult.Failed,
                            calcCodeCoverage: false),

            new BuildQuery("mlcc_build_failures", "MLCC",
                            filterBy: new List<string> {Filter.BuildDefinitions, "35xx_5.2"},
                            daysBack: 460,
                            statusFilter: BuildStatus.Completed,
                            resultFilter: BuildResult.Failed,
                            calcCodeCoverage: false),

            new WIQLQuery("fusion-all_closed_bugs",
                          "SELECT   [System.Id] " +
                          "FROM     WorkItems " +
                          "WHERE    [System.TeamProject] = 'Fusion' " +
                          "AND      [System.WorkItemType] = 'Bug' " +
                          "AND      [System.State] = 'Closed' " +
                          "AND      [Microsoft.VSTS.Common.ClosedDate] >= '2020-06-01T00:00:00.0000000' "),

            new WIQLQuery("mlcc-all_closed_bugs",
                          "SELECT   [System.Id] " +
                          "FROM     WorkItems " +
                          "WHERE    [System.TeamProject] = 'MLCC' " +
                          "AND      [System.WorkItemType] = 'Bug' " +
                          "AND      [System.State] = 'Closed' " +
                          "AND      [Microsoft.VSTS.Common.ClosedDate] >= '2020-06-01T00:00:00.0000000' "),

            new WIQLQuery("geode-all_closed_bugs",
                          "SELECT   [System.Id] " +
                          "FROM     WorkItems " +
                          "WHERE    [System.TeamProject] = 'Geode' " +
                          "AND      [System.WorkItemType] = 'Bug' " +
                          "AND      [System.State] = 'Closed' " +
                          "AND      [Microsoft.VSTS.Common.ClosedDate] >= '2020-06-01T00:00:00.0000000' "),

            new WIQLQuery("flex-all_closed_bugs",
                          "SELECT   [System.Id] " +
                          "FROM     WorkItems " +
                          "WHERE    [System.TeamProject] = 'Flex' " +
                          "AND      [System.WorkItemType] = 'Bug' " +
                          "AND      [System.State] = 'Closed' " +
                          "AND      [Microsoft.VSTS.Common.ClosedDate] >= '2020-06-01T00:00:00.0000000' "),

            new WIQLQuery("fusion-closed_fixed_bugs",
                          "SELECT   [System.Id] " +
                          "FROM     WorkItems " +
                          "WHERE    [System.TeamProject] = 'Fusion' " +
                          "AND      [System.WorkItemType] = 'Bug' " +
                          "AND      [System.State] = 'Closed' " +
                          "AND      ([Microsoft.VSTS.Common.ResolvedReason] = 'Fixed' OR [System.Reason] = 'Fixed and verified') " +
                          "AND      [Microsoft.VSTS.Common.ClosedDate] >= '2020-06-01T00:00:00.0000000' " +
                          "AND not  [System.Tags] contains 'Duplicate' "),

            new WIQLQuery("mlcc-closed_fixed_bugs",
                          "SELECT   [System.Id] " +
                          "FROM     WorkItems " +
                          "WHERE    [System.TeamProject] = 'MLCC' " +
                          "AND      [System.WorkItemType] = 'Bug' " +
                          "AND      [System.State] = 'Closed' " +
                          "AND      ([Microsoft.VSTS.Common.ResolvedReason] = 'Fixed' OR [System.Reason] = 'Fixed and verified') " +
                          "AND      [Microsoft.VSTS.Common.ClosedDate] >= '2020-06-01T00:00:00.0000000' " +
                          "AND not  [System.Tags] contains 'Duplicate' "),

            new WIQLQuery("geode-closed_fixed_bugs",
                          "SELECT   [System.Id] " +
                          "FROM     WorkItems " +
                          "WHERE    [System.TeamProject] = 'Geode' " +
                          "AND      [System.WorkItemType] = 'Bug' " +
                          "AND      [System.State] = 'Closed' " +
                          "AND      ([Microsoft.VSTS.Common.ResolvedReason] = 'Fixed' OR [System.Reason] = 'Fixed and verified') " +
                          "AND      [Microsoft.VSTS.Common.ClosedDate] >= '2020-06-01T00:00:00.0000000' " +
                          "AND not  [System.Tags] contains 'Duplicate' "),

            new WIQLQuery("flex-closed_fixed_bugs",
                          "SELECT   [System.Id] " +
                          "FROM     WorkItems " +
                          "WHERE    [System.TeamProject] = 'Flex' " +
                          "AND      [System.WorkItemType] = 'Bug' " +
                          "AND      [System.State] = 'Closed' " +
                          "AND      ([Microsoft.VSTS.Common.ResolvedReason] = 'Fixed' OR [System.Reason] = 'Fixed and verified') " +
                          "AND      [Microsoft.VSTS.Common.ClosedDate] >= '2020-06-01T00:00:00.0000000' " +
                          "AND not  [System.Tags] contains 'Duplicate' "),

            new WIQLQuery("flex-ready_test_cases",
                          "SELECT   [System.Id] " +
                          "FROM     WorkItems " +
                          "WHERE    [System.TeamProject] = 'Flex' " +
                          "AND      [System.WorkItemType] = 'Test Case' " +
                          "AND      [System.State] = 'Ready' "),

            new WIQLQuery("fusion-closed_valid_bugs_list",
                          "SELECT   [System.Id] " +
                          "FROM     WorkItems " +
                          "WHERE    [System.TeamProject] = 'Fusion' " +
                          "AND      [System.WorkItemType] = 'Bug' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'As Designed' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'Duplicate' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'Obsolete' " +
                          "AND      [System.State] = 'Closed' " +
                          "AND      [Microsoft.VSTS.Common.ClosedDate] >= '2020-06-01T00:00:00.0000000' " +
                          "AND not  [System.Tags] contains 'Duplicate' " +
                          "order by [Microsoft.VSTS.Common.ClosedDate] desc"),

            new WIQLQuery("mlcc-closed_valid_bugs_list",
                          "SELECT   [System.Id] " +
                          "FROM     WorkItems " +
                          "WHERE    [System.TeamProject] = 'MLCC' " +
                          "AND      [System.WorkItemType] = 'Bug' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'As Designed' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'Duplicate' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'Obsolete' " +
                          "AND      [System.State] = 'Closed' " +
                          "AND      [Microsoft.VSTS.Common.ClosedDate] >= '2020-06-01T00:00:00.0000000' " +
                          "AND not  [System.Tags] contains 'Duplicate' " +
                          "order by [Microsoft.VSTS.Common.ClosedDate] desc"),

            new WIQLQuery("mlcc-all_valid_bugs_list",
                          "SELECT   [System.Id] " +
                          "FROM     WorkItems " +
                          "WHERE    [System.TeamProject] = 'MLCC' " +
                          "AND      [System.WorkItemType] = 'Bug' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'As Designed' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'Duplicate' " +
                          "AND      [System.State] <> 'Closed' " +
                          "AND      [System.State] <> 'Removed' " +
                          "order by [Microsoft.VSTS.Common.Severity]"),

            new WIQLQuery("fusion-all_valid_bugs_list",
                          "SELECT   [System.Id] " +
                          "FROM     WorkItems " +
                          "WHERE    [System.TeamProject] = 'Fusion' " +
                          "AND      [System.WorkItemType] = 'Bug' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'As Designed' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'Duplicate' " +
                          "AND      [System.State] <> 'Closed' " +
                          "AND      [System.State] <> 'Removed' " +
                          "order by [Microsoft.VSTS.Common.Severity]"),

            new WIQLQuery("mlcc-open_bug_trends",
                          "select   [System.Id]" +
                          "from     WorkItems " +
                          "where    [System.TeamProject] = 'MLCC' " +
                          "and      [System.WorkItemType] = 'Bug' " +
                          "and      [System.State] <> 'Closed' " +
                          "and      [System.State] <> 'Removed'"),

            new WIQLQuery("geode-closed_valid_bugs_list",
                          "SELECT   [System.Id] " +
                          "FROM     WorkItems " +
                          "WHERE    [System.TeamProject] = 'Geode' " +
                          "AND      [System.WorkItemType] = 'Bug' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'As Designed' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'Duplicate' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'Obsolete' " +
                          "AND      [System.State] = 'Closed' " +
                          "AND      [Microsoft.VSTS.Common.ClosedDate] >= '2020-06-01T00:00:00.0000000' " +
                          "AND not  [System.Tags] contains 'Duplicate' " +
                          "order by [Microsoft.VSTS.Common.ClosedDate] desc"),

            new WIQLQuery("geode-all_valid_bugs_list",
                          "SELECT   [System.Id] " +
                          "FROM     WorkItems " +
                          "WHERE    [System.TeamProject] = 'Geode' " +
                          "AND      [System.WorkItemType] = 'Bug' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'As Designed' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'Duplicate' " +
                          "AND      [System.State] <> 'Closed' " +
                          "AND      [System.State] <> 'Removed' " +
                          "order by [Microsoft.VSTS.Common.Severity]"),

            new WIQLQuery("geode-open_bug_trends",
                          "select   [System.Id]" +
                          "from     WorkItems " +
                          "where    [System.TeamProject] = 'Geode' " +
                          "and      [System.WorkItemType] = 'Bug' " +
                          "and      [System.State] <> 'Closed' " +
                          "and      [System.State] <> 'Removed'"),

            new WIQLQuery("flex-valid_defects",
                          "SELECT   [System.Id] " +
                          "FROM     WorkItems " +
                          "WHERE    [System.TeamProject] = 'Flex' " +
                          "AND      [System.WorkItemType] = 'Bug' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'As Designed' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'Duplicate' " +
                          "AND      [System.State] <> 'Removed' " +
                          "AND not  [System.Tags] contains 'Duplicate' " +
                          "AND      ([System.CreatedDate] >= @today-364 OR [Microsoft.VSTS.Common.ClosedDate] >= @today-364) " +
                          "order by [Microsoft.VSTS.Common.Severity]"),

            new WIQLQuery("geode-valid_defects",
                          "SELECT   [System.Id] " +
                          "FROM     WorkItems " +
                          "WHERE    [System.TeamProject] = 'Geode' " +
                          "AND      [System.WorkItemType] = 'Bug' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'As Designed' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'Duplicate' " +
                          "AND      [System.State] <> 'Removed' " +
                          "AND not  [System.Tags] contains 'Duplicate' " +
                          "AND      ([System.CreatedDate] >= @today-364 OR [Microsoft.VSTS.Common.ClosedDate] >= @today-364) " +
                          "order by [Microsoft.VSTS.Common.Severity]"),

            new WIQLQuery("mlcc-valid_defects",
                          "SELECT   [System.Id] " +
                          "FROM     WorkItems " +
                          "WHERE    [System.TeamProject] = 'MLCC' " +
                          "AND      [System.WorkItemType] = 'Bug' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'As Designed' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'Duplicate' " +
                          "AND      [System.State] <> 'Removed' " +
                          "AND not  [System.Tags] contains 'Duplicate' " +
                          "AND      ([System.CreatedDate] >= @today-364 OR [Microsoft.VSTS.Common.ClosedDate] >= @today-364) " +
                          "order by [Microsoft.VSTS.Common.Severity]"),

            new WIQLQuery("fusion-valid_defects",
                          "SELECT   [System.Id] " +
                          "FROM     WorkItems " +
                          "WHERE    [System.TeamProject] = 'Fusion' " +
                          "AND      [System.WorkItemType] = 'Bug' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'As Designed' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'Duplicate' " +
                          "AND      [System.State] <> 'Removed' " +
                          "AND not  [System.Tags] contains 'Duplicate' " +
                          "AND      ([System.CreatedDate] >= @today-364 OR [Microsoft.VSTS.Common.ClosedDate] >= @today-364) " +
                          "order by [Microsoft.VSTS.Common.Severity]"),

            new WIQLQuery("flex-closed_valid_bugs_list",
                          "SELECT   [System.Id] " +
                          "FROM     WorkItems " +
                          "WHERE    [System.TeamProject] = 'Flex' " +
                          "AND      [System.WorkItemType] = 'Bug' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'As Designed' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'Duplicate' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'Obsolete' " +
                          "AND      [System.State] = 'Closed' " +
                          "AND      [Microsoft.VSTS.Common.ClosedDate] >= '2020-06-01T00:00:00.0000000' " +
                          "AND not  [System.Tags] contains 'Duplicate' " +
                          "order by [Microsoft.VSTS.Common.ClosedDate] desc"),

             new WIQLQuery("flex-all_valid_bugs_list",
                          "SELECT   [System.Id] " +
                          "FROM     WorkItems " +
                          "WHERE    [System.TeamProject] = 'Flex' " +
                          "AND      [System.WorkItemType] = 'Bug' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'As Designed' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'Duplicate' " +
                          "AND      [System.State] <> 'Closed' " +
                          "AND      [System.State] <> 'Removed' " +
                          "order by [Microsoft.VSTS.Common.Severity]"),

            new WIQLQuery("flex-open_bug_trends",
                          "select   [System.Id]" +
                          "from     WorkItems " +
                          "where    [System.TeamProject] = 'Flex' " +
                          "and      [System.WorkItemType] = 'Bug' " +
                          "and      [System.State] <> 'Closed' " +
                          "and      [System.State] <> 'Removed'"),

            new WIQLQuery("mlcc-criticalhigh-bugs",
                          "SELECT   [System.Id] " +
                          "FROM     WorkItems " +
                          "WHERE    [System.TeamProject] = 'MLCC' " +
                          "AND      [System.WorkItemType] = 'Bug' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'As Designed' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'Duplicate' " +
                          "AND      [System.Reason] <> 'Duplicate' " +
                          "AND      ([Microsoft.VSTS.Common.Severity] = '1 - Critical' OR [Microsoft.VSTS.Common.Severity] = '2 - High') " +
                          "AND      [System.CreatedDate] >= @today-460 "),

            new WIQLQuery("geode-criticalhigh-bugs",
                          "SELECT   [System.Id] " +
                          "FROM     WorkItems " +
                          "WHERE    [System.TeamProject] = 'Geode' " +
                          "AND      [System.WorkItemType] = 'Bug' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'As Designed' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'Duplicate' " +
                          "AND      [System.Reason] <> 'Duplicate' " +
                          "AND      ([Microsoft.VSTS.Common.Severity] = '1 - Critical' OR [Microsoft.VSTS.Common.Severity] = '2 - High') " +
                          "AND      [System.CreatedDate] >= @today-460 "),

            new WIQLQuery("flex-criticalhigh-bugs",
                          "SELECT   [System.Id] " +
                          "FROM     WorkItems " +
                          "WHERE    [System.TeamProject] = 'Flex' " +
                          "AND      [System.WorkItemType] = 'Bug' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'As Designed' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'Duplicate' " +
                          "AND      [System.Reason] <> 'Duplicate' " +
                          "AND      ([Microsoft.VSTS.Common.Severity] = '1 - Critical' OR [Microsoft.VSTS.Common.Severity] = '2 - High') " +
                          "AND      [System.CreatedDate] >= @today-460 "),

            new WIQLQuery("fusion-criticalhigh-bugs",
                          "SELECT   [System.Id] " +
                          "FROM     WorkItems " +
                          "WHERE    [System.TeamProject] = 'Fusion' " +
                          "AND      [System.WorkItemType] = 'Bug' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'As Designed' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'Duplicate' " +
                          "AND      [System.Reason] <> 'Duplicate' " +
                          "AND      ([Microsoft.VSTS.Common.Severity] = '1 - Critical' OR [Microsoft.VSTS.Common.Severity] = '2 - High') " +
                          "AND      [System.CreatedDate] >= @today-460 "),

            new WIQLQuery("fusion-escapes",
                          "SELECT   [System.Id] " +
                          "FROM     WorkItems " +
                          "WHERE    [System.TeamProject] = 'Fusion' " +
                          "AND      [System.WorkItemType] = 'Bug' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'As Designed' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'Duplicate' " +
                          "AND      [System.Reason] <> 'Duplicate' " +
                          "and      [System.State] <> 'Removed' " +
                          "AND      ([ESI.BugHowFound] = 'Customer' OR [ESI.BugHowFound] = 'Tech Support or Service' OR [ESI.BugHowFound] = 'Field Apps') " +
                          "AND      [System.CreatedDate] >= @today-460 "),

            new WIQLQuery("mlcc-escapes",
                          "SELECT   [System.Id] " +
                          "FROM     WorkItems " +
                          "WHERE    [System.TeamProject] = 'MLCC' " +
                          "AND      [System.WorkItemType] = 'Bug' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'As Designed' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'Duplicate' " +
                          "AND      [System.Reason] <> 'Duplicate' " +
                          "and      [System.State] <> 'Removed' " +
                          "AND      ([ESI.BugHowFound] = 'Customer' OR [ESI.BugHowFound] = 'Tech Support or Service' OR [ESI.BugHowFound] = 'Field Apps') " +
                          "AND      [System.CreatedDate] >= @today-460 "),

            new WIQLQuery("geode-escapes",
                          "SELECT   [System.Id] " +
                          "FROM     WorkItems " +
                          "WHERE    [System.TeamProject] = 'Geode' " +
                          "AND      [System.WorkItemType] = 'Bug' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'As Designed' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'Duplicate' " +
                          "AND      [System.Reason] <> 'Duplicate' " +
                          "and      [System.State] <> 'Removed' " +
                          "AND      ([ESI.BugHowFound] = 'Customer' OR [ESI.BugHowFound] = 'Tech Support or Service' OR [ESI.BugHowFound] = 'Field Apps') " +
                          "AND      [System.CreatedDate] >= @today-460 "),

            new WIQLQuery("flex-escapes",
                          "SELECT   [System.Id] " +
                          "FROM     WorkItems " +
                          "WHERE    [System.TeamProject] = 'Flex' " +
                          "AND      [System.WorkItemType] = 'Bug' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'As Designed' " +
                          "AND      [Microsoft.VSTS.Common.ResolvedReason] <> 'Duplicate' " +
                          "AND      [System.Reason] <> 'Duplicate' " +
                          "and      [System.State] <> 'Removed' " +
                          "AND      ([ESI.BugHowFound] = 'Customer' OR [ESI.BugHowFound] = 'Tech Support or Service' OR [ESI.BugHowFound] = 'Field Apps') " +
                          "AND      [System.CreatedDate] >= @today-460 ")
        };

    }
}
