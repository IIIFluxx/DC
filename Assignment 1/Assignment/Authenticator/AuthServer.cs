using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Authenticator
{
    class ServerRun
    {
        private static AuthServerInterface foob;

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to my Authentication Server!");
            //This is the actual host service system
            ServiceHost host;
            //This represents a tcp/ip binding in the Windows network stack
            NetTcpBinding tcp = new NetTcpBinding();
            //Bind server to the implementation of AuthServer
            host = new ServiceHost(typeof(AuthServer));


            host.AddServiceEndpoint(typeof(AuthServerInterface), tcp,
            "net.tcp://localhost/AuthenticationService"); // This is a fixed service endpoint.

            //And open the host for Authentication!
            host.Open();
            Console.WriteLine("Authentication System is Online");

            Console.ReadLine();
            //Close host once done;
            host.Close();
        }
    }

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    internal class AuthServer : AuthServerInterface
    {
        // In Prac 1, we just made a Database and called its methods. Here we just implement the methods here itself.
        
        
        /*  It expects two operands, i.e., name and password from an actor. 
         *  It saves these values in a local text file. If successful it returns “successfully registered” */

            public string Register(string name, string password) // SPECIFY IF OUT OR NOT.
        {
            // Receives two strings from an actor.

            // Set a variable to the Documents path.
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //string path = System.AppDomain.CurrentDomain.BaseDirectory;
            //string path = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName, "AuthFiles/log.txt");
            string export = "Successfully Registered";
            try
            {
                using (StreamWriter outputFile = new StreamWriter(Path.Combine(path, "registered.txt"), true))
                {
                    outputFile.WriteLine(name + "," + password);
                }
            }
            catch (IOException)
            {
                export = "Registration failed";
            }
            Console.WriteLine(export);
            return export;
            // if save to file successful --> successfully registered.
        }


        /*  It expects two operands, i.e., name and password from an actor. 
         *  It checks these values in a local text file. If a match is found, it creates a token (random integer), 
         *  saves it into another local text file, and returns it to the actor who calls this function */

        public int Login(string name, string password) // SPECIFY IF OUT OR NOT.
        {
            // Read a text file line by line.  

            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //string path = System.AppDomain.CurrentDomain.BaseDirectory;
            System.Random random = new System.Random();
            int token = -1;
            string line;
            Boolean found = false;

            try
            {
                StreamReader streamReader = new StreamReader(Path.Combine(path, "registered.txt"));
                line = streamReader.ReadLine();
                while(line != null)
                {
                    string[] tokens = new string[2];
                    tokens = line.Split(',');
                    if(tokens[0].Equals(name))
                    {
                        if(tokens[1].Equals(password))
                        {
                            Console.WriteLine("Username: " + name);
                            Console.WriteLine("Password: " + password);
                            found = true;
                        }
                    }
                    line = streamReader.ReadLine();
                }
                Console.WriteLine("Found val = " + found);
                if (found)
                {
                    token = Math.Abs(random.Next());
                    try
                    {
                        using (StreamWriter outputFile = new StreamWriter(Path.Combine(path, "login.txt"), true))
                        {
                            outputFile.WriteLine(token.ToString());
                        }

                    }
                    catch (IOException)
                    {
                        token = -1;
                    }
                }
                streamReader.Close();
                //Console.ReadLine();
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception - " + e.Message);
            }

            return token;
        }


        /*
        * It expects a token and checks whether the token is already generated. 
        * If the token could be validated, the return is “validated”, else “not validated”*/

        public string Validate(int token)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //string path = System.AppDomain.CurrentDomain.BaseDirectory;
            string[] lines = File.ReadAllLines(path + "/login.txt");
            string export = "";
            foreach (string line in lines)
            {
                Console.WriteLine("Line = " + line);
                if (line.Equals(token.ToString()))
                {
                    export = "validated";
                    Console.WriteLine("Token inside = " + token);
                    break;
                }
                else
                {
                    export = "not validated";
                }
            }

            Console.WriteLine("Export = " + export);
            return export;
        }
    }
}