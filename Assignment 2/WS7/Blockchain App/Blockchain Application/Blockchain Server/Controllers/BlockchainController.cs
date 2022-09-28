using API_Classes;
using Blockchain_Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Blockchain_Server.Controllers
{
    public class BlockchainController : ApiController
    {
        // Need to implement a Web API that:

        // 1. Allows the transaction generator (or users in general) to get the current state of the blockchain
        [Route("api/Blockchain/GetCurrentState")]
        [HttpGet]
        public int GetCurrentState() { return Blockchain.Size(); }

        [Route("api/Blockchain/GetCurrentChain")]
        [HttpGet]
        public List<Block> GetCurrentChain() { return Blockchain.getChain(); }


        [Route("api/Blockchain/GetChainList")]
        [HttpGet]
        public Class1 GetChainList() 
        {
            //return Blockchain.getChain();
            Class1 export = new Class1(Blockchain.getChain()); // Get current list of clients
            return export;
        }


        [Route("api/Blockchain/GetLast")]
        [HttpGet]
        public Block GetLast()
        {
            return Blockchain.findLast();
        }

        // 2. Allows the transaction generator (or users in general) to get the current coin balances of given users.
        // The server will need to go through the blockchain to determine what balance any given user has.

        // Balance = Float.
        [Route("api/Blockchain/GetBalance/{walletID}")] // For URL
        [Route("api/Blockchain/GetBalance")] // For Postman
        [HttpGet]
        public float GetBalance(uint walletID) { return Blockchain.getBalance(walletID); }


        // 3. Allows the miner to submit a new block for the chain.
        [Route("api/Blockchain/submitBlock")]
        [HttpPost]
        public void submitBlock(Block inBlock) { Blockchain.addBlock(inBlock); }
    }
}