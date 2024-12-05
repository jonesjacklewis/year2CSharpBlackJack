using OOPA1.Database;
using OOPA1.Enums;
using OOPA1.Helpers;
using OOPA1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OOPA1.Modals
{
    /// <summary>
    /// Interaction logic for DetailedAddUserModal.xaml
    /// </summary>
    public partial class DetailedAddUserModal : Window
    {
        private readonly DatabaseController databaseController = new();
        private readonly List<string> validRoles;

        public DetailedAddUserModal()
        {
            InitializeComponent();

            validRoles = databaseController.GetRoleTypes();

            cbRoles.Items.Add(""); // adds a blank role

            foreach(string role in validRoles)
            {
                cbRoles.Items.Add(role);
            }

            dudBalance.Value = Double.Parse("0.0");
            dudBalance.Visibility = Visibility.Hidden;
            lblBalance.Visibility = Visibility.Hidden;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();

            if (username.Length < 6)
            {
                MessageBox.Show("Username Length must be 6 or more characters.");
                return;
            }

            if (databaseController.CheckUserExists(username))
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

            string role = cbRoles.Text;

            if (!validRoles.Contains(role))
            {
                MessageBox.Show($"Invalid Role");
                return;
            }

            int balance = Convert.ToInt32(dudBalance.Value * 100);

            if(balance < 0)
            {
                MessageBox.Show("Cannot create a user in debt.");
                return;
            }

            RoleTypes roleType = Enum.Parse<RoleTypes>(role);
            int roleId = databaseController.GetRoleTypeIdForRoleType(roleType);


            DbUserWithBalanace newUser = new(username, password1, roleId, balance);

            databaseController.AddUserBeforeHashWithBalanace(newUser);

            txtUsername.Text = "";
            pbPassword.Password = "";
            pbPasswordConfirm.Password = "";
            cbRoles.Text = "";
            dudBalance.Value = Double.Parse("100.00");
        }

        private void cbRoles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbRoles.SelectedItem as string != "Player") // should only be able to set the balance of a player
            {
                dudBalance.Value = Double.Parse("0.0");
                dudBalance.Visibility = Visibility.Hidden;
                lblBalance.Visibility = Visibility.Hidden;
                return;
            }

            dudBalance.Value = Double.Parse("100.00");
            dudBalance.Visibility = Visibility.Visible;
            lblBalance.Visibility = Visibility.Visible;
        }

      
    }
}
