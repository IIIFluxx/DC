using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace API_Classes
{
    public class Blockchain
    {
        private static List<Block> blockchain = new List<Block>();
        static Blockchain() // Runs first time a block is made (only first time - has unlimited funds).
        {
            Block block = new Block();
            block.blockID = 0;
            block.walletIDfrom = 0;
            block.walletIDto = 0;
            block.amount = float.MaxValue;
            block.offset = 0;
            block.prevBlockHash = "";
            block.blockHash = "";
            block = GenHashCode(block);
            blockchain.Add(block);
        }

        public static Block GenHashCode(Block inB)
        {
            Block export = inB;
            bool valid = false;
            SHA256 sha256 = SHA256.Create();

            while (!valid)
            {

                try
                {
                    //while (String.IsNullOrEmpty(export.blockHash) && !export.blockHash.StartsWith("12345") && export.offset % 5 != 0)
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
                    } // End while
                    valid = true;
                }
                catch (ArgumentOutOfRangeException)
                {
                    Debug.WriteLine("String value exceeded - Arg out of range.");
                }
            } // End while
            return export;
        } // End method

        // Web API should call the following methods to satisfy the following functions:
        // 1. Allows the transaction generator (or users in general) to get the current state of the blockchain

        public static int Size() { return blockchain.Count(); }

        public static List<Block> getChain() { return blockchain; }

        public static Block findLast() { return blockchain.Last(); }

        // 2. Allows the transaction generator (or users in general) to get the current coin balances of given users.
        // The server will need to go through the blockchain to determine what balance any given user has.
        public static float getBalance(uint walletID)
        {
            float export = 0;

            if (walletID == 0) // i.e. if account for the bank itself.
            {
                export = float.MaxValue;
            }
            else
            {
                foreach (Block b in blockchain)
                {
                    if (walletID == b.walletIDto) // Found our user (sender)
                    {
                        export += b.amount;
                    }
                    else if (walletID == b.walletIDfrom) // Found our user (sender)
                    {
                        export -= b.amount;
                    }
                    // End if walletID == ...
                } // End for
            } // End if walletID 0
            return export;
        }


        // 3. Allows the miner to submit a new block for the chain.
        public static void addBlock(Block inBlock)
        {
            /*
              So the server needs to make these checks:
                ◦ The block ID must be a number higher than all other block IDs ✔️
                ◦ The from wallet ID must have at least as many coins as the transaction amount ✔️
                ◦ The amount must be greater than 0 ✔️
                ◦ The block offset must be divisible by 5 ✔️
                ◦ The previous block hash must match the last block in the current chain ✔️
                ◦ The block hash must start with 12345 ✔️
                ◦ The block hash must be valid, ie if you perform the hash process (see the miner part) the
                hash you get is equal to the one that the block has! ✔️
                ◦ No number can be negative! ✔️
             */



            if (inBlock.blockID > FindLargest() // If > largest block's block ID.

                && getBalance(inBlock.walletIDfrom) >= inBlock.amount // If sufficient number of coins in account

                && inBlock.amount > 0 // Amount > 0

                && inBlock.offset % 5 == 0 // Amt divisible by 5
                && inBlock.prevBlockHash.SequenceEqual(findLast().blockHash) // prevBlockHash = list's last block's hash

                //&& inBlock.prevBlockHash == findLast().blockHash // prevBlockHash = list's last block's hash

                && inBlock.blockHash.StartsWith("12345")
                && inBlock.blockHash == validateHash(inBlock)
                // No number can be -ve

                && inBlock.walletIDfrom >= 0

                && inBlock.walletIDto >= 0

                && inBlock.offset > 0

                ) { blockchain.Add(inBlock); }
            else
            {
                Debug.WriteLine("Error: Invalid block");
            }
        }

        public static uint FindLargest()
        {
            /*uint largest = blockchain.Max(r => r.blockID);*/
            uint largest = 0;

            foreach (Block cur in blockchain)
            {
                if (cur.blockID > largest)
                {
                    largest = cur.blockID;
                }
            }
            Debug.WriteLine("Blockchain: Largest block ID found.");
            return largest;
        }

        public static string validateHash(Block inBlock)
        {
            SHA256 sha256 = SHA256.Create();

            if (!String.IsNullOrEmpty(inBlock.toHash()))
            {
                byte[] textBytes = Encoding.UTF8.GetBytes(inBlock.toHash());
                byte[] hashed = sha256.ComputeHash(textBytes);
                string hash = BitConverter.ToUInt64(hashed, 0).ToString();
                return hash;
            }
            else
            {
                Debug.WriteLine("Block hash is null or empty. Please correct.");
                return "";
            }
        }


        public static void setChain(List<Block> inList)
        {
            blockchain = inList;
        }

    }
}
