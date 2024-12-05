using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using OOPA1.Enums;
using OOPA1.Helpers;

namespace OOPA1.Models
{
    /// <summary>
    /// Models a deck of cards
    /// </summary>
    public class Deck
    {
        public List<Card>? Cards { get; set; }

        public Deck()
        {
            SetUpNewDeck();
        }

        public void SetUpNewDeck()
        {
            Cards = new();

            // diamongs, hearts, clubs, spades
            foreach (string cardSuitString in EnumHelper.GetStringListFromEnum<CardSuit>())
            {
                CardSuit cardSuit = Enum.Parse<CardSuit>(cardSuitString);
                // ace, 2-9, jack, queen, king
                foreach (string cardValueString in EnumHelper.GetStringListFromEnum<CardValue>())
                {
                    CardValue cardValue = Enum.Parse<CardValue>(cardValueString);
                    Cards.Add(new(cardValue, cardSuit));
                }
            }

            ShuffleDeck();
        }

        // Shuffles using Fisher-Yates https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
        public void ShuffleDeck()
        {
            if (Cards is null) // if a null deck set up a new deck
            {
                SetUpNewDeck();
            }

            if(Cards?.Count == 0) // if an empty deck, set up a new deck
            {
                SetUpNewDeck();
            }

            Random random = new();
            for(int i = 0; i < Cards?.Count; i++)
            {
                int j = random.Next(0, i);
                // tuple swapping: https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/ide0180
                (Cards[j], Cards[i]) = (Cards[i], Cards[j]);
            }
        }

        public Card GetCard()
        {
            if(Cards is null) // if a null deck set up a new deck
            {
                SetUpNewDeck();
                ShuffleDeck();
            }

            if(Cards?.Count == 0) // if an empty deck, set up a new deck
            {
                SetUpNewDeck();
                ShuffleDeck();
            }
            // Gets last index https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-8.0/ranges
            Card? cardToReturn = (Cards)?[^1] ?? new(CardValue.Ace, CardSuit.Clubs); // null check
            Cards?.RemoveAt(Cards.Count - 1);

            return cardToReturn;

        }

        public int GetNumberOfCards()
        {
            return Cards?.Count ?? 0; // null check
        }

       
    }
}
