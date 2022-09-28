using API_Classes;
using Authenticator;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Web.Http;

namespace Registry.Controllers
{
    public class RegistryController : ApiController
    {
        private AuthServerInterface foob; // classfield
        public RegistryController()
        {
            ChannelFactory<AuthServerInterface> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://localhost/AuthenticationService";
            foobFactory = new ChannelFactory<AuthServerInterface>(tcp, URL);
            foob = foobFactory.CreateChannel(); // remote connection
        }

        //[Route("api/Registry/Publish/{token}")]
        [Route("api/Registry/Publish/")]
        [HttpPost]
        public RegPubUnpubResult Publish(RegistryInputData request)
        {
            RegPubUnpubResult export = new RegPubUnpubResult();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            String valid = "";
            //Console.WriteLine("Token =  " + token);
            valid = foob.Validate(request.token);
            //Console.WriteLine("Valid = " + valid);
            //System.Diagnostics.Debug.WriteLine("Name from obj = " + request.name);

            // ==================================
            if (valid.Equals("validated"))
            {
                export.status = "Authenticated";
                export.reason = "Publishing service to our list of Services";
                Console.WriteLine("Authenticated");
                //export.solution = num1 + num2;
                try
                {
                    using (StreamWriter outputFile = new StreamWriter(Path.Combine(path, "services.txt"), true))
                    {
                        outputFile.WriteLine("Name: " + request.name);
                        outputFile.WriteLine("Description: " + request.description);
                        outputFile.WriteLine("API Endpoint: " + request.APIEndpoint);
                        outputFile.WriteLine("Number of operands: " + request.numOperands);
                        outputFile.WriteLine("Operand type: " + request.operandType);
                        /*JsonSerializer serializer = new JsonSerializer();
                        serializer.Formatting = Formatting.Indented;
                        serializer.Serialize(outputFile, request);*/

                    }


                    //export.status = "";
                }
                catch (IOException e)
                {
                    //strExp = "Registration failed";
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                export.status = "Denied";
                export.reason = "Authentication Error";
            }
            // =================================
            return export;
        }


        //[Route("api/Registry/Search/{token}")]
        [Route("api/Registry/Search/")]
        [HttpPost]
        public serviceList Search(SearchData search)
        {
            serviceList export = new serviceList(); // Contains Token, Status, Reason, List<RegistryInputData>
            String valid = "";
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //string[] lines = File.ReadAllLines(path + "/services.txt");
            RegistryInputData newObj = new RegistryInputData();
            StreamReader sr = new StreamReader(Path.Combine(path + "/services.txt"));
            String line = sr.ReadLine();
            // Perform validation first
            valid = foob.Validate(search.token);
            // ===
            if (valid.Equals("validated"))
            {
                export.status = "Authenticated";
                export.reason = "Acquiring a List of all published services";
                // Logic
                // ===

                // Look through all Services.
                while(line != null)
                {
                    // Assumption: Search result is only relevant to name i.e. name has to match service name; is the only search parameter.
                    if (line.Contains("Name: ") && line.ToUpper().Contains(search.searchStr.ToUpper())) // Convert case for search str & line.
                    { // If we find "Name: dddd<add>dddd
                        newObj = new RegistryInputData();
                        newObj.name = line.Replace("Name: ", ""); // TODO: Split line // obj.name = parts[1];
                        line = sr.ReadLine();
                        //System.Diagnostics.Debug.WriteLine("Name = " + newObj.name); // Name = BarryService
                        newObj.description = line.Replace("Description: ", "");
                        line = sr.ReadLine();
                        newObj.APIEndpoint = line.Replace("API Endpoint: ", "");
                        line = sr.ReadLine();
                        newObj.numOperands = Int16.Parse(line.Replace("Number of operands: ", ""));
                        line = sr.ReadLine();
                        newObj.operandType = line.Replace("Operand type: ", "");
                        export.services.Add(newObj);
                        line = sr.ReadLine();
                        //newObj = null;
                    }
                    else
                    {
                        line = sr.ReadLine();
                    }
                }
            }
            else
            {
                export.status = "Denied";
                export.reason = "Authentication Error";
            }
            return export;
        }



        //[Route("api/Registry/AllServices/{token}")] 
        [Route("api/Registry/AllServices/")]
        [HttpPost]
        public serviceList AllServices(int token)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string[] lines = File.ReadAllLines(path + "/services.txt");
            //RegistryInputData[] export = new RegistryInputData[lines.Length];
            //List<RegistryInputData> export = new List<RegistryInputData>();
            serviceList export = new serviceList(); // Contains Token, Status, Reason, List<RegistryInputData>
            RegistryInputData newObj = new RegistryInputData();
            String valid = "";
            // Perform validation first
            valid = foob.Validate(token);
            // ===
            if (valid.Equals("validated"))
            {
                export.status = "Authenticated";
                export.reason = "Acquiring a List of all published services";
                // Logic
                // ===
                foreach (string line in lines)
                {
                    //Console.WriteLine("Line = " + line);
                    if (line.Contains("Name: "))
                    {
                        newObj = new RegistryInputData();
                        newObj.name = line.Replace("Name: ", "");
                        System.Diagnostics.Debug.WriteLine("Name = " + newObj.name); // Name = BarryService
                    }

                    else if (line.Contains("Description: "))
                    {
                        newObj.description = line.Replace("Description: ", "");
                        System.Diagnostics.Debug.WriteLine("Desc = " + newObj.description);
                    }

                    else if (line.Contains("API Endpoint: "))
                    {
                        newObj.APIEndpoint = line.Replace("API Endpoint: ", "");
                    }

                    else if (line.Contains("Number of operands: "))
                    {
                        newObj.numOperands = Int16.Parse(line.Replace("Number of operands: ", ""));
                    }

                    else if (line.Contains("Operand type: "))
                    {
                        newObj.operandType = line.Replace("Operand type: ", "");
                        export.services.Add(newObj);
                        //export[ii] = newObj;
                        newObj = null;
                        //ii++;
                    }
                }
            }    
            else
            {
                export.status = "Denied";
                export.reason = "Authentication Error";
            }            
            return export;
        }

        //[Route("api/Registry/Unpublish/{token}")] 
        [Route("api/Registry/Unpublish/")]
        [HttpPost]
        public RegPubUnpubResult Unpublish(RegistryInputData request)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string[] lines = File.ReadAllLines(path + "/services.txt");
            //System.Diagnostics.Debug.WriteLine("Lines # = " + lines.Length);
            //System.Diagnostics.Debug.WriteLine("Lines # = " + lines[0]);  // == Name: BarryService --- works fine. lines[1] would be Desc: NM.
            //StreamReader sr = new StreamReader(Path.Combine(path + "/services.txt"));
            String valid = "";

            valid = foob.Validate(request.token); 
            RegPubUnpubResult export = new RegPubUnpubResult(); // Like a "box" that can easily be serialized and sent elsewhere
            if (valid.Equals("validated"))
            {
                export.status = "Authenticated";
                export.reason = "Unpublishing a Service";
                File.WriteAllText(path + "/services.txt", String.Empty); // Make file empty.
                for (int i = 0; i < lines.Length; i++)
                {
                    //System.Diagnostics.Debug.WriteLine("API Endpoint: " + request.APIEndpoint);
                    //System.Diagnostics.Debug.WriteLine("Lines[i] = " + lines[i]);
                    if (lines[i].Equals("API Endpoint: " + request.APIEndpoint))
                    {
                        //System.Diagnostics.Debug.WriteLine("Lines[i] = " + lines[i]);
                        lines[i - 2] = ""; // Name
                        lines[i - 1] = ""; // Desc
                        lines[i] = ""; // Endpoint
                        lines[i+1] = ""; // Num Operands
                        lines[i+2] = ""; // Operand type.
                    }
                }
                // Goal: Only write the lines that are not ""
                foreach (string line in lines)
                {
                    if (!line.Equals(""))
                    {
                        using (StreamWriter outputFile = new StreamWriter(Path.Combine(path, "services.txt"), true))
                        {
                            outputFile.WriteLine(line);
                        }
                    }
                }                
            }
            else
            {
                export.status = "Denied";
                export.reason = "Authentication Error";
            }
            return export;
        }



        /*        // POST api/<controller>
                public void Post([FromBody] string value)
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