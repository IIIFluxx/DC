using BankDB;
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
    public class UserAccessController : ApiController
    {
        private UserAccessInterface userFoob = Bank.bankData.GetUserAccess(); // BankDB Function
        /*
         void SelectUser(uint userID); ✔️
         List<uint> GetUsers(); ✖️
         uint CreateUser(); ✔️
         void GetUserName(out string fname, out string lname); ✔️
         void SetUserName(string fname, string lname); ✔️

        */

        [Route("api/UserAccess/selectUser/{userID}")]
        [HttpGet]
        public UserDetailStruct selectUser(uint userID) // Works in Postman ✔️
        {
            Debug.WriteLine("UserAccessController: Getting details for user " + userID);
            string firstname, surname;
            try
            {
                UserDetailStruct userStruct = new UserDetailStruct();
                // Select before editing
                userFoob.SelectUser(userID);
                userFoob.GetUserName(out firstname, out surname);

                // Populate new user details object that we wish to return.
                userStruct.fname = firstname;
                userStruct.lname = surname;
                userStruct.userID = userID;

                // Return object populated with user details.
                return userStruct;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("UserAccessController: Error occurred: " + ex.Message);
                return null;
            }
        }

        //[Route("api/UserAccess/createUser/{fname}/{lname}")]
        [Route("api/UserAccess/createUser/")] // For Postman.
        [Route("api/UserAccess/createUser/{fname}/{lname}")]
        [HttpPost]
        public UserDetailStruct createUser(string fname, string lname) // Works in Postman ✔️
        {
            // Create object to populate and return.
            try
            {
                UserDetailStruct userStruct = new UserDetailStruct();
                userStruct.userID = userFoob.CreateUser();
                userFoob.SelectUser(userStruct.userID);
                userStruct.fname = fname;
                userStruct.lname = lname;

                userFoob.SetUserName(userStruct.fname, userStruct.lname);
                userFoob.GetUserName(out userStruct.fname, out userStruct.lname);

                return userStruct;
            }
            catch(Exception ex)
            {
                Debug.WriteLine("UserAccessController: Error occurred: " + ex.Message);
                return null;
            }
        }
    }
}