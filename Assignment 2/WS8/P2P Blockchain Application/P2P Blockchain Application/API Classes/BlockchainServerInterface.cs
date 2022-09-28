using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace API_Classes
{
    [ServiceContract]
    public interface BlockchainServerInterface
    {
        //Each of these are service function contracts. They need to be tagged as OperationContracts.
        [OperationContract]
        List<Block> GetCurrentBlockchain(); // – This will return the current block chain object in its entirety

        [OperationContract]
        Block GetCurrentBlock(); // This returns the latest block in the block chain

        [OperationContract]
        void ReceiveNewTransaction(Transaction t); // This lets other clients broadcast new transactions to your client.
    }
}
