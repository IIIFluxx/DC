using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Client_Server.Models
{
    public class ClientList
    {
        public static List<Client> list = new List<Client>();


        public static void AddClient(Client c)
        {

            if (!list.Contains(c)) //If client doesn't already exist 
            {
                list.Add(c);
            }
            // NOTE: Only adds if not already existing within the List.
        }


        public static void removeClient(Client c)
        {
            //list.Remove(c);
            try
            {
                for (int i = 0; i < list.Count; i++)
                {
                    // loop through clients, find port and remove
                    if (list.ElementAt(i).IPAddress == c.IPAddress && list.ElementAt(i).portNum == c.portNum)
                    {
                        list.RemoveAt(i);
                        break;
                    }
                }
            }
            catch (ArgumentOutOfRangeException)
            {
            }
        }
    }
}