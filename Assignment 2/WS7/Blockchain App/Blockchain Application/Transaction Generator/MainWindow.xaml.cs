using API_Classes;
using Miner.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

namespace Transaction_Generator
{
    public partial class MainWindow : Window
    {
        private RestClient blcClient;
        private RestClient mnrClient;
        public static Mutex mtx = new Mutex();
        private string minerURL = "https://localhost:44362/";
        private string blockchainURL = "https://localhost:44324/";

        public MainWindow()
        {
            InitializeComponent();
            blcClient = new RestClient(blockchainURL);
            mnrClient = new RestClient(minerURL);
            existTxt.Text = updateNumBlocks().ToString();
            updateListbox();
            //balBox.ItemsSource = 

        }

        private void createBtn_Click(object sender, RoutedEventArgs e)
        { // Button to send transaction 
            string from = walletFromTxt.Text; //Get all input fields 
            string to = walletToTxt.Text;
            string amt = amountTxt.Text;

            uint parsed1, parsed2;
            float parsed3;

            mtx.WaitOne(); // Assure atomicity of each transaction

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
                                // Make transaction.
                                Transaction t = new Transaction();
                                t.walletIDfrom = uint.Parse(from);
                                t.walletIDto = uint.Parse(to);
                                t.amount = float.Parse(amt);

                                RestRequest trReq = new RestRequest("api/Miner/AddTransaction/");
                                trReq.AddJsonBody(t);
                                mnrClient.Post(trReq);
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

        public int updateNumBlocks()
        {
            IRestResponse resp = blcClient.Get(new RestRequest("api/Blockchain/GetCurrentState"));
            //string num = resp.Content.ToString();
            int num = JsonConvert.DeserializeObject<int>(resp.Content);
            return num;
        }

        public void updateListbox()
        {
            IRestResponse resp = blcClient.Get(new RestRequest("api/Blockchain/GetChainList"));
            Class1 c1 = JsonConvert.DeserializeObject<Class1>(resp.Content);
            List<Block> updList = new List<Block>();
            updList = c1.blocks;

            balBox.ItemsSource = updList;

        }


        private void getBtn_Click(object sender, RoutedEventArgs e)
        {
            uint parsed;
            string ID = accIDtxt.Text.ToString();
            try
            {
                if (!string.IsNullOrEmpty(ID))
                {
                    if (uint.TryParse(ID, out parsed))
                    {
                        IRestResponse resp = blcClient.Get(new RestRequest("api/Blockchain/GetBalance/" + parsed));
                        float num = JsonConvert.DeserializeObject<float>(resp.Content);
                        balanceTxt.Text = num.ToString();
                        if(num != 0)
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
            catch(JsonReaderException) { MessageBox.Show("Invalid account ID, please correct. ");  }
            catch(Exception) { }
        }
    }
}
