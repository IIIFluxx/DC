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
    public class GetValuesController : ApiController
    {
        // GET api/<controller>/5
        public DataIntermed Get(int id) // You can use this method to return the values from the Data Tier.
        {
            DataModel dm = new DataModel();
            int balance;
            string fName, lName;
            uint acctNo, pin;

            
            //DataIntermed export = new DataIntermed(); // Like a "box" that can easily be serialized and sent elsewhere
                                                      // This is the point of the DataIntermed class.
            dm.GetValuesForEntry(id, out acctNo, out pin, out balance, out fName, out lName);
            //dm.GetValuesForEntry(id, out export.acct, out export.pin, out export.bal, out export.fname, out export.lname);
            DataIntermed output = new DataIntermed();
            output.bal = balance;
            output.fname = fName;
            output.lname = lName;
            output.acct = acctNo;
            output.pin = pin;

            return output;
        }
    }
}