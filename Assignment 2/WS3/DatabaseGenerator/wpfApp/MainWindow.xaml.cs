using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.ServiceModel;
using DatabaseGenerator;
using DBInterface;
using System.Windows.Interop;
using Business_Tier;
using System.Drawing;
using System.Runtime.Remoting.Messaging;
using Newtonsoft.Json;
using RestSharp;
using API_Classes;
using System.Text.RegularExpressions;


// This is the backend code for your window.

namespace wpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RestClient client;

        private class SearchOperation
        {
            public string firstName;
            public string lastName;
            public int balance;
            public uint account;
            public uint pin;
        }

        private BusinessServerInterface foob;

        public MainWindow()
        {
            InitializeComponent(); //  Constructor pre-filled with a function call that will start up the window renderer.
            // Add to constructor to start up and hold onto a connection to your server.

            // NOTE: This is a factory that generates remote connections to our remote class.
            // This is what hides the RPC stuff!

            //ChannelFactory<BusinessServerInterface> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();

            // Set URL
            string URL = "https://localhost:44347/";
            client = new RestClient(URL);
            // ======================
            RestRequest request = new RestRequest("api/values");
            IRestResponse numOfThings = client.Get(request);
            //Also, tell me how many entries are in the DB.
            totalBox.Text = numOfThings.Content;

            /*//Set the URL and create the connection!
            string URL = "net.tcp://localhost:8200/BusinessService";
            foobFactory = new ChannelFactory<BusinessServerInterface>(tcp, URL);
            foob = foobFactory.CreateChannel(); // remote connection 
            LoadData(0);*/

            indexBox.Text = "0";
            progressBar.IsIndeterminate = true;
        }

        /*private void goButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(indexBox.Text, out var goIndex))
            {
                System.Diagnostics.Debug.WriteLine("goBtn Index: " + goIndex); // 
                LoadData(goIndex);
            }
            else
            {
                MessageBox.Show($"\"{indexBox.Text}\" is not a valid integer...");
            }
        }*/

        private void goButton_Click(object sender, RoutedEventArgs e)
        {
            //On click, Get the index....
            if (int.TryParse(indexBox.Text, out var goIndex))
            {
                int index = Int32.Parse(indexBox.Text);
                //Then, set up and call the API method...
                RestRequest request = new RestRequest("api/GetValues/" + index.ToString());
                IRestResponse resp = client.Get(request);
                // And now use the JSON Deserializer to deseralize our object back to the class we want
                API_Classes.DataIntermed dataIntermed = JsonConvert.DeserializeObject<API_Classes.DataIntermed>(resp.Content);
                //And now, set the values in the GUI!

                firstNameBox.Text = dataIntermed.fname;
                lastNameBox.Text = dataIntermed.lname;
                balanceBox.Text = dataIntermed.bal.ToString("C");
                acctNoBox.Text = dataIntermed.acct.ToString();
                pinBox.Text = dataIntermed.pin.ToString("D4");
                //userIcon.Source = dataIntermed.icon;
                //userIcon.Source = Imaging.CreateBitmapSourceFromHBitmap(icon.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                //icon.Dispose();
            }
            else
            {
                MessageBox.Show($"\"{indexBox.Text}\" is not a valid integer...");
            }

        }

        private void LoadData(int inIndex)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("In LoadData: " + inIndex); // 
                //foob.GetValuesForEntry(inIndex, out var accNo, out var pin, out var bal, out var fName, out var lName, out var icon);
                foob.GetValuesForEntry(inIndex, out var accNo, out var pin, out var bal, out var fName, out var lName);
                firstNameBox.Text = fName;
                lastNameBox.Text = lName;
                balanceBox.Text = bal.ToString("C");
                acctNoBox.Text = accNo.ToString();
                pinBox.Text = pin.ToString("D4");
                // Convert to image source -- From Alex
                //userIcon.Source = Imaging.CreateBitmapSourceFromHBitmap(icon.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                //icon.Dispose();
            }
            catch (FaultException<DBInterface.IndexOutOfRangeFault> exception)
            {
                MessageBox.Show(exception.Detail.Issue);
            }
        }

        // Delegate for async search
        // Delegate Definition: private delegate SearchOperation [=Data Type Name] DoSearch [=name of fn pointer] (string Q) [=param];
        private delegate DataIntermed DoSearch(string query);

        // REFERENCE: https://stackoverflow.com/a/6017834/15872054
        private bool isLetters(String inStr)
        {
            return Regex.IsMatch(inStr, @"^[a-zA-Z]+$");
        }


        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            if (isLetters(searchBox.Text))
            {
                //Set callback function to run when async task finishes running
                AsyncCallback callback = this.OnSearchComplete;
                string lName = searchBox.Text;
                progressBar.Visibility = Visibility.Visible;
                //blockView();
                DoSearch search = asyncSearch;
                //Set delegate target function
                search.BeginInvoke(lName, callback, null);
            }
            else
            {
                MessageBox.Show("Please enter a valid search string - [ A - Z ] ");
            }
        }

        
        private DataIntermed asyncSearch(string surname)
        {
            //Use API class search template
            SearchData search = new SearchData();
            search.searchStr = surname;

            //Set URL req - POST fn & add searchData
            RestRequest req = new RestRequest(String.Concat("api/search/"));
            req.AddJsonBody(search);

            //Do the request
            IRestResponse resp = client.Post(req);

            //Deserialize and send to callback.
            return JsonConvert.DeserializeObject<DataIntermed>(resp.Content);
        }

        public void OnSearchComplete(IAsyncResult asyncResult)
        {

            // Get async task
            AsyncResult asyncObj = (AsyncResult)asyncResult;
            DoSearch makeSearch;

            // Must not call EndInvoke more than once.
            if (asyncObj.EndInvokeCalled == false)
            {
                makeSearch = (DoSearch)asyncObj.AsyncDelegate;
                DataIntermed result = makeSearch.EndInvoke(asyncObj);
                this.Dispatcher.Invoke(() =>
                {
                    firstNameBox.Text = result.fname;
                    lastNameBox.Text = result.lname;
                    acctNoBox.Text = result.acct.ToString();
                    pinBox.Text = result.pin.ToString("D4");
                    balanceBox.Text = result.bal.ToString("C");
                    // Clean up/reset view
                    progressBar.Visibility = Visibility.Hidden;
                });
            }

            // Close/clean up.
            asyncObj.AsyncWaitHandle.Close();
        }
  

    }
}
