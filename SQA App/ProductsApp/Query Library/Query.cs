using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.TeamFoundation.Client;
using System.Dynamic;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.Services.Common;
using System.Net;
using SQApp.Models;

namespace SQApp.Query_Library
{
    public abstract class Query
    {
        public const string tfsUri = "http://pdxdevops.esi.com:8080/tfs/ESI";
        protected static TfsTeamProjectCollection _tfsInstance;
        public string Name { get; set; }
        
        public static void connectTFS(TFSCredentials creds)
        {
            // tfsinstance instantiated only once.
            // If fails on first call, tfsinstance not recoverable - latch onto credentials indefinitely to restore connection if needed?
            /*
            string username, password;
            username = Environment.GetEnvironmentVariable("DEVOPS_USR", EnvironmentVariableTarget.User);
            password = Environment.GetEnvironmentVariable("DEVOPS_PSWD", EnvironmentVariableTarget.User);
            if (String.IsNullOrEmpty(username)) username = Environment.GetEnvironmentVariable("DEVOPS_USR", EnvironmentVariableTarget.Process);
            if (String.IsNullOrEmpty(password)) password = Environment.GetEnvironmentVariable("DEVOPS_PSWD", EnvironmentVariableTarget.Process);
            */
            NetworkCredential netCred = new NetworkCredential(new string(creds.Username), new string(creds.Password));
            Microsoft.VisualStudio.Services.Common.WindowsCredential winAuth = new Microsoft.VisualStudio.Services.Common.WindowsCredential(netCred);
            VssCredentials vssAuth = new VssCredentials(winAuth);
            UriBuilder uri = new UriBuilder(tfsUri);
            _tfsInstance = new TfsTeamProjectCollection(uri.Uri, vssAuth);
            _tfsInstance.Authenticate();
        }
         
        public static bool isAuthenticated()
        {
            return _tfsInstance == null ? false : true;
        }

        public abstract string RunQuery();
    }

    // class for a string-based enum
    public static class Filter
    {
        public static readonly string Repositories = "Repositories";
        public static readonly string BuildDefinitions = "BuildDefinitions";
    }

}