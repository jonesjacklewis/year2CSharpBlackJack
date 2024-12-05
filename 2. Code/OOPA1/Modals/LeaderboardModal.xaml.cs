using OOPA1.Database;
using OOPA1.Models;
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

namespace OOPA1.Modals
{
    /// <summary>
    /// Interaction logic for LeaderboardModal.xaml
    /// </summary>
    public partial class LeaderboardModal : Window
    {
        private DatabaseController databaseController = new();

        private ObservableCollection<UserBalanceRank> leaderboard;

        public LeaderboardModal()
        {
            InitializeComponent();

            leaderboard = new ObservableCollection<UserBalanceRank>(databaseController.GetTopNBalances(5));

            lvLeaderboard.ItemsSource = leaderboard;
        }

    }
}
