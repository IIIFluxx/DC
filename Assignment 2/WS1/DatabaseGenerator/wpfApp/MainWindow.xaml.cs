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


// This is the backend code for your window.

namespace wpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataServerInterface foob;

        public MainWindow()
        {
            InitializeComponent(); //  Constructor pre-filled with a function call that will start up the window renderer.
            // Add to constructor to start up and hold onto a connection to your server.

            // NOTE: This is a factory that generates remote connections to our remote class.
            // This is what hides the RPC stuff!
            //ChannelFactory<ServerProg.DataServerInterface> foobFactory;

            ChannelFactory<DataServerInterface> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();

            //Set the URL and create the connection!
            string URL = "net.tcp://localhost:8100/DataService";
            foobFactory = new ChannelFactory<DataServerInterface>(tcp, URL);
            foob = foobFactory.CreateChannel(); // remote connection
            //Also, tell me how many entries are in the DB.
            totalBox.Text = foob.GetNumEntries().ToString();
            LoadData(0);
            indexBox.Text = "0";
        }

        private void goButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(indexBox.Text, out var index))
            {
                LoadData(index);
            }
            else
            {
                MessageBox.Show($"\"{indexBox.Text}\" is not a valid integer...");
            }
        }

        // REFERENCE: Inspired by Alex's solution.
        private void LoadData(int index)
        {
            try
            {
                foob.GetValuesForEntry(index, out var accNo, out var pin, out var bal, out var fName, out var lName, out var icon);
                firstNameBox.Text = fName;
                lastNameBox.Text = lName;
                balanceBox.Text = bal.ToString("C");
                acctNoBox.Text = accNo.ToString();
                pinBox.Text = pin.ToString("D4");
                // Convert to image source -- From Alex
                userIcon.Source = Imaging.CreateBitmapSourceFromHBitmap(icon.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                icon.Dispose();
            }
            catch (FaultException<IndexOutOfRangeFault> exception)
            {
                MessageBox.Show(exception.Detail.Issue);
            }
        }
    }
}
