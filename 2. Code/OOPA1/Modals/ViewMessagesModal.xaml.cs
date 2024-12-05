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
using OOPA1.Database;
using OOPA1.Enums;
using OOPA1.Helpers;
using OOPA1.Models;

namespace OOPA1.Modals
{
    /// <summary>
    /// Interaction logic for ViewMessagesModal.xaml
    /// </summary>
    public partial class ViewMessagesModal : Window
    {
        private DatabaseController databaseController = new();
        private ObservableCollection<Message> messages;

        public ViewMessagesModal()
        {
            InitializeComponent();
            
            messages = new ObservableCollection<Message>(databaseController.GetMessages());

            lvMessages.ItemsSource = messages;
        }

        /// <summary>
        /// Updates the list view of messages
        /// </summary>
        private void UpdateListView()
        {
            List<Message> listMessage = databaseController.GetMessages();
            messages.Clear();

            foreach (Message m in listMessage)
            {
                messages.Add(m);
            }
        }

        // Approve_Click
        private void Approve_Click(object sender, RoutedEventArgs e)
        {
            string username = ((sender as Button)?.CommandParameter as string) ?? "";

            if(username == "")
            {
                this.Close();
            }

            int oneHundredPounds = 100 * 100;

            Message message = new(username, oneHundredPounds, MessageStates.Approved);

            databaseController.AddMessage(message);
            databaseController.UpdateUserBalanace(username, oneHundredPounds);

            UpdateListView();
        }

        // Decline_Click
        private void Decline_Click(object sender, RoutedEventArgs e)
        {
            string username = ((sender as Button)?.CommandParameter as string) ?? "";

            if (username == "")
            {
                this.Close();
            }

            int currentBalance = databaseController.GetBalanceForUser(username);

            Message message = new(username, currentBalance, MessageStates.Declined);
            databaseController.AddMessage(message);

            UpdateListView();
        }

       
    }
}
