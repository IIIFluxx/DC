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
using System.Text.RegularExpressions;


// This is the backend code for your window.

namespace wpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private delegate SearchOperation DoSearch(string query);

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
            //ChannelFactory<ServerProg.DataServerInterface> foobFactory;

            ChannelFactory<BusinessServerInterface> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();

            //Set the URL and create the connection!
            string URL = "net.tcp://localhost:8200/BusinessService";
            foobFactory = new ChannelFactory<BusinessServerInterface>(tcp, URL);
            foob = foobFactory.CreateChannel(); // remote connection
            //Also, tell me how many entries are in the DB.
            totalBox.Text = foob.GetNumEntries().ToString();
            LoadData(0);
            indexBox.Text = "0";
            progressBar.IsIndeterminate = true;
        }

        private void goButton_Click(object sender, RoutedEventArgs e)
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
        }

        private void LoadData(int inIndex)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("In LoadData: " + inIndex); // 
                foob.GetValuesForEntry(inIndex, out var accNo, out var pin, out var bal, out var fName, out var lName, out var icon);
                firstNameBox.Text = fName;
                lastNameBox.Text = lName;
                balanceBox.Text = bal.ToString("C");
                acctNoBox.Text = accNo.ToString();
                pinBox.Text = pin.ToString("D4");
                // Convert to image source -- From Alex
                userIcon.Source = Imaging.CreateBitmapSourceFromHBitmap(icon.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                icon.Dispose();
            }
            catch (FaultException<DBInterface.IndexOutOfRangeFault> exception)
            {
                MessageBox.Show(exception.Detail.Issue);
            }
        }

        // REFERENCE: https://stackoverflow.com/a/6017834/15872054
        private bool isLetters(String inStr)
        {
            return Regex.IsMatch(inStr, @"^[a-zA-Z]+$");
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            if(isLetters(searchBox.Text))
            {
                // AsyncCallback callbackDel = new AsyncCallback(this.onAddCompletion); // From Lecture Slides.
                AsyncCallback callback = new AsyncCallback(this.onSearchCompletion);
                // Delegate Definition: private delegate SearchOperation [Data Type Name] DoSearch [name of fn pointer] (string Q) [param];
                DoSearch search;
                progressBar.Visibility = Visibility.Visible;
                blockView();
                search = new DoSearch(this.asyncSearch);
                string searchEntry = searchBox.Text;
                search.BeginInvoke(searchEntry, callback, null);
            }
            else
            {
                MessageBox.Show("Please enter a valid search string - [ A - Z ] ");
            }
        }

        private SearchOperation asyncSearch(string surname)
        {
            int searchIdx;
            string givenName = "", lastname = "";
            int balanceAmt = 0;
            uint acctNum = 0, pinNum = 0;
            Bitmap inBit;

            try
            {
                searchIdx = foob.searchLastName(surname);
                System.Diagnostics.Debug.WriteLine("searchEntry Index: " + searchIdx); 

                // ==============
                foob.GetValuesForEntry(searchIdx, out acctNum, out pinNum, out balanceAmt, out givenName, out lastname, out inBit);
                // ===============
            }
            catch (FaultException<DBInterface.IndexOutOfRangeFault> exception)
            {
                MessageBox.Show(exception.Detail.Issue);
            }
            SearchOperation retObj = new SearchOperation();

            retObj.lastName = lastname;
            retObj.firstName = givenName;
            retObj.balance = balanceAmt;
            retObj.account = acctNum;
            retObj.pin = pinNum;
            return retObj;
        }

        private void blockView()
        {
            goButton.IsEnabled = false;
            lastNameBox.IsEnabled = false;
            searchButton.IsEnabled = false;
            indexBox.IsEnabled = false;
        }
        private void unblockView()
        {
            searchButton.IsEnabled = true;
            lastNameBox.IsEnabled = true;
            goButton.IsEnabled = true;
            indexBox.IsEnabled = true;
        }

        private void onSearchCompletion(IAsyncResult asyncResult)
        {
            // Get async task 
            AsyncResult asyncObj = (AsyncResult)asyncResult;

            DoSearch makeSearch;

            if(asyncObj.EndInvokeCalled == false) // Must not call EndInvoke more than once.
            {
                makeSearch = (DoSearch)asyncObj.AsyncDelegate;
                SearchOperation result = makeSearch.EndInvoke(asyncObj);
                
                // Update view
                this.Dispatcher.Invoke(() =>
                {
                    firstNameBox.Text = result.firstName;
                    lastNameBox.Text = result.lastName;
                    pinBox.Text = result.pin.ToString("D4");
                    //balanceBox.Text = result.balance.ToString();
                    balanceBox.Text = result.balance.ToString("C");
                    acctNoBox.Text = result.account.ToString();
                });
            }

            // Clean up/reset view
            this.Dispatcher.Invoke(() => {
                unblockView(); 
                progressBar.Visibility = Visibility.Hidden;
            });
            // Close/clean up.
            asyncObj.AsyncWaitHandle.Close();
        }

    }
}
