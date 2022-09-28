using Authenticator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for ServiceWindow.xaml
    /// </summary>
    public partial class ServiceWindow : Window
    {
        private AuthServerInterface foob; // classfield
        public ServiceWindow()
        {
            InitializeComponent();
            // Set up Auth foob connection.
            ChannelFactory<AuthServerInterface> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://localhost/AuthenticationService";
            foobFactory = new ChannelFactory<AuthServerInterface>(tcp, URL);
            foob = foobFactory.CreateChannel(); // remote connection.

            // Set URL
            string restURL = "https://localhost:44304/"; // Fixed endpoint for Registry Web Service.
            RestClient client;
            client = new RestClient(restURL);
        }

        private void searchBtn_Click(object sender, RoutedEventArgs e) // Call Search method & load into dropdown.
        {

        }

        private void allServicesBtn_Click(object sender, RoutedEventArgs e) // Call AllServices method & load into dropdown.
        {

        }

        private void serviceSelections_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Based on the Selection, update textboxes based on the service?
        }
    }
}
// Pop up textbox for Error input: MessageBox.Show($"\"{indexBox.Text}\" is not a valid integer...");
// lw.Show(); // Shows the window ALONGSIDE the current window.
// lw.ShowDialog(); // Shows the window ON TOP OF the current window (blocks the current window like a message box).
// this.Close();