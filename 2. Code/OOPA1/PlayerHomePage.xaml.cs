using OOPA1.Database;
using OOPA1.Enums;
using OOPA1.Helpers;
using OOPA1.Models;
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
using System.Windows.Shapes;

namespace OOPA1
{
    /// <summary>
    /// Interaction logic for PlayerHomePage.xaml
    /// </summary>
    public partial class PlayerHomePage : Window
    {

        private readonly DatabaseController databaseController = new();
        private string Username { get; set; }
        private int CurrentBalanace { get; set; }

        /// <summary>
        /// Method to handle to log out process
        /// </summary>
        private void LogOut()
        {
            Globals.CurrentUser.Username = "";
            Globals.CurrentUser.RoleType = RoleTypes.Player;

            MainWindow mainWindow = new();
            mainWindow.Show();
            this.Close();
        }

        public PlayerHomePage()
        {
            InitializeComponent();

            if(Globals.CurrentUser.Username == "" || Globals.CurrentUser.RoleType == RoleTypes.Admin)
            {
                LogOut();
            }

            this.Username = Globals.CurrentUser.Username;

            lblWelcome.Content = $"Welcome {this.Username}";

            this.CurrentBalanace = databaseController.GetBalanceForUser(Username);

            lblCurrentBalanceValue.Content = $"£{CurrentBalanace / 100.0:F2}"; // force to 2 decimal points via string interpolation


            if (this.CurrentBalanace <= 0) // stop user being able to play, if negative/zero balance
            {
                btnPlay.IsEnabled = false;
                btnSendMessage.Visibility = Visibility.Visible;
                btnSendMessage.IsEnabled = true;
            }
        }

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            LogOut();
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            SetBet setBet = new();
            setBet.Show();
            this.Close();
        }

        private void btnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            Message message = new(Username, CurrentBalanace, MessageStates.Unread);
            databaseController.AddMessage(message);

            MessageBox.Show("Message Sent, and admin will approve or decline your request");
        }
    }
}
