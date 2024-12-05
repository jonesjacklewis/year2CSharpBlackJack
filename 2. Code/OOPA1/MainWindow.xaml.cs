using OOPA1.Database;
using OOPA1.Enums;
using OOPA1.Helpers;
using OOPA1.Modals;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace OOPA1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DatabaseController DatabaseController { get; set; } = new();

       
        public MainWindow()
        {
            InitializeComponent();
            if(Globals.CurrentUser.Username != "")
            {
                txtUsername.Text = Globals.CurrentUser.Username;
            }


            if(!File.Exists("OOP Game Instructions (XS).pdf")) // if instructions do not exist, hide the button
            {
                btnViewGuideBook.Visibility = Visibility.Hidden;
            }
        }

        private void btnLogIn_Click(object sender, RoutedEventArgs e)
        {
            // get user details
            string username = txtUsername.Text.Trim();

            // username validation
            if(username == "")
            {
                MessageBox.Show("Must Enter a Username.");
                return; // early return to stop log in process
            }

            string password = pbPassword.Password;

            // password validation
            if(password == "")
            {
                MessageBox.Show("Must Enter a Password.");
                return; // early return to stop log in process
            }

            string? hash = DatabaseController.GetHashForUser(username);

            if(hash == null)
            {
                MessageBox.Show("Invalid Credentials (No Hash)");
                return; // early return to stop log in process
            }

            string? hashedPassword = DatabaseController.GetHashedPasswordForUser(username);

            if(hashedPassword == null)
            {
                MessageBox.Show("Invalid Credentials (No Hashed Password)");
                return; // early return to stop log in process
            }

            bool correctCredentials = PasswordHelper.ValidateUser(password, hash, hashedPassword);

            if (!correctCredentials)
            {
                MessageBox.Show("Invalid Credentials (Wrong Details)");
                return; // early return to stop log in process
            }

            RoleTypes? roleTypeNullable = DatabaseController.GetRoleTypeForUsername(username);

            if(roleTypeNullable == null)
            {
                MessageBox.Show("Error with getting the correct role type.");
                return;
            }

            RoleTypes roleType = (RoleTypes)roleTypeNullable;

            Globals.CurrentUser = new(username, roleType);

            if(Globals.CurrentUser.RoleType == RoleTypes.Admin)
            {
                AdminHomePage adminHomePage = new();
                adminHomePage.Show();
                this.Close();
                return; // early return, adminHomePage already set up
            }

            PlayerHomePage playerHomePage = new();
            playerHomePage.Show();
            this.Close();
        }

        private void btnCreateUser_Click(object sender, RoutedEventArgs e)
        {
            // get user details
            string username = txtUsername.Text.Trim();

            RoleTypes playerRole = RoleTypes.Player;

            Globals.CurrentUser = new(username, playerRole);

            LowLevelCreateUser lowLevelCreateUser = new();
            lowLevelCreateUser.Show();
            this.Close();
        }

        private void btnViewLeaderboard_Click(object sender, RoutedEventArgs e)
        {
            LeaderboardModal leaderboardModal = new();
            leaderboardModal.ShowDialog();
        }

        private void btnViewGuideBook_Click(object sender, RoutedEventArgs e)
        {
            string file = "OOP Game Instructions (XS).pdf";

            try
            {
                Process.Start(new ProcessStartInfo(file) { UseShellExecute = true }); // open PDF in default pdf reader
            }
            catch
            {
                // Handle exceptions, for instance, if the file is not found
                MessageBox.Show("Something went wrong, unable to find the pdf file.");
            }
        }
    }
}
