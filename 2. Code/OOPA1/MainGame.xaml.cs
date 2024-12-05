using OOPA1.Database;
using OOPA1.Helpers;
using OOPA1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace OOPA1
{
    /// <summary>
    /// Interaction logic for MainGame.xaml
    /// </summary>
    public partial class MainGame : Window
    {
        private readonly DatabaseController databaseController = new();
        private readonly int _betAmount;
        private readonly Deck Deck;
        private readonly string Username;
        private readonly int CurrentBalance;

        private readonly InGamePlayer userPlayer;
        private readonly InGamePlayer computer = new("Computer");

        private bool GameOver = false;
        private bool PlayerWin = false;
        private bool ComputerWin = false;

        /// <summary>
        /// Show the cards of a player
        /// </summary>
        /// <param name="player">The player to show the cards of</param>
        /// <param name="onlyShowLast">Defaults false. Whether to only show the players last card. Mainly used for computer</param>
        /// <returns>A string containing the cards as unicode</returns>
        private string ShowCards(InGamePlayer player, bool onlyShowLast = false)
        {
            List<Card> cards = new(player.GetCards());

            List<string> cardStrings = new();

            string unknownCard = "\U0001F0A0"; // unicode code for face down card

            if (onlyShowLast)
            {
                // Don't care about the index, so use underscore
                for(int _ = 0; _ < cards.Count - 1; _++)
                {
                    cardStrings.Add(unknownCard);
                }

                cardStrings.Add(cards[^1].ToString()); // last item in list
            }
            else
            {
                foreach(Card card in cards)
                {
                    cardStrings.Add(card.ToString());
                }
            }

            return String.Join(" ", cardStrings);
        }

        public MainGame()
        {
            InitializeComponent();
            _betAmount = Globals.BetAmount;

            Deck = new();
            Deck.ShuffleDeck();

            this.Username = Globals.CurrentUser.Username;
            this.CurrentBalance = databaseController.GetBalanceForUser(this.Username);

            userPlayer = new(this.Username);

        }

        private void ShowPlayerHomePage()
        {
            PlayerHomePage playerHomePage = new();
            playerHomePage.Show();
            this.Close();
        }

        private void btnFold_Click(object sender, RoutedEventArgs e) { 
            int loseAmount = (int)Math.Floor(Globals.BetAmount * 0.5); // on fold, lose half the bet amount
            int newBalance = this.CurrentBalance - loseAmount;
            databaseController.UpdateUserBalanace(Username, newBalance);

            ShowPlayerHomePage();
        }

        private void LoseCondition()
        {
            MessageBox.Show("You Lose", "You Lose!", MessageBoxButton.OK);
            int newBalance = CurrentBalance - _betAmount; // on a lose, lose entire bet amount

            databaseController.UpdateUserBalanace(Username, newBalance);
            ShowPlayerHomePage();
        }

        // asynchronous to allow for the wait between cards being dealt
        private async Task CompleteComputerTurn()
        {
            int milisecondWait = 1_000;

            lblComputerCardsValue.Content = ShowCards(computer);
            lblComputerScoreValue.Content = computer.GetScore();

            await Task.Delay(milisecondWait);

            while (computer.GetScore() < 17) // computer/dealer must stand on 17
            {
                Card newCard = Deck.GetCard();
                computer.AddCard(newCard);

                lblComputerCardsValue.Content = ShowCards(computer);
                lblComputerScoreValue.Content = computer.GetScore();
                
                await Task.Delay(milisecondWait);
            }
        }

        private void DrawCondition()
        {
            MessageBox.Show("You Drew", "You Drew!", MessageBoxButton.OK);
           
            ShowPlayerHomePage();
        }

        private void WinCondition()
        {
            MessageBox.Show("You Win", "You Win!", MessageBoxButton.OK);
            int newBalance = CurrentBalance + _betAmount;

            databaseController.UpdateUserBalanace(Username, newBalance);
            ShowPlayerHomePage();
        }

        private void HitTwentyOneCondition()
        {
            GameOver = true;

            if(computer.GetScore() == 21)
            {
                DrawCondition();
            }

            WinCondition();
        }

        private async void btnHit_Click(Object sender, RoutedEventArgs e)
        {
            Card newCard = Deck.GetCard();

            userPlayer.AddCard(newCard);
            
            lblPlayerScoreValue.Content = userPlayer.GetScore();
            lblPlayerCardsValue.Content = ShowCards(userPlayer);

            if(userPlayer.GetScore() > 21)
            {
                LoseCondition();
                return;
            }

            if(userPlayer.GetScore() == 21)
            {
                await CompleteComputerTurn();
                HitTwentyOneCondition();
            }

        }

        private async void btnStand_Click(object sender, RoutedEventArgs e)
        {
            btnHit.IsEnabled = false;
            btnFold.IsEnabled = false;

            await CompleteComputerTurn();

            if(computer.GetScore() > 21)
            {
                WinCondition();
                return;
            }

            if(computer.GetScore() > userPlayer.GetScore())
            {
                LoseCondition();
                return;
            }

            if(computer.GetScore() == userPlayer.GetScore())
            {
                DrawCondition();
                return;
            }

            WinCondition();

        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            // hide play button
            btnPlay.Visibility = Visibility.Hidden;

            // show other components
            lblComputerScore.Visibility = Visibility.Visible;
            lblComputerScoreValue.Visibility = Visibility.Visible;
            lblComputerCardsValue.Visibility = Visibility.Visible;

            seperator.Visibility = Visibility.Visible;

            lblPlayerScore.Visibility = Visibility.Visible;
            lblPlayerScoreValue.Visibility = Visibility.Visible;
            lblPlayerCardsValue.Visibility = Visibility.Visible;

            btnFold.Visibility = Visibility.Visible;
            btnHit.Visibility = Visibility.Visible;
            btnStand.Visibility = Visibility.Visible;

            // don't care about the iterator
            for (int _ = 0; _ <= 1; _++)
            {
                userPlayer.AddCard(Deck.GetCard());
                lblPlayerScoreValue.Content = userPlayer.GetScore();
                lblPlayerCardsValue.Content = ShowCards(userPlayer);

                computer.AddCard(Deck.GetCard());
            }

            lblComputerCardsValue.Content = ShowCards(computer, true);

            if (userPlayer.GetScore() == 21)
            {
                GameOver = true;
                PlayerWin = true;

                if (computer.GetScore() == 21)
                {
                    ComputerWin = true;
                    lblComputerCardsValue.Content = ShowCards(computer);
                    lblComputerScoreValue.Content = computer.GetScore();
                }
            }

            if (GameOver)
            {
                btnHit.IsEnabled = false;
                btnFold.IsEnabled = false;
                btnStand.IsEnabled = false;

                if (PlayerWin && ComputerWin) // Draw on load
                {
                    MessageBox.Show("It's a draw", "It's a draw", MessageBoxButton.YesNo);

                    ShowPlayerHomePage();

                    return;
                }

                if (PlayerWin)
                {
                    int newBalance = this.CurrentBalance + Globals.BetAmount;
                    databaseController.UpdateUserBalanace(Username, newBalance);

                    MessageBox.Show("You Win", "You Win", MessageBoxButton.OK);

                    ShowPlayerHomePage();
                }
            }

        }
    }
}
