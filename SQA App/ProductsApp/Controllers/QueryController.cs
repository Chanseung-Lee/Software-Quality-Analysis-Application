using SQApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using SQApp.Query_Library;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SQApp.Controllers
{
    public class QueryController : ApiController
    {

        // return all supported queries by name 
        public IHttpActionResult GetValidQueries()
        {
            if (!Query.isAuthenticated())
            {
                return BadRequest("TFS instance has not authenticated yet. Please connect to TFS via api/tfsTest/connect");
            }
            try
            {
                List<string> queryNames = new List<string>();
                foreach (Query _q in QueryLists.Queries)
                {
                    queryNames.Add(_q.Name);
                }
                return Ok(queryNames);
            }
            catch(Exception e)
            {
                return InternalServerError(e);
            }
        }

        public IHttpActionResult GetQuery(string queryName)
        {
            //ProductsApp.Models.SqliteDataAccess.AddToWI("Ayaka");
            //ProductsApp.Models.SqliteDataAccess.ClearBTable();

            if (!Query.isAuthenticated())
            {
                return BadRequest("TFS instance has not authenticated yet. Please connect to TFS via api/tfsTest/connect");
            }
            // currently double serializes. TODO: change response method to not serialize result from RunQuery()
            HttpRequestMessage returnMsg = new HttpRequestMessage();
            var query = QueryLists.Queries.FirstOrDefault((q) => q.Name == queryName);
            
            /*if (query == null)
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }
            return query.RunQuery();*/
            if (query == null) return NotFound();
            try
            {
                string res = query.RunQuery();
                return Ok(res);
            }
            catch(Exception e)
            {
                return InternalServerError(e);
            }

        }
    }
}