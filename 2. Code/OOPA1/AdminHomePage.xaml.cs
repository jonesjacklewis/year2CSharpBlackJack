using OOPA1.Database;
using OOPA1.Enums;
using OOPA1.Helpers;
using OOPA1.Modals;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for AdminHomePage.xaml
    /// </summary>
    public partial class AdminHomePage : Window
    {
        private DatabaseController databaseController = new();
        private ObservableCollection<string> players = new();

        public AdminHomePage()
        {
            InitializeComponent();

            // Return to main window if not a specific user or not an admin 
            if(Globals.CurrentUser.Username == "" || Globals.CurrentUser.RoleType != Enums.RoleTypes.Admin)
            {
                OpenMainWindow();
            }

            lblWelcomeMessage.Content = $"Welcome {Globals.CurrentUser.Username}";

            List<string> listPlayers = databaseController.GetUsersForSpecificRole(RoleTypes.Player);
            players.Clear();

            foreach(string player in listPlayers)
            {
                players.Add(player);
            }

            lvPlayers.ItemsSource = players;
        }

        /// <summary>
        /// Opens the main window and closes the current one
        /// </summary>
        private void OpenMainWindow()
        {
            MainWindow mainWindow = new();
            mainWindow.Show();
            this.Close();
        }

        private void btnResetAllCredit_Click(object sender, RoutedEventArgs e)
        {
            // Loosely based on https://stackoverflow.com/questions/18315786/confirmation-box-in-c-sharp-wpf

            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure?", "Reset All Credit", MessageBoxButton.YesNo);

            // early return if not a confirm
            if(messageBoxResult != MessageBoxResult.Yes){ return; }

            databaseController.ResetAllBalance();

            MessageBox.Show("Reset All Balances");
        }

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            Globals.CurrentUser = new("", RoleTypes.Player);
            OpenMainWindow();
        }

        private void ResetPassword_Click(object sender, RoutedEventArgs e)
        {
            string username = ((sender as Button)?.CommandParameter as string) ?? "";

            if(username == "")
            {
                btnLogOut_Click(sender, e);
            }

            Globals.ToEditUsername = username;

            ResetUserPasswordModal resetUserPasswordModal = new();
            resetUserPasswordModal.ShowDialog();
        }

        private void SetBalance_Click(object sender, RoutedEventArgs e)
        {
            string username = ((sender as Button)?.CommandParameter as string) ?? "";

            if (username == "")
            {
                btnLogOut_Click(sender, e);
            }

            Globals.ToEditUsername = username;

            SetUserBalanceModal setUserBalanceModal = new();
            setUserBalanceModal.ShowDialog();


        }

        private void btnCreateUser_Click(object sender, RoutedEventArgs e)
        {
            DetailedAddUserModal detailedAddUserModal = new();
           
            detailedAddUserModal.ShowDialog();

            List<string> listPlayers = databaseController.GetUsersForSpecificRole(RoleTypes.Player);
            players.Clear();

            foreach (string player in listPlayers)
            {
                players.Add(player);
            }

            lvPlayers.ItemsSource = players;


        }

        private void btnViewMessages_Click(object sender, RoutedEventArgs e)
        {
            ViewMessagesModal viewMessagesModal = new();
            viewMessagesModal.ShowDialog();
        }

      
    }
}
