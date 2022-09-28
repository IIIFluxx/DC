using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Classes
{
    public class serviceList
    {
        //public int token; // If we ever import this then use this, otherwie delete as unnecessary.
        public string status;
        public string reason;
        public List<RegistryInputData> services = new List<RegistryInputData>();
    }
}
