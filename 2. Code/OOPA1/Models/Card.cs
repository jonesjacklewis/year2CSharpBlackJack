using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using OOPA1.Enums;

namespace OOPA1.Models
{
    /// <summary>
    /// This model represents and is used to represent a Card
    /// </summary>
    public class Card
    {
        public CardValue CardValue { get;  }
        public CardSuit CardSuit { get;  }
        public bool UseHighAce { get; set; } = false;

        public Card(CardValue cardValue, CardSuit cardSuit)
        {
            this.CardValue = cardValue;
            this.CardSuit = cardSuit;

            if(this.CardValue == CardValue.Ace)
            {
                this.UseHighAce = true;
            }
        }

        public int GetCardValue()
        {
            if (UseHighAce) // handles high ace
            {
                return 11;
            }

            if (CardValue >= CardValue.Ten) // handles ten, jack, queen, king
            {
                return 10;
            }

            return (int)CardValue; // handles low ace through to 9
        }

        public void ToggleAce()
        {
            if(CardValue == CardValue.Ace)
            {
                UseHighAce = !UseHighAce; // toggles false --> true, true --> false using boolean not
            }
        }

        public override string ToString()
        {
            // Unicode based cards: https://en.wikipedia.org/wiki/Playing_cards_in_Unicode

            // Replace final ? with code value
            Dictionary<string, string> GetSuiteCode = new()
            {
                { CardSuit.Spades.ToString(), "U0001F0A?" },
                { CardSuit.Hearts.ToString(), "U0001F0B?" },
                { CardSuit.Diamonds.ToString(), "U0001F0C?" },
                { CardSuit.Clubs.ToString(), "U0001F0D?" },
            };

            Dictionary<string, string> GetValueCode = new()
            {
                {CardValue.Ace.ToString(), "1" },
                {CardValue.Two.ToString(), "2" },
                {CardValue.Three.ToString(), "3" },
                {CardValue.Four.ToString(), "4" },
                {CardValue.Five.ToString(), "5" },
                {CardValue.Six.ToString(), "6" },
                {CardValue.Seven.ToString(), "7" },
                {CardValue.Eight.ToString(), "8" },
                {CardValue.Nine.ToString(), "9" },
                {CardValue.Ten.ToString(), "A" },
                {CardValue.Jack.ToString(), "B" },
                {CardValue.Queen.ToString(), "D" },
                {CardValue.King.ToString(), "E" },
            };

            string suitCode = GetSuiteCode[CardSuit.ToString()];
            string valueCode = GetValueCode[CardValue.ToString()];

            string thisCard = suitCode.Replace("?", valueCode);

            int codePoint = int.Parse(thisCard[1..], System.Globalization.NumberStyles.HexNumber);
            string result = char.ConvertFromUtf32(codePoint);

            return result;
        }

        public override bool Equals(object? obj)
        {
            try
            {
                // Pattern Matching: https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/functional/pattern-matching
                if (obj is not Card card) { return false; }

                return card.CardValue == this.CardValue
                    && card.CardSuit == this.CardSuit;
            }
            catch
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CardValue, CardSuit);
        }
    }
}
