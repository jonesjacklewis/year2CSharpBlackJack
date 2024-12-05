using OOPA1.Database;
using OOPA1.Enums;
using OOPA1.Helpers;
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

namespace OOPA1.Modals
{
    /// <summary>
    /// Interaction logic for ResetUserPasswordModal.xaml
    /// </summary>
    public partial class ResetUserPasswordModal : Window
    {

        private readonly DatabaseController databaseController = new();
        private readonly string username;
        public ResetUserPasswordModal()
        {
            InitializeComponent();

            if (Globals.CurrentUser.Username == "" || Globals.CurrentUser.RoleType != RoleTypes.Admin || Globals.ToEditUsername == null)
            {
                this.Close();
            }

            username = Globals.ToEditUsername ?? "";

            lblUserHolder.Content = username;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Globals.ToEditUsername = null;
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string password1 = pbPassword.Password;
            string password2 = pbPassword.Password;

            if(password1 != password2)
            {
                MessageBox.Show("Invalid Password: Passwords do not match");
                return;
            }

            (bool isValidPassword, string reason) = PasswordHelper.PasswordIsValid(password1);

            if (!isValidPassword)
            {
                MessageBox.Show($"Invalid Password: {reason}");
                return;
            }

            databaseController.ResetUserPassword(username, password1);
            Globals.ToEditUsername = null;

            this.Close();

        }
    }
}
