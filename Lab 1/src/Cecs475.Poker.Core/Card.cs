using System;

namespace Cecs475.Poker.Cards
{
    /// <summary>
    /// Represents a single card in a 52-card deck of playing cards.
    /// </summary>
    public class Card : IComparable
    {
        public enum CardSuit
        {
            Spades, // 0
            Clubs,  // 1, etc.
            Diamonds,
            Hearts
        }

        public enum CardKind
        {
            Two = 2,
            Three,
            Four,
            Five,
            Six,
            Seven,
            Eight,
            Nine,
            Ten,
            Jack,
            Queen,
            King,
            Ace // == 14
        }

        // A Card consists of a suit and a kind.
        private CardSuit mSuit;
        private CardKind mKind;

        // Constructor
        public Card(CardKind kind, CardSuit suit)
        {
            mSuit = suit;
            mKind = kind;
        }

        public CardSuit Suit
        {
            get { return mSuit; }
            set { mSuit = value; }
        }
        
        public CardKind Kind
        {
            get { return mKind; }
            set { mKind = value; }
        }

        public override string ToString()
        {
            int kindValue = (int)mKind;
            string r = null;
            if (kindValue >= 2 && kindValue <= 10)
            {
                r = kindValue.ToString();
            }
            else
            {
                r = mKind.ToString();
            }
            return r + " of " + mSuit.ToString();
        }

        public int CompareTo(object obj)
        {
            Card c = obj as Card;
            return this.Kind.CompareTo(c.Kind);
        }
    }
}