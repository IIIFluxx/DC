using API_Classes;
using Authenticator;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServicePublisher
{
    class Program
    {
        static void Main(string[] args)
        {
            ChannelFactory<AuthServerInterface> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://localhost/AuthenticationService";
            foobFactory = new ChannelFactory<AuthServerInterface>(tcp, URL);
            AuthServerInterface foob = foobFactory.CreateChannel(); // remote connection
            // Set URL
            string restURL = "https://localhost:44304/"; // Fixed endpoint for Registry Web Service.
            RestClient client;
            client = new RestClient(restURL);
            //RestRequest req;
            int userToken = -1;
            Boolean done = false;
            Boolean done2 = false;

            // While loop ------------
            while (!done)
            {
                spacer();
                Console.WriteLine("Would you like to:\n(1) - Register\n(2) - Login");
                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": // Register.
                    {
                        spacer();
                        Console.WriteLine("Please enter your details to Register");
                        Console.WriteLine("Enter your desired account username");
                        string username1 = Console.ReadLine();
                        Console.WriteLine("Enter your desired account password");
                        string password1 = Console.ReadLine();
                        Console.WriteLine("Attempting register...");
                        spacer();
                        string result1 = foob.Register(username1, password1);
                        Console.WriteLine("Status: " + result1);
                        Console.WriteLine("Press any key to continue.");
                        Console.ReadLine();
                        spacer();
                        break;
                    }

                    case "2": // Login.
                    {
                        spacer();
                        Console.WriteLine("Please enter your login details ");
                        Console.WriteLine("Enter your account username");
                        string username2 = Console.ReadLine();
                        Console.WriteLine("Enter your account password");
                        string password2 = Console.ReadLine();
                        Console.WriteLine("Attempting login...");
                        spacer();
                        userToken = foob.Login(username2, password2);
                        //Console.WriteLine("Status: " + userToken); // Debug
                        if (userToken != -1)
                        {
                            done = true;
                            Console.WriteLine("Login succeeded. Press any key to continue.");
                            Console.ReadLine();
                        }
                        else
                        {
                            Console.WriteLine("Login error. Press any key to try again.");
                            Console.ReadLine();
                        }

                        break;
                    }
                }
            } // End while

            // Publish/Unpublish
            RegistryInputData pub = new RegistryInputData();
            RegistryInputData unpub = new RegistryInputData();
            RegPubUnpubResult info = new RegPubUnpubResult();

            while (!done2)
            {
                spacer();
                Console.WriteLine("Would you like to: \n(1) - Publish\n(2) - Unpublish a service\n(3) - Exit");
                string pubunpub = Console.ReadLine();
                spacer();

                switch (pubunpub)
                {
                    case "1": // Publish
                        {
                            // Make a JSON Object, add token into it.
                            pub.token = userToken;
                            Console.WriteLine("Please enter the name of your Service: ");
                            pub.name = Console.ReadLine();

                            Console.WriteLine("Please enter your Service description: ");
                            pub.description = Console.ReadLine();

                            Console.WriteLine("Please enter the fixed API endpoint of your Service: ");
                            pub.APIEndpoint = Console.ReadLine();

                            Console.WriteLine("Please enter the number of operands your Service handles: ");
                            pub.numOperands = Int16.Parse(Console.ReadLine());

                            Console.WriteLine("Please enter the type of operands your Service handles: ");
                            pub.operandType = Console.ReadLine();

                            Console.WriteLine("Attempting Publish...");
                            spacer();
                            //Console.ReadLine();
                            // Send off pub object to the Publish method, retrieve result, and display to screen the result.

                            //Set URL request for search POST function, and add SearchData
                            RestRequest req = new RestRequest(String.Concat("api/Registry/Publish/")); // api/Registry/Publish
                            req.AddJsonBody(pub);

                            //Do the request
                            IRestResponse resp = client.Post(req);

                            // Retrieve the returning object and deserialize it
                            info = JsonConvert.DeserializeObject<RegPubUnpubResult>(resp.Content);
                            Console.WriteLine(info.status);
                            Console.WriteLine(info.reason);
                            Console.WriteLine("Press any key to continue.");
                            Console.ReadLine();
                            // ====
                            break;
                        }
                    case "2":
                        {
                            unpub.token = userToken;
                            Console.WriteLine("Please provide the API Endpoint for the Service you wish to unpublish: ");
                            string endpointUnPub = Console.ReadLine();
                            Console.WriteLine("Attempting Unpublish...");
                            spacer();

                            unpub.APIEndpoint = endpointUnPub;
                            // Send off unpub object to Unpublish method, retrieve result and display to screen the result.

                            //Set URL request for search POST function, and add SearchData
                            RestRequest req = new RestRequest(String.Concat("api/Registry/Unpublish/"));
                            req.AddJsonBody(unpub);

                            //Do the request
                            IRestResponse resp = client.Post(req);

                            // Retrieve the returning object and deserialize it
                            //RegPubUnpubResult info = new RegPubUnpubResult();
                            info = JsonConvert.DeserializeObject<RegPubUnpubResult>(resp.Content);
                            Console.WriteLine(info.status);
                            Console.WriteLine(info.reason);
                            Console.WriteLine("Press any key to continue.");
                            Console.ReadLine();
                            // =====
                            break;
                        }
                    case "3":
                        {
                            done2 = true;
                            break;
                        }
                }
            }
        } // End method

        public static void spacer()
        {
            Console.WriteLine("==========================");
        }
    } // End class
} // End namespace
