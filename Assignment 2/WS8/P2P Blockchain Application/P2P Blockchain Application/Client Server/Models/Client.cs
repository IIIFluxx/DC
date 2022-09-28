using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Client_Server.Models
{
    public class Client
    {
        public string IPAddress;
        public uint portNum;
        public int jobsDone;

        public Client(string inIP, uint inPort)
        {
            IPAddress = inIP;
            portNum = inPort;
            jobsDone = 0;
        }
    }
}