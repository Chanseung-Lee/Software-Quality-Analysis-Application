using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System.Text.Json.Serialization; // can use [JsonIgnore] to remove any attributes from serialization

namespace SQApp.Models
{
    // class that contains meaningful information from a WorkItem instance to send to Power BI.
    // Obsolete
    public class WorkItemFormat
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public Int32 AreaId { get; set; }
        public string Description { get; set; }
        public DateTime RevisedDate { get; set; }
        public string CreatedBy { get; set; }
        public string AreaPath { get; set; }
        public string State { get; set; }
        public string TypeName { get; set; } // from WorkItemType
        public string Url { get; set; }
        public string ProjectName { get; set;  }

        public void FormatUrl(string urlFormatString)
        {
            Url = String.Format(urlFormatString, ProjectName, Id);
        }
    }
}