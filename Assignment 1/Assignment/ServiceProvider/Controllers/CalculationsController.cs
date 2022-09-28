using API_Classes;
using Authenticator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Web.Http;


namespace ServiceProvider.Controllers
{
    //TODO: Make class constructor make an Auth Server.
    // [Route("api/<keyword>")] 
    // [HttpPost/Put/Get/Delete]
    public class CalculationsController : ApiController
    {
        private AuthServerInterface foob; // classfield
        public CalculationsController()
        {
            ChannelFactory<AuthServerInterface> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://localhost/AuthenticationService";
            foobFactory = new ChannelFactory<AuthServerInterface>(tcp, URL);
            foob = foobFactory.CreateChannel(); // remote connection
        }


        // ADDTwoNumbers: This rest service adds two input integers and returns the output in JSON

        //[Route("api/Calculations/ADDTwoNumbers/{num1}/{num2}/{token}")] // Instead of specifying in the URL, we do below:
        [Route("api/Calculations/ADDTwoNumbers/")] // ONLY took it out BECAUSE easier for Postman.
        [HttpPost] // If I want POST, I think I need to pass thru JSON Body like I did in Registry. -- EDIT: apparently not, but I
                    // probably need to send in an object either way instead of 3-4-5 integers each time ~ cleaner. Do later down the line.
        public DataIntermed ADDTwoNumbers(int num1, int num2, int token)
        {
            String valid = "";
            valid = foob.Validate(token);
            //valid = foob.Validate(640355021);
            DataIntermed export = new DataIntermed(); // Like a "box" that can easily be serialized and sent elsewhere
            if (valid.Equals("validated"))
            {
                export.status = "Authenticated";
                export.reason = "Performing add operation to 2 numbers";
                export.solution = num1 + num2;
            }
            else
            {
                export.status = "Denied";
                export.reason = "Authentication Error";
            }
            return export;
        }

        // ADDThreeNumbers: This rest service adds three input integers and returns the output in JSON
        //[Route("api/Calculations/ADDThreeNumbers/{num1}/{num2}/{num3}/{token}")]
        [Route("api/Calculations/ADDThreeNumbers/")]
        [HttpPost]
        public DataIntermed ADDThreeNumbers(int num1, int num2, int num3,int token)
        {
            String valid = "";
            valid = foob.Validate(token);
            //valid = foob.Validate(640355021);
            DataIntermed export = new DataIntermed(); // Like a "box" that can easily be serialized and sent elsewhere
            if (valid.Equals("validated"))
            {
                export.status = "Authenticated";
                export.reason = "Performing add operation to 3 numbers";
                export.solution = num1 + num2 + num3;
            }
            else
            {
                export.status = "Denied";
                export.reason = "Authentication Error";
            }
            return export;
        }

        // MulTwoNumbers: This rest service multiplies two input integers and returns the output in JSON
        //[Route("api/Calculations/MulTwoNumbers/{num1}/{num2}/{token}")]
        [Route("api/Calculations/MulTwoNumbers/")]
        [HttpPost]
        public DataIntermed MulTwoNumbers(int num1, int num2, int token)
        {
            String valid = "";
            valid = foob.Validate(token);
            //valid = foob.Validate(640355021);
            DataIntermed export = new DataIntermed(); // Like a "box" that can easily be serialized and sent elsewhere
            if (valid.Equals("validated"))
            {
                export.status = "Authenticated";
                export.reason = "Performing multiplication operation to 2 numbers";
                export.solution = num1 * num2;
            }
            else
            {
                export.status = "Denied";
                export.reason = "Authentication Error";
            }
            return export;
        }

        // MulThreeNumbers: This rest service multiplies three input integers and returns the output in JSON
        //[Route("api/Calculations/MulThreeNumbers/{num1}/{num2}/{num3}/{token}")]
        [Route("api/Calculations/MulThreeNumbers/")]
        [HttpPost]
        public DataIntermed MulThreeNumbers(int num1, int num2, int num3, int token)
        {
            String valid = "";
            valid = foob.Validate(token);
            //valid = foob.Validate(640355021);
            DataIntermed export = new DataIntermed(); // Like a "box" that can easily be serialized and sent elsewhere
            if (valid.Equals("validated"))
            {
                export.status = "Authenticated";
                export.reason = "Performing multiplication operation to 2 numbers";
                export.solution = num1 * num2 * num3;
            }
            else
            {
                export.status = "Denied";
                export.reason = "Authentication Error";
            }
            return export;
        }

        // ================= Prime methods ===================
        public Boolean isPrimeNum(int inNum)
        {
            for (int ii=2;ii<inNum;ii++)
            {
                if (inNum % ii == 0)
                {
                    return false; // I.E. if not divisible by prime
                }
            }
            return true; // if no number can be divided by inNum, then is Prime
        }

        //[Route("api/Calculations/GeneratePrimeNumberstoValue/{num1}/{token}")]
        [Route("api/Calculations/GeneratePrimeNumberstoValue/")]
        [HttpPost]
        public DataIntermed GeneratePrimeNumberstoValue(int num1, int token)
        {
            String valid = "";
            valid = foob.Validate(token);
            //valid = foob.Validate(640355021);
            DataIntermed export = new DataIntermed(); // Like a "box" that can easily be serialized and sent elsewhere
            if (valid.Equals("validated"))
            {
                export.status = "Authenticated";
                export.reason = "Generating prime numbers to provided value";
                // ========================== Perform Calc =========================
                for (int ii = 2; ii <= num1; ii++)
                {
                    if (isPrimeNum(ii))
                    {
                        export.primeNums.Add(ii);
                    }
                }
                // =================================================================
                //export.solution = 5; // TODO ---------------
            }
            else
            {
                export.status = "Denied";
                export.reason = "Authentication Error";
            }
            return export;
        }

        //[Route("api/Calculations/GeneratePrimeNumbersinRange/{num1}/{num2}/{token}")]
        [Route("api/Calculations/GeneratePrimeNumbersinRange/")]
        [HttpPost]
        public DataIntermed GeneratePrimeNumbersinRange(int num1, int num2, int token)
        {
            String valid = "";
            valid = foob.Validate(token);
            //valid = foob.Validate(640355021);
            DataIntermed export = new DataIntermed(); // Like a "box" that can easily be serialized and sent elsewhere
            if (valid.Equals("validated"))
            {
                export.status = "Authenticated";
                export.reason = "Generating prime numbers within the provided range";
                // ========================== Perform Calc =========================
                
                //loop for finding and printing all prime numbers between given range
                for (int ii = num1; ii <= num2; ii++)
                {
                    if (isPrimeNum(ii))
                    {
                        export.primeNums.Add(ii);
                    }
                }

                // =================================================================
                //export.solution = 5; // TODO ---------------
            }
            else
            {
                export.status = "Denied";
                export.reason = "Authentication Error";
            }
            return export;
        }

        //[Route("api/Calculations/IsPrimeNumber/{num1}/{token}")]
        [Route("api/Calculations/IsPrimeNumber/")]
        [HttpPost]
        public DataIntermed IsPrimeNumber(int num1, int token)
        {
            String valid = "";
            valid = foob.Validate(token);
            //valid = foob.Validate(640355021);
            DataIntermed export = new DataIntermed(); // Like a "box" that can easily be serialized and sent elsewhere
            if (valid.Equals("validated"))
            {
                export.status = "Authenticated";
                export.reason = "Checking whether a provided number is Prime";
                // ========================== Perform Calc =========================
                if(isPrimeNum(num1))
                {
                    export.solution = 1; // 1 for true
                    // Do what?
                }
                else
                {
                    export.solution = 0; // 0 for false.
                }
                // =================================================================
            }
            else
            {
                export.status = "Denied";
                export.reason = "Authentication Error";
            }
            return export;
        }


        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        /*public void Post([FromBody] string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }*/
    }
}