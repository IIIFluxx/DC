using Data_Tier.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Data_Tier.Controllers
{
    public class AdminController : ApiController
    {
        [Route("api/Admin/ProcessAllTransactions")]
        [HttpPost]
        public bool ProcessAllTransactions() // Works in Postman ✔️
        {
            try
            {
                Debug.WriteLine("AdminController: Processing transactions............");
                Bank.bankData.ProcessAllTransactions();
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("AdminController: Process Transactions has failed: " + e.Message);
                return false;
            }
        }

        [Route("api/Admin/saveToDisk")]
        [HttpPost]
        public void saveToDisk() // Works in Postman ✔️
        {
            Debug.WriteLine("AdminController: Saving to disk............");
            Bank.bankData.SaveToDisk();
        }
    }
}