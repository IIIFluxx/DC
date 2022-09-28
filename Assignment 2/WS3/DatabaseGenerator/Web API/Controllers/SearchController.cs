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
    public class SearchController : ApiController
    {

        // POST api/<controller>
        [Route("api/search")]
        [HttpPost]
        public DataIntermed Post(SearchData value) // Method names dont need to be Post/Get anymore. It'd be search/<method name> in URL now .
        {
            // just return a DataIntermed object populated with the record you want to send to the GUI
            DataModel dm = new DataModel();

            string fName, lName;
            uint acct, pin;
            int bal, idx;

            lName = value.searchStr;

            //System.Diagnostics.Debug.WriteLine("Last Name: " + lName + "."); -- This works.

            //idx = dm.searchLastName(lName);

            dm.searchLastName(lName, out fName, out acct, out pin, out bal);
            DataIntermed export = new DataIntermed(); // Like a "box" that can easily be serialized and sent elsewhere

            //dm.GetValuesForEntry(idx, out export.acct, out export.pin, out export.bal, out export.fname, out export.lname, out export.icon);
            export.acct = acct;
            export.pin = pin;
            export.fname = fName;
            export.lname = lName;
            export.bal = bal;
            return export;
        }
    }
}