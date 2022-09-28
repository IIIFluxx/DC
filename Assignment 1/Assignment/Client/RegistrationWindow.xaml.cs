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
    /// Interaction logic for RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        private AuthServerInterface foob; // classfield
        public RegistrationWindow()
        {
            InitializeComponent();
            // Set up Auth foob connection.
            ChannelFactory<AuthServerInterface> foobFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://localhost/AuthenticationService";
            foobFactory = new ChannelFactory<AuthServerInterface>(tcp, URL);
            foob = foobFactory.CreateChannel(); // remote connection  
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void registerBtn_Click(object sender, RoutedEventArgs e)
        {
            string username = usernameTxt.Text;
            string password = passwordTxt.Password.ToString(); // toString or not?

            string outcome = foob.Register(username, password);
            MessageBox.Show(outcome);
            if (outcome.Equals("Successfully Registered"))
            {
                this.Close();
            }
        }
    }
}
