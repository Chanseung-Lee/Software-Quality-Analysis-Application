using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.TeamFoundation.Framework.Client;
using SQApp.Models;
using System.Text.Json;
using Newtonsoft.Json;
using System.Dynamic;

namespace SQApp.Query_Library
{
    public class WIQLQuery : Query
    { 
        private const string _workItemUrlFormat = "http://pdxdevops.esi.com:8080/tfs/ESI/{0}/_workitems/edit/{1}";
        private static WorkItemStore _workItems;

        public string Query { get; set; }

        public WIQLQuery()
        {
            Name = string.Empty;
            Query = string.Empty;
            GetServices();
        }

        public WIQLQuery(string name, string query)
        {
            Name = name;
            Query = query;
            GetServices();
        }

        private static void GetServices()
        {
            _workItems = (WorkItemStore)_tfsInstance.GetService(typeof(WorkItemStore));
        }

        // Obsolete
        public WorkItemFormat[] FormatWorkItemList(WorkItemCollection list)
        {
            WorkItemFormat[] workItemList = new WorkItemFormat[list.Count];
            int i = 0;
            foreach(WorkItem _item in list)
            {
                WorkItemFormat tmp = new WorkItemFormat
                {
                    Id = _item.Id,
                    Title = _item.Title,
                    CreatedDate = _item.CreatedDate,
                    AreaId = _item.AreaId,
                    Description = _item.Description,
                    RevisedDate = _item.RevisedDate,
                    CreatedBy = _item.CreatedBy,
                    AreaPath = _item.AreaPath,
                    State = _item.State,
                    TypeName = _item.Type.Name,
                    ProjectName = _item.Project.Name
                };
                tmp.FormatUrl(_workItemUrlFormat);
                workItemList[i] = tmp;
                i++;
            }
            return workItemList;
        }

        private string CreateUrl(dynamic item)
        {
            Object projectName, workItemId;
            if (!((IDictionary<string, Object>)item).TryGetValue("Team Project", out projectName)) return null;
            if (!((IDictionary<string, Object>)item).TryGetValue("ID", out workItemId)) return null;
            return String.Format(_workItemUrlFormat, projectName, workItemId);
        }

        // Retrieves all possible fields for all possible work items.
        //
        // Notes: 
        //      - Reason for making a new object for each workItem is because a query
        //        can have different work Item Types with different fields. Same work items
        //        also have different fields depending on project.
        //
        //      - To reduce size of returns, can remove all html fields by checking if value contains
        //        <div> in the beginning.
        public List<dynamic> FormatWorkItems(WorkItemCollection list)
        {
            var queryResults = new List<dynamic>();
            foreach (WorkItem _item in list)
            {
                dynamic itemLayout = new ExpandoObject();
                // get all dynamic key value pairs
                foreach (Field _field in _item.Fields)
                {
                    ((IDictionary<string, Object>)itemLayout).Add(_field.Name, _field.Value);
                }
                // add url key + value for work item
                itemLayout.Url = CreateUrl(itemLayout);
                queryResults.Add(itemLayout);
            }
            return queryResults;
        }

        public override string RunQuery()
        {
            WorkItemCollection workItemList = _workItems.Query(Query);

            /*
            SqliteDataAccess.AddToDB("Yelan");
            foreach (WorkItem _item in workItemList)
            {
                SqliteDataAccess.AddToWI(_item.Title);
            }
            */

            //SqliteDataAccess.ClearWITable();

            // need to have uniform return type b/c c# 7.3 doesn't support covariant returns
            string json = JsonConvert.SerializeObject(FormatWorkItems(workItemList));
            return json;
        }
    }
}