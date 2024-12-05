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
    /// Interaction logic for SetUserBalanceModal.xaml
    /// </summary>
    public partial class SetUserBalanceModal : Window
    {
        private DatabaseController databaseController = new();
        private string username;

        public SetUserBalanceModal()
        {
            InitializeComponent();

            if(Globals.CurrentUser.Username == "" || Globals.CurrentUser.RoleType != RoleTypes.Admin || Globals.ToEditUsername == null)
            {
                this.Close();
            }

            username = Globals.ToEditUsername ?? "";

            lblUserHolder.Content = username;

            int userCurrentBalancePence = databaseController.GetBalanceForUser(username);

            double balanceInPounds = userCurrentBalancePence / 100.0;
            dudBalance.Value = balanceInPounds < 0 ? 0 : balanceInPounds;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Globals.ToEditUsername = null;
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            int newBalance = Convert.ToInt32(dudBalance.Value * 100);
            databaseController.UpdateUserBalanace(username, newBalance);
            Globals.ToEditUsername = null;
            this.Close();
        }

        private void dudBalance_LostFocus(object sender, RoutedEventArgs e)
        {
            if (dudBalance.Value < 0)
            {
                dudBalance.Value = 0;
            }
        }


        private void dudBalance_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var oldValue = e.OldValue;
            
            if(oldValue is null) { return; }

            double dValue = Convert.ToDouble(oldValue);

            if(dValue < 0)
            {
                dudBalance.Value = 0;
            }
        }
    }
}
