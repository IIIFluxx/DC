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
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private AuthServerInterface foob; // classfield
        public LoginWindow()
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

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            string username = usernameTxt.Text;
            string password = passwordTxt.Password.ToString(); // toString or not?

            int userToken = foob.Login(username, password);

            if (userToken == -1)
            {
                MessageBox.Show("Provided credentials are not valid...please try again.");
            }
            else
            {
                this.Close();
                ServiceWindow sw = new ServiceWindow();
                sw.ShowDialog();
            }
        }
    }
}

// Pop up textbox for Error input: MessageBox.Show($"\"{indexBox.Text}\" is not a valid integer...");
// lw.Show(); // Shows the window ALONGSIDE the current window.
// lw.ShowDialog(); // Shows the window ON TOP OF the current window (blocks the current window like a message box).
// this.Close();
