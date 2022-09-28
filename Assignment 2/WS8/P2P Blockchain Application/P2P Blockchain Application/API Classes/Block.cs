using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Classes
{
    public class Block
    {
        public uint blockID { get; set; } // Uniquely identifies the Block

        public uint walletIDfrom { get; set; } // Identifies source of transaction


        public uint walletIDto { get; set; } // Identifies destination of transaction

        public float amount { get; set; } // Amount of money being transacted.

        public uint offset { get; set; } // Ensures validity of Hash (multiple of 5).

        public string prevBlockHash { get; set; }

        public string blockHash { get; set; }

        public string toHash()
        {
            return blockID.ToString() + walletIDfrom.ToString() + walletIDto.ToString() + amount.ToString() + offset.ToString() + prevBlockHash.ToString();
        }
    }
}
