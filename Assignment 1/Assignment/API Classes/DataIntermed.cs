using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Classes
{
    public class DataIntermed // USED ONLY TO RETURN THE ANSWER FROM OUR SERVICES.
    {
        // JSON Object passed back after a Service is called.
        public string status; // Whether you are Auth'd or not.
        public string reason; 
        public int solution; // Used for Addition & Multiplication of 2-3 Numbers.
        public List<int> primeNums = new List<int>(); // Used for Prime Calculations
        // Could just delete solution and always add to the List regardless ~ judge if iterating List is feasible every time or not.
    }
}
