using API_Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Web_Server.Models;

namespace Web_Server.Controllers
{
    public class ClientController : ApiController
    {
        // You will need to build a web service that can accept two kinds of request. 
        // 1. Register themselves -- POST method (ip address + port they host their .NET Remoting Service on)
        // 2. Request Client List -- GET method (to connect to other clients' IP/Port to ask for jobs).

        [Route("api/Client/")]
        [HttpGet]
        public ClientList GetClients() 
        {
            Debug.WriteLine("/Client/: Attempting to get clients list");
            DataModel dm = DataModel.get(); // Retrieve singleton class.
            ClientList export = new ClientList(dm.getClientsList()); // Get current list of clients
            return export;
        }

        [Route("api/Client/GetList")]
        [HttpGet]
        public List<Client> GetList()
        {
            Debug.WriteLine("/Client/: Attempting to retrieve list of clients");
            DataModel dm = DataModel.get(); // Retrieve singleton class.
            ClientList export = new ClientList(dm.getClientsList()); // Get current list of clients
            return export.clients;
        }

        [Route("api/Client/GetLeaderboard")]
        [HttpGet]
        public List<Client> GetLeaderboard()
        {
            DataModel dm = DataModel.get(); // Retrieve singleton class.
            ClientList cl = new ClientList(dm.getClientsList()); // Get current list of clients
            List<Client> export = cl.clients;
            export.OrderBy(x => x.jobsDone).ToList();
            return export;
        }


        [Route("api/Client/registerClient")]
        [HttpPost]
        public void registerClient(Client inClient)
        {
            Debug.WriteLine("/Client/: Attempting to register client: " + inClient.IPAddress + ":" +inClient.portNum);
            DataModel dm = DataModel.get(); // Retrieve singleton class.
            dm.addClient(inClient.IPAddress, inClient.portNum); // Add client to singleton 'database' client list
        }

        [Route("api/Client/removeClient")]
        [HttpPost]
        public void removeClient(Client inClient)
        {
            Debug.WriteLine("/Client/: Attempting to remove client: " + inClient.IPAddress + ":" + inClient.portNum);
            DataModel dm = DataModel.get(); // Retrieve singleton class.
            dm.removeClient(inClient.IPAddress, inClient.portNum); // Add client to singleton 'database' client list
        }

        [Route("api/Client/updateClient/{index}")]
        [Route("api/Client/updateClient/")]
        [HttpPost]
        public int updateClient(int index)
        {
            DataModel dm = DataModel.get(); // Retrieve singleton class.
            dm.getClientsList().ElementAt(index).jobsDone++;
            return dm.getClientsList().ElementAt(index).jobsDone;
        }

    }
}