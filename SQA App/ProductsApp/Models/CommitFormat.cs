using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace SQApp.Models
{
    public class CommitFormat
    {
        public GitUserDate Author { get; set; }
        public string Branch { get; set; } // null if not specifiable
        public string Comment { get; set; }
        public string CommitId { get; set; }
        public GitUserDate Committer { get; set; }
        public ChangeCountDictionary ChangeCounts { get; set; }
        public string Repository { get; set; }
        public string Url { get; set; }
    }
}