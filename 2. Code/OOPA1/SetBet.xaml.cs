using OOPA1.Database;
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

namespace OOPA1
{
    /// <summary>
    /// Interaction logic for SetBet.xaml
    /// </summary>
    public partial class SetBet : Window
    {
        private readonly DatabaseController _databaseController = new();
        private readonly string? _username;
        private readonly int _currentBalanace;

        public SetBet()
        {
            InitializeComponent();
            this._username = Globals.CurrentUser.Username;

            if(this._username == null || this._username == "")
            {
                this.Close();
            }

            lblUsernameValue.Content = this._username;
            this._currentBalanace = _databaseController.GetBalanceForUser(this._username ?? ""); // null check/protection

            sBetAmount.Minimum = 0.0;
            sBetAmount.Maximum = this._currentBalanace / 100.0; // show in pounds

            double halfCurrentBalanace = Math.Floor(this.sBetAmount.Maximum / 2.0);

            sBetAmount.Value = halfCurrentBalanace;
            sBetAmount.TickFrequency = 0.5;

            lblCurrentValue.Content = $"£{sBetAmount.Value:N2}"; // 2dp with comma seperators
            lblMinimumValue.Content = $"£{sBetAmount.Minimum:N2}";
            lblMaximumValue.Content = $"£{sBetAmount.Maximum:N2}";

            sBetAmount.UpdateDefaultStyle();
            sBetAmount.UpdateLayout();


        }

        private void sBetAmount_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lblCurrentValue.Content = $"£{e.NewValue:N2}";
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            PlayerHomePage playerHomePage = new();
            playerHomePage.Show();
            this.Close();
        }

        private void btnSetBet_Click(object sender, RoutedEventArgs e)
        {
            int betValue = (int)Math.Floor(sBetAmount.Value * 100);
            Globals.BetAmount = betValue;

            MainGame mainGame = new();
            mainGame.Show();
            this.Close();
           
            

        }
    }
}
