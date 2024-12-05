using OOPA1.Models;
using OOPA1.Enums;

namespace OOPA1_Test
{
    public class DeckTests
    {


        /// <summary>
        /// This test is to show that a new deck has 52 unique cards
        /// </summary>
        [Test]
        public void ShouldHave52Cards()
        {
            Deck deck = new();
            int expectedNumber = 52;
            HashSet<Card> cards = new(); // hashset only allows unique items, bsaed on the GetHashCode method of the Card class

            foreach (Card card in deck.Cards)
            {
                cards.Add(card);
            }

            Assert.That(cards.Count, Is.EqualTo(expectedNumber));
        }

        /// <summary>
        /// The order of the deck should change when shuffled. Probability of not chaing = 1/52! approx = 1*10^-68
        /// </summary>
        [Test]
        public void ShouldBeDifferentOnceShuffled()
        {
            Deck deck = new();
            List<Card> originalOrder = new(deck.Cards);

            deck.ShuffleDeck();
            List<Card> shuffledOrder = new(deck.Cards);

            Assert.That(originalOrder.SequenceEqual(shuffledOrder), Is.False);
        }


    }
}