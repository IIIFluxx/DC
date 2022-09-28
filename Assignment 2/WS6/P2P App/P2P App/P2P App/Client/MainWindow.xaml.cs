using API_Classes;
using IronPython.Hosting;
using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client_GUI
{
    /*
        Need to make 2 more threads 
            - one for Networking    |  which is going to connect to the server and other clients to find and do jobs
            - one for Server.       | which manages connections from other clients via .NET Remoting  (so just set up a server like Prac 1)
    */


    public partial class MainWindow : Window
    {
        private Thread serverThread, networkingThread;
        private uint currPort;
        private RestClient client;
        private ChannelFactory<JobServerInterface> foobFactory;
        private JobServerInterface foob;
        private int totJobs = 0;
        private int jobC = 0;
        private string URL;
        private bool isClosed = false;
        public bool winClosed;
		
        public MainWindow()
        {
            InitializeComponent();
            currPort = 8100;
            URL = "https://localhost:44376/";
            client = new RestClient(URL);
            // The GUI code is also going to have to start the other two threads on initialisation,
            // as THIS THREAD == main() for this program
            serverThread = new Thread(new ThreadStart(ServerFunction));
            serverThread.Start();

            networkingThread = new Thread(new ThreadStart(NetworkingFunction));
            networkingThread.Start();

            NetTcpBinding tcp = new NetTcpBinding();
            string conURL = "net.tcp://localhost:" + currPort.ToString() + "/ClientServer";
            foobFactory = new ChannelFactory<JobServerInterface>(tcp, conURL);
            foob = foobFactory.CreateChannel();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Debug.WriteLine("GUI: Closing window..... removing Client from Server");
            RestClient closeServer = new RestClient(URL);
            RestRequest request = new RestRequest("api/Client/removeClient");
            request.AddJsonBody(new Client("127.0.0.1", currPort));
            IRestResponse resp = closeServer.Post(request);

            
            //serverThread.Join();
            Debug.WriteLine("GUI: Client closed successfully.");
            isClosed = true;
			
            //networkingThread.Join();
        }

        private void submitBtn_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("GUI: Attempting to submit job");
            SHA256 sha256Hash = SHA256.Create();
            // Contents of codeTxt needs to be submitted to the Server thread.

            // Encode string (code) in Base 64: REF: WS Code.
            if(!String.IsNullOrEmpty(codeTxt.Text))
            {
                if (codeTxt.Text.StartsWith("def main():") && codeTxt.Text.Contains("return"))
                {
                    Debug.WriteLine("Code: ");
                    Debug.WriteLine(codeTxt.Text);
                    // Encode in Base 64.
                    byte[] textBytes = Encoding.UTF8.GetBytes(codeTxt.Text);
                    string encodedStr = Convert.ToBase64String(textBytes);

                    // Decoding finished; now use Cryptographic hashes to uniquely identify above string.
                    byte[] jobHash = Encoding.UTF8.GetBytes(encodedStr);
                    byte[] compHash = sha256Hash.ComputeHash(jobHash);

                    // Now populate Job object and submit.
                    // int jobID, string pythonCode, byte[] hashCode, string solution, bool req;
                    Job newJob = new Job();
                    JobServer server = JobServer.get();
                    newJob.setJobNumber(server.GetJobsList().Count + 1);
                    newJob.setPythonSrc(encodedStr);
                    newJob.setHashCode(compHash);
                    // Soln should be null
                    // Req should be false (done in constructor)
                    foob.SubmitJob(newJob);
                    resultsTxt.AppendText("Job submitted");
                    Debug.WriteLine("GUI: Job " + newJob.jobID + "has been submitted");
                }
                else
                {
                    MessageBox.Show("Code must start with 'def main(): ' & must return something");
                }
            }
        }


        public void NetworkingFunction() {
            RestClient queryWS = new RestClient(URL);
            List<Client> curList = null;
            ClientList updList = null;
            NetTcpBinding tcp = new NetTcpBinding();
            SHA256 sha256Hash = SHA256.Create();
            ScriptEngine engine = Python.CreateEngine();
            ScriptScope scope = engine.CreateScope();
            // In while loop ---> 1. Look for new clients & 2. Check each client for Jobs and do if can.

            while (!isClosed)
            {
                bool success = true;
                // ============ 1. Query the Web Service for a list of other clients. ============
                RestRequest request = new RestRequest("api/Client"); // Calls GetClient method.
                IRestResponse resp = queryWS.Get(request);
                if(resp.IsSuccessful == false)
                {// Error occured.
                    if(curList == null)
                    {
                        //isClosed = true;

                        Console.WriteLine("Error occurred: Could not retrieve client list from Web Server.");
                        Console.WriteLine("Check Web Server status.");
                    }
                }
                else if (resp.IsSuccessful) // If we got our current list.
                {
                    // Set our updated list from JSON object returned from GetClients method in ClientsController.cs
                    updList = JsonConvert.DeserializeObject<ClientList>(resp.Content);
                    // Operate on a separate List<Client>
                    curList = new List<Client>(updList.clients);

                    totJobs = 0;
                    for(int ii = 0; ii<curList.Count;ii++)
                    {
                        try
                        {

                            // ============ 2.Check each client for Jobs and do if can. ============

                            // Disallow curr client if server's owner.
                            if (curList.ElementAt(ii).portNum != currPort)
                            {
                                //Connect to that client's server
                                string conURL = "net.tcp://"
                                    + curList.ElementAt(ii).IPAddress.ToString()
                                    + ":"
                                    + curList.ElementAt(ii).portNum.ToString() + "/ClientServer";
                                // Connect to ClientServer
                                foobFactory = new ChannelFactory<JobServerInterface>(tcp, conURL);
                                foob = foobFactory.CreateChannel();

                                // It then can query if any jobs exist, and download them.
                                List<Job> jList = foob.GetJobsList();

                                Job currJob = jList.Find(j => j.solution == null);
                                //currJob = jList.Find(j => j.pythonCode != null); Should it be this?
                                //Debug.WriteLine("CurrJob's code = " + currJob.pythonCode);
                                //if (currJob.pythonCode != null)
                                if (currJob != null && currJob.solution == null)
                                {
                                    // If Job is found.
                                    // ===================== ALL ABOVE IS WORKING =======================

                                    // Download job from Hash - check if matching using Base64.

                                    /** REFERENCE: WS6 worksheet **/

                                    // Decode to a string.
                                    byte[] encodedBytes = Convert.FromBase64String(currJob.pythonCode);
                                    string pySnippet = Encoding.UTF8.GetString(encodedBytes);
                                    // Encoding finished; now use Cryptographic hashes to uniquely identify above string:

                                    byte[] jobHash = currJob.hashCode;
                                    byte[] compHash = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(currJob.pythonCode));

                                    // To verify, compare hash we make with hash within the Job - must be exactly the same

                                    if (compHash.SequenceEqual(jobHash)) // Only perform the job if the hashes are the same.
                                    {
                                        currJob.pythonCode = pySnippet;

                                        // Upon successfully downloading a job, the Networking thread needs to use Iron Python to execute
                                        // the code & post the answer back to the client that hosted the job.

                                        this.Dispatcher.Invoke(() =>
                                        {
                                            try
                                            {
                                                statusTxt.Text = "Computing a job";
                                                Debug.WriteLine("Conducting Job");
                                                // Iron Python Code: Executing codeTxt's contents as a Python script.
                                                // REFERENCE: WS6 Code.
                                                engine.Execute(pySnippet, scope);
                                                dynamic testFn = scope.GetVariable("main"); // Run the main method in script.
                                                                                            // Retrieve result from function
                                                var result = testFn();
                                                // Method finished running; update Views to represent this.
                                                string resultStr = result.ToString();
                                                currJob.solution = resultStr;
                                                resultsTxt.Text = resultStr;
                                                // =================================
                                                RestClient wsv = new RestClient(URL);
                                                RestRequest req = new RestRequest("api/Client/updateClient/" + ii.ToString());
                                                IRestResponse num = wsv.Post(req);
                                                jobC = JsonConvert.DeserializeObject<int>(num.Content);
                                                Debug.WriteLine("Completed Job " + jobC.ToString());
                                            }
                                            catch (NullReferenceException) { MessageBox.Show("Return from method/code is null. Please rectify python script"); }
                                            catch (UnboundLocalException) { MessageBox.Show("Reference made to hidden local variable. Please fix python script"); }
                                            catch (RuntimeException rex) { MessageBox.Show("Runtime error occured: " + rex.Message); }
                                            catch (UnboundNameException) { MessageBox.Show("Method/code is invalid. Please enter a valid python script"); }
                                            catch (SyntaxErrorException) { MessageBox.Show("Syntax error present in Method/code. Please enter a valid python script"); }
                                        });
                                        
                                        foob.SubmitSoln(currJob.pythonCode, currJob);
                                        // Update views reflecting completion
                                        this.Dispatcher.Invoke(() =>
                                        {
                                            statusTxt.Text = "Job completed";
                                            numJobsTxt.Text = jobC.ToString();
                                            Thread.Sleep(1000);
                                            // TODO: Log
                                        });
                                    } // End if hashes match
                                    else
                                    {
                                        success = false;
                                        Debug.WriteLine("Hashes don't match");
                                    }
                                } // End if (curJob != null)
                                else 
                                { 
                                    success = false;
                                    //Debug.WriteLine("Current job is null");
                                }
                                if (!success) //Output error to UI 
                                {
                                    //Debug.WriteLine("Job failed");
                                }


                            } // End if (curList.ElementAt(ii).portNum != currPort)

                        } // End try
                        catch (EndpointNotFoundException) { MessageBox.Show("Error: Endpoint not found. "); }
                        catch (Exception ex) { MessageBox.Show("Fatal error occured: " + ex); }
                    } // End for
                } // End if
            } // End while
        } // End method

        public void ServerFunction() // The client's job board
        {
            // codeTxt contents should be retrieved here.
            //This is the actual host service system
            ServiceHost host;

            // Create new instance of our Server
            // host = new ServiceHost(DataServer.get());
            JobServer server = JobServer.get();

            //Bind server to the implementation of JobServer
            host = new ServiceHost(server);

            //This represents a tcp/ip binding in the Windows network stack
            NetTcpBinding tcp = new NetTcpBinding();

            bool online = false;
            Console.WriteLine("Client Server is ");
            while (!online)
            {
                try
                {
                    // ============================================
                    //Present the publicly accessible interface to the client. 
                    // ClientServer is the name for the actual service.

                    host.AddServiceEndpoint(typeof(JobServerInterface), tcp,
                    "net.tcp://127.0.0.1:" + currPort.ToString() + "/ClientServer");
                    //And open the host for business!
                    host.Open();
                    online = true;
                    // ============================================
                }
                catch (AddressAlreadyInUseException iue) 
                { 
                    currPort++; 
                    host = new ServiceHost(typeof(JobServer)); 
                }

                catch (Exception ex) { Debug.WriteLine("GUI: Exception occurred" + ex.Message); }
            }

            // Add client to Web Server.
            RestClient wsv = new RestClient(URL);
            RestRequest request = new RestRequest("api/Client/registerClient"); // Calls GetClient method.
            request.AddJsonBody(new Client("127.0.0.1", currPort));
            IRestResponse resp = wsv.Post(request);

            Console.WriteLine("running on Port: " + currPort + ". Welcome!");

            while (!isClosed) {}

            Debug.WriteLine("Server is closing on port: " + currPort.ToString());
            host.Close();
        }
    }




    // ================================================================
    // ================================================================
    // Setting up Job Server - REFERENCE: Based on Prac 3 Code:

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, InstanceContextMode = InstanceContextMode.Single)]
    internal class JobServer : JobServerInterface
    {
        private static JobServer instance;
        private static List<Job> jobList;
        private Mutex mtx;

        public static JobServer get()
        {
            if (instance == null)
            {
                instance = new JobServer();
            }
            return instance;
        }
        private JobServer()
        {
            jobList = new List<Job>();
            mtx = new Mutex();
        }
        public List<Job> GetJobsList()
        {
            List<Job> export = jobList;
            return export;
        }

        [MethodImpl(MethodImplOptions.Synchronized)] // Tells .NET to only allow one thread at a time to run this function - automatically syncs.
        public bool SubmitSoln(string inSoln, Job inJob) // Submit python code to corresponding Job.
        {
            Job curJob = jobList.Find(j => j.jobID == inJob.jobID);

            if (curJob != null && curJob.solution == null)
            {
                curJob.solution = inSoln;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SubmitJob(Job inJob)
        {
            mtx.WaitOne();
            jobList.Add(inJob);
            mtx.ReleaseMutex();
        }


    }

    //Make this a service contract as it is a service interface
    [ServiceContract]
    public interface JobServerInterface
    {
        //Each of these are service function contracts. They need to be tagged as OperationContracts.
        [OperationContract]
        List<Job> GetJobsList(); // Returns list of jobs.

        [OperationContract]
        bool SubmitSoln(string solution, Job job); // Submit job solution.
        [OperationContract]
        void SubmitJob(Job inJob);
    }

}
