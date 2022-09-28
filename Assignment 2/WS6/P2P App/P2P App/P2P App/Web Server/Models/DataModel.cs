using API_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_Server.Models
{
    public class DataModel
    {
        private static DataModel instance;
        private List<Client> clients;
        public DataModel()
        {
            clients = new List<Client>(); // Same as Prac 3 Singleton.
        }
        public static DataModel get() // Same as Prac 3 Singleton get() method
        {
            if (instance == null)
            {
                instance = new DataModel();
            }
            return instance;
        }
        // Singleton is now setup. We have a static list of clients, each client containing an IP Address and a Port.

        public List<Client> getClientsList()
        {
            // Return current copy of List ---> For Get method in ClientController.cs
            return new List<Client>(clients);
        }


        public void addClient(string inIPAddress, uint inPortNum) {
            Client newC = new Client(inIPAddress, inPortNum);
            if(!clients.Contains(newC))
            {
                clients.Add(newC);
            }
            // NOTE: Only adds if not already existing within the List.
        }

        public void removeClient(string inIPAddress, uint inPortNum) {
            //Client newC = new Client(inIPAddress, inPortNum);
            try
            {
                for (int ii = 0; ii < clients.Count; ii++)
                {
                    if (clients.ElementAt(ii).portNum == inPortNum)
                    {
                        clients.RemoveAt(ii);
                        break;
                    }
                }
            }
            catch(IndexOutOfRangeException){ } // No Clients found w/ specified Port Num
        }
    }
}