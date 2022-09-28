using Client_Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Client_Server.Controllers
{
    public class ClientController : ApiController
    {
        // You will need to build a web service that can accept two kinds of request. 
        // 1. Register themselves -- POST method (ip address + port they host their .NET Remoting Service on)
        // 2. Request Client List -- GET method (to connect to other clients' IP/Port to ask for jobs).

        [Route("api/Client/")]
        [Route("api/Client/GetList")]
        [HttpGet]
        public List<Client> getClientsList()
        {
            return ClientList.list;
        }


        [Route("api/Client/registerClient")]
        [HttpPost]
        public void registerClient(Client inClient)
        {
            ClientList.AddClient(inClient);
        }

        [Route("api/Client/removeClient")]
        [HttpPost]
        public void removeClient(Client inClient)
        {
            ClientList.removeClient(inClient);
        }

        [Route("api/Client/updateClient/{index}")]
        [Route("api/Client/updateClient/")]
        [HttpPost]
        public void updateClient(int index)
        {
            ClientList.list.ElementAt(index).jobsDone++;
        }
    }
}