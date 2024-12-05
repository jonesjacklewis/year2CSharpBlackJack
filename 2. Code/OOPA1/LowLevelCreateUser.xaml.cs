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
using OOPA1.Database;
using OOPA1.Enums;
using OOPA1.Helpers;
using OOPA1.Models;

namespace OOPA1
{
    /// <summary>
    /// Interaction logic for LowLevelCreateUser.xaml
    /// </summary>
    public partial class LowLevelCreateUser : Window
    {
        DatabaseController DatabaseController { get; set; } = new();
        public LowLevelCreateUser()
        {
            InitializeComponent();

            string username = Globals.CurrentUser.Username;

            if(username != "")
            {
                txtUsername.Text = username;
            }

            lblRoleTypeValue.Content = RoleTypes.Player; // only allow Player creation
        }

        /// <summary>
        /// This method opens the MainWindow, and closes the current window
        /// </summary>
        private void ReturnToMainWindow()
        {
            MainWindow mainWindow = new();
            mainWindow.Show();
            this.Close();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            ReturnToMainWindow();
        }

        private void btnCreateUser_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();

            if (username.Length < 6)
            {
                MessageBox.Show("Username Length must be 6 or more characters.");
                return;
            }

            if (DatabaseController.CheckUserExists(username))
            {
                MessageBox.Show("Username already in use.");
                return;
            }

            string password1 = pbPassword.Password;
            string password2 = pbPasswordConfirm.Password;

            if (password1 != password2)
            {
                MessageBox.Show("Passwords do not match.");
                return;
            }

            (bool isValidPassword, string reason) = PasswordHelper.PasswordIsValid(password1);

            if (!isValidPassword)
            {
                MessageBox.Show($"Invalid Password: {reason}");
                return;
            }

            int playerRoleTypeId = DatabaseController.GetRoleTypeIdForRoleType(RoleTypes.Player);

            DbUser dbUser = new(username, password1, playerRoleTypeId);

            DatabaseController.AddUserBeforeHash(dbUser);
            Globals.CurrentUser.Username = username;
            Globals.CurrentUser.RoleType = RoleTypes.Player;

            MessageBox.Show("User Successfully Created");

            ReturnToMainWindow();
        }
    }
}
