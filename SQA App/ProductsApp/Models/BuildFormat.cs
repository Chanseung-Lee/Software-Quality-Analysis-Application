using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SQApp.Models
{
    public class BuildFormat
    {
        public int Id { get; set; }
        public string BuildNumber { get; set; }
        public string BuildDefinitionName { get; set; }
        public string Result { get; set; }
        public string Status { get; set; }
        public string Url { get; set; }
        public float CodeCoverage { get; set; }
        public DateTime FinishedDate { get; set; }
        public string Branch { get; set; }
        public string Repository { get; set; }
        public TimeSpan RunTime { get; set; }
        public DateTime StartDate { get; set; }
        public int TestsExecuted { get; set; } // Can try to filter out manual tests with run type
        public Object Links { get; set; }
    }

    public class CodeCoverageAvgFormat
    {
        public double AvgCodeCoverage { get; set; }
        public string BuildDefinitionName { get; set; }
        public int BuildCount { get; set; }
        public DateTime Month { get; set; }
    }

}