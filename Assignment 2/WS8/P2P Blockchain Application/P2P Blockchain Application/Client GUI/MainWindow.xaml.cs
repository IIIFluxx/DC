using API_Classes;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
using Block = API_Classes.Block;

namespace Client_GUI
{

    public delegate void startMiner();
    public delegate void startServer();

    public partial class MainWindow : Window
    {
        private string serverIP = "127.0.0.1";
        private uint currPort = 8100;
        private bool isClosed = false;
        private bool online = false;
        private Dictionary<string, int> dic;
        public static Mutex mtx = new Mutex();
        private RestClient client;
        private string URL = "https://localhost:44396/";

        public MainWindow()
        {
            startMiner miner = Miner;
            startServer server = Server;

            miner.BeginInvoke(null, null);
            server.BeginInvoke(null, null);

            InitializeComponent();
            dic = new Dictionary<string, int>();
            client = new RestClient(URL);
            existTxt.Text = updateNumBlocks().ToString();
            updateListbox();
        }

        private void onClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Debug.WriteLine("Shutting down " + currPort.ToString());
            RestRequest remReq = new RestRequest("api/Client/removeClient");
            remReq.AddJsonBody(new Client("127.0.0.1", currPort));
            client.Post(remReq);
            isClosed = true;
        }

        private void createBtn_Click(object sender, RoutedEventArgs e) // Creates in a Transaction
        { // Button to send transaction 
            string from = walletFromTxt.Text; //Get all input fields 
            string to = walletToTxt.Text;
            string amt = amountTxt.Text;

            uint parsed1, parsed2;
            float parsed3;

            mtx.WaitOne(); // Assure atomicity of each transaction

            IRestResponse resp = client.Get(new RestRequest("api/Client/"));
            List<Client> list = JsonConvert.DeserializeObject<List<Client>>(resp.Content);

            try
            {
                if (!String.IsNullOrEmpty(from) && !String.IsNullOrEmpty(to) && !String.IsNullOrEmpty(amt))
                { // Check if any empty or null values submitted.
                    if (uint.TryParse(from.ToString(), out parsed1) && uint.TryParse(to.ToString(), out parsed2))
                    { // Check if submitted values are indeed numbers 
                        if (float.TryParse(amt.ToString(), out parsed3))
                        { // and amount is a float

                            if (from != to || (from == "0" && to == "0"))
                            {

                                foreach (Client cur in list) //Send new transaction to all clients 
                                {
                                    try
                                    {
                                        NetTcpBinding tcp = new NetTcpBinding();
                                        string clientURL = "net.tcp://" + cur.IPAddress.ToString() + ":" + cur.portNum.ToString() + "/BlockchainServer";
                                        ChannelFactory<BlockchainServerInterface> foobFactory;
                                        foobFactory = new ChannelFactory<BlockchainServerInterface>(tcp, clientURL);
                                        BlockchainServerInterface foob = foobFactory.CreateChannel();

                                        // Make transaction.
                                        Transaction t = new Transaction();
                                        t.walletIDfrom = uint.Parse(from);
                                        t.walletIDto = uint.Parse(to);
                                        t.amount = float.Parse(amt);
                                        foob.ReceiveNewTransaction(t);
                                    }
                                    catch(EndpointNotFoundException)
                                    {
                                        MessageBox.Show("Invalid endpoint - connection failed");
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("Fatal error occurred - " +ex.Message);
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Cannot transact yourself money!");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please provide valid values!");
                    }
                }
                else
                {
                    MessageBox.Show("Please provide non-empty, valid values!");
                }

                existTxt.Text = updateNumBlocks().ToString();
                updateListbox();
                mtx.ReleaseMutex();
            }
            catch (Exception ex) { MessageBox.Show("Fatal error occurred - " + ex.Message); }
        }

        private void getBtn_Click(object sender, RoutedEventArgs e) // Get balance.
        {
            uint parsed;
            string ID = accIDtxt.Text.ToString();
            try
            {
                if (!string.IsNullOrEmpty(ID))
                {
                    if (uint.TryParse(ID, out parsed))
                    {
                        float num = Blockchain.getBalance(parsed);
                        balanceTxt.Text = num.ToString();
                        if (num != 0)
                        {
                            Debug.WriteLine("Balance for account " + accIDtxt.Text + " found. Balance = " + num);
                        }
                        else
                        {
                            Debug.WriteLine("Balance for account not found.");
                        }
                        Debug.WriteLine("------------~~~~~~~---------~~~~~~~----------~~~~~~~--------~~~~~~~-----------");

                    }
                    else
                    {
                        MessageBox.Show("Please enter a valid account ID");
                    }
                }
                else
                {
                    MessageBox.Show("Please fill the account ID textbox");
                }
                existTxt.Text = updateNumBlocks().ToString();
                updateListbox();
            }
            catch (JsonReaderException) { MessageBox.Show("Invalid account ID, please correct. "); }
            catch (Exception ex) { Debug.WriteLine("Fatal error occured: " + ex.Message); }
        }

        public int updateNumBlocks()
        {
            return Blockchain.Size();
        }

        public void updateListbox()
        {
            List<Block> updList = new List<Block>();
            updList = Blockchain.getChain();
            walletBox.Items.Refresh();
            balBox.Items.Refresh();
            balBox.ItemsSource = updList;
            walletBox.ItemsSource = Blockchain.getChain();



        }

        public void Server()
        {
            //This is the actual host service system
            ServiceHost host;

            //Bind server to the implementation of JobServer
            host = new ServiceHost(typeof(BlockchainServer));

            bool online = false;
            Console.WriteLine("Blockchain Server is ");

            Debug.WriteLine("Server Address = " + "net.tcp://" + serverIP + ":" + currPort.ToString() + "/BlockchainServer");

            while (!online)
            {
                try
                {
                    // ============================================
                    //This represents a tcp/ip binding in the Windows network stack
                    NetTcpBinding tcp = new NetTcpBinding();

                    //Present the publicly accessible interface to the client. 
                    // BlockchainServer is the name for the actual service.

                    host.AddServiceEndpoint(typeof(BlockchainServerInterface), tcp,
                    "net.tcp://" +serverIP + ":" + currPort.ToString() + "/BlockchainServer");
                    //And open the host for business!
                    host.Open();
                    online = true;
                    // ============================================

                    registerClient();

                }
                catch (AddressAlreadyInUseException)
                {
                    Debug.WriteLine("GUI: Address in use. Retrying.");
                    currPort++;
                    host = new ServiceHost(typeof(BlockchainServer));
                }

                catch (Exception ex) { Debug.WriteLine("Fatal error occured: " + ex.Message); }
            }

            while (!isClosed) { }

            Debug.WriteLine("Server is closing on port: " + currPort.ToString());
            host.Close();
        }

        public void registerClient()
        {
            // Add client to Web Server.
            RestClient wsv = new RestClient(URL);
            RestRequest request = new RestRequest("api/Client/registerClient"); // Calls GetClient method.
            request.AddJsonBody(new Client("127.0.0.1", currPort));
            IRestResponse resp = wsv.Post(request);
            Console.WriteLine("running on Port: " + currPort + ". Welcome!");
        }


        public void Miner()
        {
            RestClient client = new RestClient(URL);
            NetTcpBinding tcp = new NetTcpBinding();

            while (!isClosed)
            {
                try
                {
                    Queue<Transaction> transactions = Transactions.getTransactions(); // EDIT: Moved into While Loop.

                    if (transactions.Count > 0)
                    {
                        Transaction t = transactions.Dequeue();

                        if (!t.processed) // Get only the non-processed 
                        {
                            Debug.WriteLine("Processing transaction: " + t.walletIDfrom.ToString() + " --> " + t.walletIDto.ToString());

                            /*
                             * 1. Validate the transaction details with the server, this includes
                                    1. Are there enough coins in the sender’s account to allow this transaction?
                                    2. Is the amount valid (greater than 0)?
                                    3. Are all numbers involved with this transaction not negative?
                             */

                            // #1: Validate transaction details

                            if (t.walletIDto >= 0 && t.walletIDfrom >= 0 // 3. Are all numbers involved with this transaction not negative?
                                && t.amount > 0) // 2. Is the amount valid (greater than 0)?
                            {
                                float walletBal = Blockchain.getBalance(t.walletIDfrom);
                                Debug.WriteLine("# of Blockchains: " + Blockchain.Size());
                                // 1. Are there enough coins in the sender’s account to allow this transaction?
                                if (walletBal >= t.amount)
                                {
                                    Debug.WriteLine("Adding block to chain list");

                                    // #2. Insert the transaction details into a block.
                                    Block newBlock = new Block();

                                    // 3. Pull down the last block from the current blockchain, and insert the hash of that block into the new block
                                    Block lastBlock = Blockchain.findLast();
                                    /*
                                    public uint blockID; // Uniquely identifies the Block
                                    public uint walletIDfrom; // Identifies source of transaction
                                    public uint walletIDto; // Identifies destination of transaction
                                    public float amount; // Amount of money being transacted.
                                    public uint offset; // Ensures validity of Hash (multiple of 5).
                                    public string prevBlockHash;
                                    public string blockHash;
                                     */
                                    newBlock.blockID = 1 + lastBlock.blockID;
                                    newBlock.walletIDfrom = t.walletIDfrom;
                                    newBlock.walletIDto = t.walletIDto;
                                    newBlock.amount = t.amount;
                                    newBlock.offset = 0;
                                    newBlock.prevBlockHash = lastBlock.blockHash;
                                    newBlock.blockHash = "";
                                    // 4. Brute force a valid hash (one that starts with 12345) + 5. Insert the now valid hash and hash offset into the block
                                    newBlock = GenHashCode(newBlock);

                                    // Submit the block to the Bank Server for inclusion into the blockchain
                                    Blockchain.addBlock(newBlock);
                                    Transactions.setProcessed(t);
                                    Dispatcher.Invoke(() => { existTxt.Text = Blockchain.Size().ToString(); });

                                }
                            }
                        }
                    }// End if (transactions.Count > 0)
                    else
                    {
                        Debug.WriteLine("Empty transaction queue");
                    }
                }
                catch (InvalidOperationException ioEx)
                {
                    Debug.WriteLine("Error: " + ioEx.ToString());
                    Debug.WriteLine("Empty transaction queue");
                }
                catch (NullReferenceException nEx) { Debug.WriteLine("Error: " + nEx.ToString()); }
                catch (Exception ex) { Debug.WriteLine("Error: " + ex.ToString()); }



                // ========================= Addition from Prac 7 =========================
                // Determine most popular blockchain:

                // 1. Gather current hashes from all clients
                // 2. Find which Hash occurs the most in the "prevHash" field?
                // If popular == current block, all good.
                // If not, download/overwrite popular.

                /***** REFERENCE: https://stackoverflow.com/questions/42726292/hash-table-to-find-the-number-that-appears-the-most-in-a-list *****/
                // ======================================================
                //refreshDic();
                // ======================================================
                try
                {
                    IRestResponse resp = client.Get(new RestRequest("api/Client/"));
                    List<Client> list = JsonConvert.DeserializeObject<List<Client>>(resp.Content);

                    if (list != null)
                    {
                        foreach (Client cur in list)
                        {
                            string clientURL = "net.tcp://" + cur.IPAddress.ToString() + ":" + cur.portNum.ToString() + "/BlockchainServer";
                            ChannelFactory<BlockchainServerInterface> foobFactory;
                            foobFactory = new ChannelFactory<BlockchainServerInterface>(tcp, clientURL);
                            BlockchainServerInterface foob = foobFactory.CreateChannel();

                            if (dic.ContainsKey(foob.GetCurrentBlock().blockHash))
                            {
                                dic[foob.GetCurrentBlock().blockHash] += 1;
                            }
                            else
                            {
                                dic.Add(foob.GetCurrentBlock().blockHash, 1);
                            }
                        }

                        // ======================================================
                        //string popHash = getMVH();
                        int bestKey = 0;
                        string popHash = "";

                        foreach (KeyValuePair<string, int> d in dic)
                        {
                            if (d.Value >= bestKey)
                            {
                                popHash = d.Key;
                                bestKey = d.Value;
                            }
                        }
                        // ======================================================
                        Debug.WriteLine("Most popular hash: " + popHash);

                        if (Blockchain.findLast().blockHash == popHash) //If Current Blockchain doesn't have the most popular end block
                        {
                            Debug.WriteLine("Last block in the current blockchain is currently the most popular. ");
                        }
                        else
                        {
                            // Otherwise, download more popular blockchain from someone else and replace its own blockchain with that one
                            Debug.WriteLine("Current blockchain is not most popular. Downloading most valuable block. ");
                            //updateChain(popHash);
                            foreach (Client cur in list)
                            {
                                string clientURL = "net.tcp://" + cur.IPAddress.ToString() + ":" + cur.portNum.ToString() + "/BlockchainServer";
                                ChannelFactory<BlockchainServerInterface> foobFactory;
                                foobFactory = new ChannelFactory<BlockchainServerInterface>(tcp, clientURL);
                                BlockchainServerInterface foob = foobFactory.CreateChannel();
                                if (foob.GetCurrentBlock().blockHash == popHash)
                                {
                                    Blockchain.setChain(foob.GetCurrentBlockchain());
                                    Debug.WriteLine("Blockchain has been changed.");
                                    break;
                                }
                            }
                        }
                    }
                }
                catch(EndpointNotFoundException)
                {
/*                   if (isClosed)
                    {
                        Debug.WriteLine("Shutting down " + currPort.ToString());
                        RestRequest remReq = new RestRequest("api/Client/removeClient");
                        remReq.AddJsonBody(new Client("127.0.0.1", currPort));
                        client.Post(remReq);
                    }*/
                }
                // ========================================================================
                Dispatcher.Invoke(() => { existTxt.Text = Blockchain.Size().ToString(); });
                Thread.Sleep(5000);
            } // End while
        }


        public void refreshDic()
        {
            IRestResponse resp = client.Get(new RestRequest("api/Client/"));
            List<Client> list = JsonConvert.DeserializeObject<List<Client>>(resp.Content);

            if (list != null)
            {
                foreach (Client cur in list)
                {
                    NetTcpBinding tcp = new NetTcpBinding();
                    string clientURL = "net.tcp://" + cur.IPAddress.ToString() + ":" + cur.portNum.ToString() + "/BlockchainServer";
                    ChannelFactory<BlockchainServerInterface> foobFactory;
                    foobFactory = new ChannelFactory<BlockchainServerInterface>(tcp, clientURL);
                    BlockchainServerInterface foob = foobFactory.CreateChannel();

                    if (dic.ContainsKey(foob.GetCurrentBlock().blockHash))
                    {
                        dic[foob.GetCurrentBlock().blockHash] += 1;
                        // CHECK  <-----------------------------
                    }
                    else
                    {
                        dic.Add(foob.GetCurrentBlock().blockHash, 1);
                    }
                }
            }
        }

        public string getMVH() // Get most valuable hash
        {
            IRestResponse resp = client.Get(new RestRequest("api/Client/"));
            List<Client> list = JsonConvert.DeserializeObject<List<Client>>(resp.Content);
            int bestKey = 0;
            string popHash = "";

            if(list != null)
            {
                foreach (KeyValuePair<string, int> d in dic)
                {
                    if (d.Value >= bestKey)
                    {
                        popHash = d.Key;
                        bestKey = d.Value;
                    }
                }
            }
            return popHash;
        }


        public void updateChain(string popHash)
        {
            IRestResponse resp = client.Get(new RestRequest("api/Client/"));
            List<Client> list = JsonConvert.DeserializeObject<List<Client>>(resp.Content);
            if (list != null)
            {
                foreach (Client cur in list)
                {
                    NetTcpBinding tcp = new NetTcpBinding();
                    string clientURL = "net.tcp://" + cur.IPAddress.ToString() + ":" + cur.portNum.ToString() + "/BlockchainServer";
                    ChannelFactory<BlockchainServerInterface> foobFactory;
                    foobFactory = new ChannelFactory<BlockchainServerInterface>(tcp, clientURL);
                    BlockchainServerInterface foob = foobFactory.CreateChannel();
                    if (foob.GetCurrentBlock().blockHash == popHash)
                    {
                        Blockchain.setChain(foob.GetCurrentBlockchain());
                        Debug.WriteLine("Blockchain has been changed.");
                        break;
                    }
                }
            }
        }
        
        public Block GenHashCode(Block b) { return Blockchain.GenHashCode(b); }

    }
}
