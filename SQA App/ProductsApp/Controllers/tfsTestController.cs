using SQApp.Models;
using SQApp.Query_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Client;
// using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.VisualStudio.Services.Common;

namespace SQApp.Controllers
{
    public class tfsTestController : ApiController
    {

        public IHttpActionResult PostConnectTFS(TFSCredentials connect)
        {
            if (Query.isAuthenticated())
            {
                return Ok("Valid Credentials have already been provided");
            }
            if (!ModelState.IsValid) return BadRequest("Invalid body contents. Please provide Username and Password");
            try
            {
                // test credentials on non-static object
                NetworkCredential netCred = new NetworkCredential(new string(connect.Username), new string(connect.Password));
                Microsoft.VisualStudio.Services.Common.WindowsCredential winAuth = new Microsoft.VisualStudio.Services.Common.WindowsCredential(netCred);
                VssCredentials vssAuth = new VssCredentials(winAuth);
                UriBuilder uri = new UriBuilder(Query.tfsUri);
                TfsTeamProjectCollection tfs = new TfsTeamProjectCollection(uri.Uri, vssAuth);
                tfs.Authenticate(); // throw error if credentials invalid
                // authenticate actual tfsInstance
                Query.connectTFS(connect);
                return Ok("Connected Succesfully");
            }
            catch (Microsoft.TeamFoundation.TeamFoundationServerUnauthorizedException)
            {
                //Exception res = new UnauthorizedAccessException(e + " User: " + new string(connect.Username));
                //return InternalServerError(res);
                return BadRequest("Invalid Credentials. Credentials provided are not authorized to access the on-premise TFS (DevOps)");
            }
            catch(Exception e)
            {
                return InternalServerError(e);
            }
        }

        public string GetWorkItems()
        {
            //return TestConnection.connectWorkItems();
            // return TestConnection.connectBuilds();
            // return TestConnection.GetAlreadyMadeQueries();
            return TestConnection.testDomains();
        }
    }
}