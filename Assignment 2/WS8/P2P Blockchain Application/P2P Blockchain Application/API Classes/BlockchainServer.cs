using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace API_Classes
{
    //[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, InstanceContextMode = InstanceContextMode.Single)]
    public class BlockchainServer : BlockchainServerInterface
    {
        public Block GetCurrentBlock() // This returns the latest block in the block chain
        {
            return Blockchain.findLast();
        }

        public List<Block> GetCurrentBlockchain() // – Returns the current block chain object in its entirety
        {
            return Blockchain.getChain();
        }

        public void ReceiveNewTransaction(Transaction t) // // This lets other clients broadcast new transactions to your client.
        {
            Transactions.addTransaction(t);
        }
    }
}