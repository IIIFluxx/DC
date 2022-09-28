using API_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Web_API.Models;

namespace Web_API.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/<controller>
        public int Get() 
        {
            //return new string[] { "value1", "value2" };
            // DataModel - Singleton w/ access to WCF 
            // Controller -- access things from DataModel.
            // This is the default GET request that returns the num. of entries in our database.
            int numE;
            DataModel dm = new DataModel();
            numE =  dm.GetNumEntries(); //When I do /api/values/2, it executes THIS LINE. -- 100000 
            return numE;
        }

        // POST api/<controller>
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }

    /*
        This is essentially self documenting. But essentially if you start a new debug instance of your web
        service, and direct the url to /api/values (or whatever the first part of the controller name is in your
        case), it will return raw JSON.

        Anything you return from the API controller methods will be converted in the backend into JSON
        as best as .NET can manage it.
     */
}