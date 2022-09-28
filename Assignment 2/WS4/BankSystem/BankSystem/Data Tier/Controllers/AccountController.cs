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
    public class AccountController : ApiController
    {
        private AccountAccessInterface accFoob = Bank.bankData.GetAccountInterface(); // BankDB Function

        /*
            void SelectAccount(uint accountID); ✔️
            List<uint> GetAccountIDsByUser(uint userID); ✖️
            uint CreateAccount(uint userID); ✔️
            void Deposit(uint amount); ✔️
            void Withdraw(uint amount); ✔️
            uint GetBalance(); ✔️
            uint GetOwner(); ✔️        */

        /* From WS:
        [Route("api/Account/{accountID}")]
        [HttpGet]
        public AccountDetailStruct GetAccountDetails (uint accountID)
         */
        [Route("api/Account/{userID}")]
        [HttpGet]
        public AccountDetailStruct GetAccountDetails(uint userID) // Works in Postman ✔️
        {
            try
            {
                AccountDetailStruct accStruct = new AccountDetailStruct(); // public uint acctID, userID, public double acctBalance;
                Debug.WriteLine("AccountController: Retrieving account details for desired acc ID = " + userID);
                // Need to 'Select' before you can edit it.
                accFoob.SelectAccount(userID);

                // Assign fields to struct object.
                accStruct.acctID = userID;
                accStruct.userID = accFoob.GetOwner();
                accStruct.acctBalance = accFoob.GetBalance();
                // Return struct.
                return accStruct;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error occurred: " + e.Message);
                return null;
            }
        }

        [Route("api/Account/createAcct/{userID}")]
        [Route("api/Account/createAcct/")] // For Postman. Applies to all methods below.
        [HttpPost]
        public AccountDetailStruct createAcct(uint userID) // Works in Postman ✔️
        {
            // uint CreateAccount(uint userID);
            uint acctID = accFoob.CreateAccount(userID);
            
            Debug.WriteLine("AccountController: Creating acc: ID = " + userID);

            AccountDetailStruct accStruct = new AccountDetailStruct(); // public uint acctID, userID, public double acctBalance;
            // Need to 'Select' before you can edit it.
            accFoob.SelectAccount(userID); // Check --- use accFoob here or make a new one?
            
            // Populate our struct with userID from Parameter, accID from return from CreateAccount() & Bal.
            accStruct.userID = userID;
            accStruct.acctID = acctID;
            accStruct.acctBalance = 0; // They start off with 0 Balance obviously.

            return accStruct;
        }

        [Route("api/Account/DepositAmt/{accountID}/{amount}")]
        [Route("api/Account/DepositAmt/")]
        [HttpPost]
        public uint DepositAmt(uint accountID, uint amount) // Works in Postman ✔️
        {
            Debug.WriteLine("AccountController: Depositing $" + amount + " for Account: " + accountID);
            try
            {
                accFoob.SelectAccount(accountID); // Select before editing
                accFoob.Deposit(amount);
                return amount;
            }
            catch (Exception e)
            {
                Debug.WriteLine("AccountController: Deposit failed: " + e.Message);
                return 0;
            }
        }


        [Route("api/Account/WithdrawAmt/{accountID}/{amount}")]
        [Route("api/Account/WithdrawAmt/")]
        [HttpPost]
        public uint WithdrawAmt(uint accountID, uint amount) // Works in Postman ✔️
        {
            Debug.WriteLine("AccountController: Withdrawing $" + amount + " for Account: " + accountID);
            try
            {
                accFoob.SelectAccount(accountID); // Select before editing
                accFoob.Withdraw(amount);
                return amount;
            }
            catch (Exception e)
            {
                Debug.WriteLine("AccountController: Withdraw failed: " + e.Message);
                return 0;
            }
        }
    }
}
