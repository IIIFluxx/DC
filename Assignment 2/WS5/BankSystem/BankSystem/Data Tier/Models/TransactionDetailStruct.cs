using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Data_Tier.Models
{
    public class TransactionDetailStruct
    {
        public uint transactionID;
        public uint senderID;
        public uint recipientID;
        public uint amount;
    }
}