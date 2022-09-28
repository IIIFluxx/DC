using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace API_Classes
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

    public class ClientList
    {
        public List<Client> clients;
        public ClientList(List<Client> inList)
        {
            clients = inList;
        }
    }

    public class Job
    {
        public int jobID; // ID specific to a job.
        public string pythonCode; // Source code
        public byte[] hashCode; // Byte array of SHA256 Hashes
        public string solution; // Solution
        public bool req; // Requested or not.

        public Job()
        {
            req = false;
        }
		
        public void setHashCode(byte[] inHash)
        {
            this.hashCode = inHash;
        }

        public void setPythonSrc(string inCode)
        {
            this.pythonCode = inCode;
        }

        public void setJobNumber(int inID)
        {
            this.jobID = inID;
        }

        public void setSolution(string inSol)
        {
            this.solution = inSol;
        }
    }
}
