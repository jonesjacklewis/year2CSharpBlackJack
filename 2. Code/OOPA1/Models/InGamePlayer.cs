using OOPA1.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOPA1.Models
{
    // Represents a player (or the computer) in an active game
    public class InGamePlayer
    {
        public string DisplayName { get; set; }
        private readonly List<Card> Cards = new(); // the player's hand

        public InGamePlayer(string displayName)
        {
            DisplayName = displayName;
        }

        public void AddCard(Card card)
        {
            Cards.Add(card);
        }

        public List<Card> GetCards()
        {
            return Cards;
        }

        public int GetScore()
        {
            int score = 0;

            List<Card> aces = Cards.Where(c => c.CardValue == CardValue.Ace).ToList(); // filter out the aces

            foreach (Card card in Cards)
            {
                score += card.GetCardValue();
            }

            if(aces.Count == 0)
            {
                return score;
            }

            int i = 0;
            while(score > 21 && i < aces.Count) // if gone bust, try to reduce score using aces
            {
                aces[i].ToggleAce();
                score -= 10;
                i++;
            }

            return score;
        }

        public void SetCards(List<Card> cards)
        {
            Cards.Clear();
            foreach(Card card in cards)
            {
                Cards.Add(card);
            }
        }
       
    }
}
