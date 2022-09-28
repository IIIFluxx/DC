using Data_Tier.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace Business_Tier.Controllers
{
    public class BankAPIController : ApiController
    {
        private RestClient client;
        private string URL = "https://localhost:44328/";
        private static uint numTrans = 0;

        // ======== Admin Controller ========
        // Process Transactions IN: n/a | OUT: boolean indicating success of processing.
        [Route("api/BankAPI/ProcessTrans")]
        [HttpPost]
        public bool ProcessTrans()
        {
            Debug.WriteLine("/BankAPI/: Attempting to process transactions");
            bool status;
            RestRequest request = new RestRequest("api/Admin/ProcessAllTransactions");
            // Fire off the rest request - POST
            IRestResponse resp = client.Post(request);
            status = JsonConvert.DeserializeObject<bool>(resp.Content);
            return status;
        }

        // ======== Account Controller ========

        // Get Account Details IN: (uint userID) | OUT: AccountDetailStruct
        // GET method
        [Route("api/BankAPI/GetAccDet/{accountID}")] // For URL
        [Route("api/BankAPI/GetAccDet")] // For Postman
        [HttpGet]
        public AccountDetailStruct GetAccDet(uint accountID)
        {
            AccountDetailStruct export = new AccountDetailStruct();
            int parsed;
            try
            {
                // Check if account ID is a number.
                if (Int32.TryParse(accountID.ToString(), out parsed))
                {
                    //System.Diagnostics.Debug.WriteLine("Entered if statement"); // Debug
                    client = new RestClient(URL);
                    RestRequest request = new RestRequest("api/Account/" + accountID.ToString());
                    // Fire off the rest request - GET
                    IRestResponse resp = client.Get(request);
                    export = JsonConvert.DeserializeObject<AccountDetailStruct>(resp.Content);
                    return export;
                }

                return export;
            }
            catch (NullReferenceException nex)
            {
                Debug.WriteLine("/BankAPI/: Error occurred: " + nex.Message);
                return null;
            }
        }

        // Deposit Money IN: (uint accountID, uint amount) | OUT:  deposited amount - uint
        [Route("api/BankAPI/Deposit")]
        [Route("api/BankAPI/Deposit/{accountID}/{amount}")]
        [HttpPost]
        public uint Deposit(uint accountID, uint amount)
        {
            uint deposited = 0;
            int parsed1, parsed2;
            if (Int32.TryParse(accountID.ToString(), out parsed1) && Int32.TryParse(amount.ToString(), out parsed2))
            {
                //System.Diagnostics.Debug.WriteLine("Entered if statement"); // Debug
                client = new RestClient(URL);
                // "api/Account/DepositAmt/{accountID}/{amount}"
                RestRequest request = new RestRequest("api/Account/DepositAmt/" + accountID.ToString() + "/" + amount.ToString());
                // Fire off the rest request - POST
                IRestResponse resp = client.Post(request);
                deposited = JsonConvert.DeserializeObject<uint>(resp.Content); //TODO: Check

                // Save to Disk to ensure our deposit gets kept track of.
                // "api/Admin/saveToDisk"
                RestRequest savePlease = new RestRequest("api/Admin/saveToDisk");
                // Fire off the rest request - POST
                client.Post(savePlease);

                return deposited;
            }
            else
            {
                Debug.WriteLine("/BankAPI/: No money was deposited. Error in uint parameters. ");
                return 0; // No money deposited
            }
        }

        // Withdraw Money IN: (uint accountID, uint amount) | OUT: withdrawn amount - uint 
        [Route("api/BankAPI/Withdraw")]
        [Route("api/BankAPI/Withdraw/{accountID}/{amount}")]
        [HttpPost]
        public uint Withdraw(uint accountID, uint amount)
        {
            uint withdrawn = 0;
            int parsed1, parsed2;
            if (Int32.TryParse(accountID.ToString(), out parsed1) && Int32.TryParse(amount.ToString(), out parsed2))
            {
                //System.Diagnostics.Debug.WriteLine("Entered if statement"); // Debug
                client = new RestClient(URL);
                // "api/Account/WithdrawAmt/{accountID}/{amount}"
                RestRequest request = new RestRequest("api/Account/WithdrawAmt/" + accountID.ToString() + "/" + amount.ToString());
                // Fire off the rest request - POST
                IRestResponse resp = client.Post(request);
                withdrawn = JsonConvert.DeserializeObject<uint>(resp.Content); //TODO: Check

                // Save to Disk to ensure our withdrawal gets kept track of.
                // "api/Admin/saveToDisk"
                RestRequest savePlease = new RestRequest("api/Admin/saveToDisk");
                // Fire off the rest request - POST
                client.Post(savePlease);
                return withdrawn;
            }
            else
            {
                Debug.WriteLine("/BankAPI/: No money was withdrawn. Error in uint parameters. ");
                return 0; // No money withdrawn
            }
        }

        // ======== Transaction Controller ========
        // Create Transaction IN: (uint amount, uint senderID, uint recipientID) | OUT: bool indicating success of transaction.
        [Route("api/BankAPI/CreateTrans")]
        [Route("api/BankAPI/CreateTrans/{amount}/{senderID}/{recipientID}")]
        [HttpPost]
        public bool CreateTrans(uint amount, uint senderID, uint recipientID)
        {
            int parsed1, parsed2, parsed3;
            bool condition = false;
            bool outcome = false;
            if (Int32.TryParse(amount.ToString(), out parsed1)
                && Int32.TryParse(senderID.ToString(), out parsed2)
                && Int32.TryParse(recipientID.ToString(), out parsed3) && amount >= 0)
            {
                client = new RestClient(URL);
                //System.Diagnostics.Debug.WriteLine("URL = " + URL + "api/Transactions/createTransaction/" + amount.ToString() + "/" + senderID.ToString() + "/" + recipientID.ToString());
                RestRequest restRequest = new RestRequest("api/Transactions/createTransaction/" + amount.ToString() + "/" + senderID.ToString() + "/" + recipientID.ToString());
                // Fire off the rest request - POST
                IRestResponse resp = client.Post(restRequest);
                // Method returns a boolean indicating success of transaction.

                condition = JsonConvert.DeserializeObject<bool>(resp.Content);

                if (condition == false) // If create transaction fails
                {
                    return false;
                }
                else // If transaction succeeds, process it.
                {

                    if (numTrans == 3) // Process every 3 transactions.
                    {
                        ProcessTrans();
                        numTrans = 0;
                    }
                    else
                    {
                        numTrans++;
                    }


                    // "api/Admin/saveToDisk"
                    RestRequest savePlease = new RestRequest("api/Admin/saveToDisk");
                    // Fire off the rest request - POST
                    client.Post(savePlease);

                    //outcome = ProcessTrans();
                    /*outcome = true;
                    if(outcome == false)
                    {
                        System.Diagnostics.Debug.WriteLine("Transaction failed");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Transaction succeeded.");
                        // If worked, save to disk.
                        // "api/Admin/saveToDisk"
                        RestRequest savePlease = new RestRequest("api/Admin/saveToDisk");
                        // Fire off the rest request - POST
                        client.Post(savePlease);
                    }*/
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Transaction parameters are not valid uints.");
            }
            //System.Diagnostics.Debug.WriteLine("CreateTrans return = " + outcome);
            return condition;
        }

        // Get Transaction IN: (uint transactionID) | OUT: TransactionDetailStruct
        // GET method
        [Route("api/BankAPI/GetTrans")]
        [Route("api/BankAPI/GetTrans/{transactionID}")]
        [HttpGet]
        public TransactionDetailStruct GetTrans(uint transactionID)
        {
            //System.Diagnostics.Debug.WriteLine("GetTrans trans ID = " + transactionID + ".");
            try
            {
                client = new RestClient(URL);
                //System.Diagnostics.Debug.WriteLine("RestRequest URL  = api/Transactions/" + transactionID.ToString());
                RestRequest request = new RestRequest("api/Transactions/" + transactionID.ToString());
                IRestResponse resp = client.Get(request);
                TransactionDetailStruct transStruct = JsonConvert.DeserializeObject<TransactionDetailStruct>(resp.Content);
                //System.Diagnostics.Debug.WriteLine("TransStruct returned: trans SID = " + transStruct.transactionID);
                return transStruct;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("/BankAPI/: Error occurred in getting transaction: " + ex.Message);
                return null;
            }
        }



        // ======== User Access Controller ========
        // Create User IN: (string fname, string lname) | OUT: UserDetailStruct
        [Route("api/BankAPI/CreateUserAcc")]
        [Route("api/BankAPI/CreateUserAcc/{fname}/{lname}")]
        [HttpPost]
        public UserDetailStruct CreateUserAcc(string fname, string lname)
        {
            try
            {
                if (isLetters(fname) == false || isLetters(lname) == false)
                {
                    // ERROR:
                    Debug.WriteLine("/BankAPI/: Invalid parameters for name provided - please correct.");
                    return null;
                }
                else
                {
                    client = new RestClient(URL);

                    // "api/UserAccess/createUser" -- Returns UserDetailStruct.

                    // Fire off the rest request - POST
                    RestRequest request = new RestRequest("api/UserAccess/createUser/" + fname + "/" + lname);
                    IRestResponse resp = client.Post(request);
                    // Initialise return from method to a UDS.
                    UserDetailStruct uds = JsonConvert.DeserializeObject<UserDetailStruct>(resp.Content);

                    // Now we make an account from that user struct that contains: userID, fname, lname;
                    // Fire off the rest request - POST
                    RestRequest request2 = new RestRequest("api/Account/createAcct/" + uds.userID.ToString());
                    client.Post(request2);

                    // Now we save all additions/changes to disk.
                    // "api/Admin/saveToDisk"
                    RestRequest savePlease = new RestRequest("api/Admin/saveToDisk");
                    // Fire off the rest request - POST
                    client.Post(savePlease);

                    return uds;
                }
            }
            catch (NullReferenceException)
            {
                Debug.WriteLine("/BankAPI/: Null reference has been made.");
                return null;
            }
        }

        // REFERENCE: https://stackoverflow.com/a/6017834/15872054
        private bool isLetters(String inStr)
        {
            return Regex.IsMatch(inStr, @"^[a-zA-Z]+$");
        }


        // Select User IN: (uint userID) | OUT: UserDetailStruct
        // GET method
        [Route("api/BankAPI/SelectUser")]
        [Route("api/BankAPI/SelectUser/{userID}")]
        [HttpGet]
        public UserDetailStruct SelectUser(uint userID)
        {
            UserDetailStruct export = new UserDetailStruct();
            int parsed;
            // Check if account ID is a number.
            if (Int32.TryParse(userID.ToString(), out parsed))
            {
                //System.Diagnostics.Debug.WriteLine("Entered if statement"); // Debug
                client = new RestClient(URL);
                RestRequest request = new RestRequest("api/UserAccess/selectUser/" + userID.ToString());
                // Fire off the rest request - GET
                IRestResponse resp = client.Get(request);
                export = JsonConvert.DeserializeObject<UserDetailStruct>(resp.Content);
                return export;
            }
            else
            {
                return null;
            }
        }

    }
}