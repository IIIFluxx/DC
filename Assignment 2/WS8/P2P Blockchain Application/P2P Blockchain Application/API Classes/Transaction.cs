using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Classes
{
    public class Transaction
    {
        public uint walletIDfrom; // Identifies source of transaction

        public uint walletIDto; // Identifies destination of transaction

        public float amount; // Amount of money being transacted.

        public bool processed;

        public string toString()
        {
            return walletIDfrom.ToString() + " to " + walletIDto.ToString() + ". Amount: " + amount.ToString();
        }

    }
}
