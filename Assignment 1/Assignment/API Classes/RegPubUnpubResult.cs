using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Classes
{
    public class RegPubUnpubResult // Used only when Publishing and Unpublishing -- we only need to return Auth status & reason.
    {
        public string status; // Whether you are Auth'd or not.
        public string reason;
    }
}
