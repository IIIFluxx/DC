using API_Classes;
using Miner.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web.Http;

namespace Miner.Controllers
{
    public delegate void ProcTrans();

    public class MinerController : ApiController
    {
        // Accepts transactions from the Transaction Generator & generates blocks to add those transactions to the blockchain

        // The web service needs to allow the transaction program to asynchronously add new transactions to the miner.

        // The mining part of the miner will need to mine the actual hashes for the blocks,
        // and will need to run in the background if there are blocks to mine

        private static bool notRunning = true;
        private static Queue<Transaction> transactions = new Queue<Transaction>();
        private string URL = "https://localhost:44324/"; // URL = Blockchain Server 

        [Route("api/Miner/AddTransaction/")]
        [HttpPost]
        public void AddTransaction(Transaction t)
        {
            if (notRunning)
            {
                ProcTrans procDel = ProcessTransactions;
                procDel.BeginInvoke(null, null);
                notRunning = false;
            }

            transactions.Enqueue(t);
        }


        private void ProcessTransactions()
        {
            RestClient cl = new RestClient(URL);

            while (true)
            {
                try
                {
                    if (transactions.Count > 0)
                    {
                        Transaction t = transactions.Dequeue();

                        if (!t.processed) // Get only the non-processed 
                        {
                            Debug.WriteLine("Processing transaction for: " + t.ToString());

                            /*
                             * 1. Validate the transaction details with the server, this includes
                                    1. Are there enough coins in the sender’s account to allow this transaction?
                                    2. Is the amount valid (greater than 0)?
                                    3. Are all numbers involved with this transaction not negative?
                             */

                            // #1: Validate transaction details

                            if (t.walletIDto >= 0 && t.walletIDfrom >= 0 // 3. Are all numbers involved with this transaction not negative?
                                && t.amount > 0) // 2. Is the amount valid (greater than 0)?
                            {
                                RestRequest balReq = new RestRequest("api/Blockchain/GetBalance/" + t.walletIDfrom.ToString());
                                IRestResponse balResp = cl.Get(balReq);
                                float walletBal = JsonConvert.DeserializeObject<float>(balResp.Content);

                                // 1. Are there enough coins in the sender’s account to allow this transaction?
                                if (walletBal >= t.amount)
                                {
                                    Debug.WriteLine("Adding block to chain list");

                                    // #2. Insert the transaction details into a block.
                                    Block newBlock = new Block();

                                    // 3. Pull down the last block from the current blockchain, and insert the hash of that block into the new block
                                    string resp = cl.Get(new RestRequest("api/Blockchain/GetLast")).Content;
                                    Block lastBlock = JsonConvert.DeserializeObject<Block>(resp);
                                    /*
                                    public uint blockID; // Uniquely identifies the Block
                                    public uint walletIDfrom; // Identifies source of transaction
                                    public uint walletIDto; // Identifies destination of transaction
                                    public float amount; // Amount of money being transacted.
                                    public uint offset; // Ensures validity of Hash (multiple of 5).
                                    public string prevBlockHash;
                                    public string blockHash;
                                     */
                                    newBlock.blockID = 1 + lastBlock.blockID; 
                                    newBlock.walletIDfrom = t.walletIDfrom;
                                    newBlock.walletIDto = t.walletIDto;
                                    newBlock.amount = t.amount;
                                    newBlock.offset = 0;
                                    newBlock.prevBlockHash = lastBlock.blockHash;
                                    newBlock.blockHash = "";
                                    // 4. Brute force a valid hash (one that starts with 12345) + 5. Insert the now valid hash and hash offset into the block
                                    newBlock = GenHashCode(newBlock);

                                    // Submit the block to the Bank Server for inclusion into the blockchain
                                    RestRequest addReq = new RestRequest("api/Blockchain/submitBlock");
                                    addReq.AddJsonBody(newBlock);
                                    cl.Post(addReq);

                                    t.processed = true;
                                }
                            }
                        }
                    }// End if (transactions.Count > 0)
                    else
                    {
                        Debug.WriteLine("Empty transaction queue");
                    }
                }
                catch (InvalidOperationException ioEx)
                {
                    Debug.WriteLine("Empty transaction queue");
                    Debug.WriteLine("Error: " + ioEx.ToString());
                }
                catch (NullReferenceException nEx) { Debug.WriteLine("Error: " + nEx.ToString()); }
                catch (Exception ex) { Debug.WriteLine("Error: " + ex.ToString()); }

                Thread.Sleep(5000);
            } // End while
        } // End method.

        public static Block GenHashCode(Block inB)
        {
            Block export = inB;
            bool valid = false;
            SHA256 sha256 = SHA256.Create();

            while (!valid)
            {
                //while (String.IsNullOrEmpty(export.blockHash) || !export.blockHash.StartsWith("12345"))
                while (export.blockHash == "" || !export.blockHash.StartsWith("12345") || export.offset % 5 != 0)
                {   
                    export.offset++;
                    // Getting the details from the Block in a toString format
                    string blockToHash = export.toHash();

                    // Convert to Base 64
                    byte[] textBytes = Encoding.UTF8.GetBytes(blockToHash);
                    // Use cryptographic hashing / SHA256 to generate Hash
                    byte[] hashed = sha256.ComputeHash(textBytes);

                    string hash = BitConverter.ToUInt64(hashed, 0).ToString();

                    export.blockHash = hash;
                }
                valid = true;
            } // End while
            return export;
        }

    } // End class
} // End namespace