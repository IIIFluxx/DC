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
    public class TransactionController : ApiController
    {
        private TransactionAccessInterface tranFoob = Bank.bankData.GetTransactionInterface();
        /*
        void SelectTransaction(uint TransactionID); ✔️
        List<uint> GetTransactions(); ✖️
        uint CreateTransaction(); ✔️
        uint GetAmount(); ✔️
        uint GetSendrAcct(); ✔️
        uint GetRecvrAcct(); ✔️
        void SetAmount(uint amount); ✔️
        void SetSendr(uint acctID); ✔️
        void SetRecvr(uint acctID); ✔️
     */
        [Route("api/Transactions/createTransaction/{amount}/{senderID}/{recipientID}")]
        [Route("api/Transactions/createTransaction/")] // For Postman.
        [HttpPost] // Works in Postman ✔️
        public bool createTransaction(uint amount, uint senderID, uint recipientID) // Account ID's 
        {
            //System.Diagnostics.Debug.WriteLine("Params: amount=" + amount + " sdrID=" + senderID + " recipID=" + recipientID);
            TransactionDetailStruct transStruct = new TransactionDetailStruct();
            AccountAccessInterface acctFb = Bank.bankData.GetAccountInterface();
            Debug.WriteLine("TransactionController: Transacting $" + amount + " from Account: " + senderID + " to " + recipientID);
            try
            {
                // Select the account we send money from.
                acctFb.SelectAccount(senderID);
                uint curBal = acctFb.GetBalance();

                if (amount < curBal) // Restriction
                {
                    /*  Populate transaction object with relevant fields:
                     *  transactionID, senderID, recipientID & amount. 
                     */
                    transStruct.transactionID = tranFoob.CreateTransaction();
                    transStruct.senderID = senderID;
                    transStruct.recipientID = recipientID;
                    transStruct.amount = amount;
                    // Select before editing & populate transaction with object we made:
                    tranFoob.SelectTransaction(transStruct.transactionID);
                    tranFoob.SetSendr(transStruct.senderID);
                    tranFoob.SetRecvr(transStruct.recipientID);
                    tranFoob.SetAmount(transStruct.amount);

                    System.Diagnostics.Debug.WriteLine("Transaction ID: " + transStruct.transactionID);
                    return true;
                }
                else
                {
                    Debug.WriteLine("TransactionController: Insufficient funds in account!");
                    return false;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine("TransactionController: Transaction creation failed: " + ex.Message);
                return false;
            }
            //return false;
        }



        [Route("api/Transactions/{transactionID}")]
        [HttpGet]
        public TransactionDetailStruct GetTransactions(uint transactionID) // Works in Postman ✔️
        {
            //System.Diagnostics.Debug.WriteLine("TID in method = " + transactionID);
            Debug.WriteLine("TransactionController: Getting details for transaction " + transactionID);
            TransactionDetailStruct transStruct = new TransactionDetailStruct();
            try
            {
                tranFoob.SelectTransaction(transactionID); // Select before editing.
                //System.Diagnostics.Debug.WriteLine("Reached in method.");
                /*  Populate transaction object with relevant fields:
                 *  transactionID, senderID, recipientID & amount. 
                 */

                // Use uint GetAmount(), uint GetSendrAcct() & uint GetRecvrAcct()
                transStruct.transactionID = transactionID;
                transStruct.amount = tranFoob.GetAmount();
                transStruct.senderID = tranFoob.GetSendrAcct();
                transStruct.recipientID = tranFoob.GetRecvrAcct();

                // Return object we populate.
                return transStruct;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("TransactionController: Getting Transaction details failed: " + ex.Message);
                return null;
            }
        }

    }

}