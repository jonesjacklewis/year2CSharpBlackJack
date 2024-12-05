using OOPA1.Models;
using OOPA1.Enums;

namespace OOPA1_Test
{
    public class CardTests
    {


        /// <summary>
        /// This test is to show that when a card of value ace has its value checked it is 11.
        /// </summary>
        [Test]
        public void ShouldReturnCardValueForAce()
        {
            Card card = new(CardValue.Ace, CardSuit.Clubs);
            int expectedValue = 11;

            Assert.That(card.GetCardValue(), Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// This test is to show that when a card of a numeric value is checked, it is the correct number.
        /// </summary>
        [Test]
        public void ShouldReturnCardValueForNumber()
        {
            Card card = new(CardValue.Five, CardSuit.Clubs);
            int expectedValue = 5;

            Assert.That(card.GetCardValue(), Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// This test is to show that when a card of a royal value (king, queen, jack) is checked, it is 10.
        /// </summary>
        [Test]
        public void ShouldReturnCardValueForRoyal()
        {
            List<Card> cardsToCheck = new()
            {
                new(CardValue.King, CardSuit.Clubs),
                new(CardValue.Queen, CardSuit.Clubs),
                new(CardValue.Jack, CardSuit.Clubs)
            };

            List<int> cardValues = cardsToCheck.Select(card => card.GetCardValue()).ToList();

            List<int> expectedValues = new() { 10, 10, 10};

            Assert.That(cardValues, Is.EquivalentTo(expectedValues));

            Assert.That(cardValues, Is.EqualTo(expectedValues));
        }

        [Test]
        public void ShouldSwitchAceValueOnToggleAce()
        {
            Card card = new(CardValue.Ace, CardSuit.Clubs);
            int expectedValue = 11;

            Assert.That(card.GetCardValue(), Is.EqualTo(expectedValue));

            card.ToggleAce();

            expectedValue = 1;

            Assert.That(card.GetCardValue(), Is.EqualTo(expectedValue));
        }

        [Test]
        public void ShouldShowCardStringValue()
        {
            // two of clubs
            Card card = new(CardValue.Two, CardSuit.Clubs);

            string shouldEqual = "\U0001F0D2";

            Assert.That(card.ToString(), Is.EqualTo(shouldEqual));
        }

    }
}