using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Classes
{
    public class RegistryInputData  // Used whenever we need to insert to/delete from/query Registry files.
    {
        // JSON Object input into Registry Controller's methods
        public int token;
        public string name;
        public string description;
        public string APIEndpoint;
        public int numOperands;
        public string operandType;
    }
}
