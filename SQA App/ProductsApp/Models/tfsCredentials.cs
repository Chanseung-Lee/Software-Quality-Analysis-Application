using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.TeamFoundation.Client;
using Microsoft.VisualStudio.Services.Common;
using System.Net;

namespace SQApp.Models
{
    // Used for body in POST request
    public class TFSCredentials
    {
        public char[] Username { get; set; }
        public char[] Password { get; set; }
        ~TFSCredentials()
        {
            // clear credentials just in case
            Array.Clear(Username, 0, Username.Length);
            Array.Clear(Password, 0, Password.Length);
            // make arrays eligible for collection to remove credentials
            Username = null;
            Password = null;
        }
    }

   /* public static class TFSInstance
    {
        public static bool connected = false;
        public static TfsTeamProjectCollection _tfsInstance;
        public const string tfsUri = "http://pdxdevops.esi.com:8080/tfs/ESI";
        public static void connectTFS(TFSCredentials creds)
        {
            // only needs env vars when api is called for the fisrt time - latches onto tfsinstance for indefinite time.
            // If fails on first call, tfsinstance not recoverable - latch onto credentials indefinitely to restore connection if needed?
            // catching for whether pipeline puts temp variables in User space or process space.

            NetworkCredential netCred = new NetworkCredential(new string(creds.Username), new string(creds.Password));
            Microsoft.VisualStudio.Services.Common.WindowsCredential winAuth = new Microsoft.VisualStudio.Services.Common.WindowsCredential(netCred);
            VssCredentials vssAuth = new VssCredentials(winAuth);
            UriBuilder uri = new UriBuilder(tfsUri);
            _tfsInstance = new TfsTeamProjectCollection(uri.Uri, vssAuth);
            _tfsInstance.Authenticate();
            connected = true;
        }

    } */

}