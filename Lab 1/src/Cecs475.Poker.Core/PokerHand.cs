using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cecs475.Poker.Cards
{

    public class PokerHand
    {

        public enum HandType
        {
            HighCard,
            Pair,
            TwoPair,
            ThreeOfKind,
            Straight,
            Flush,
            FullHouse,
            FourOfKind,
            StraightFlush
        }

        private List<Card> hand;
        private HandType type;

        public PokerHand(IEnumerable<Card> card)
        {
            if (hand == null)
            {
                hand = new List<Card>();
            }
            hand.AddRange(card);
            type = GetHandType();
        }

        public int HandSize
        {
            get { return hand.Count; }
        }

        public HandType PokerHandType
        {
            get { return type; }
        }

        private List<Card> GetSorted()
        {
            hand.Sort();
            return hand;
        }

        private HandType GetHandType() // Can definitely be shortened significantly...
        {
            GetSorted();
            // Kinds
            if (hand[0].Kind == hand[1].Kind)
            {
                if (hand[1].Kind == hand[2].Kind)
                {
                    if (hand[2].Kind == hand[3].Kind)
                    {
                        return HandType.FourOfKind;
                    }
                    else if (hand[3].Kind == hand[4].Kind)
                    {
                        return HandType.FullHouse;
                    }
                    return HandType.ThreeOfKind;
                }
                if (hand[2].Kind == hand[3].Kind)
                {
                    if (hand[3].Kind == hand[4].Kind)
                    {
                        return HandType.FullHouse;
                    }
                    return HandType.TwoPair;
                }
                // Pair. Do this return later.
            }
            // Handle reverse cases for Kinds
            if (hand[3].Kind == hand[4].Kind)
            {
                if (hand[2].Kind == hand[3].Kind)
                {
                    if (hand[1].Kind == hand[2].Kind)
                    {
                        return HandType.FourOfKind;
                    }
                    else if (hand[0].Kind == hand[1].Kind)
                    {
                        return HandType.FullHouse;
                    }
                    return HandType.ThreeOfKind;
                }
                if (hand[1].Kind == hand[2].Kind)
                {
                    if (hand[0].Kind == hand[1].Kind)
                    {
                        return HandType.FullHouse;
                    }
                    return HandType.TwoPair;
                }
                // Pair. Do this return later.
            }
            // Handle remaining TwoPair case...
            if (hand[0].Kind == hand[1].Kind && hand[3].Kind == hand[4].Kind)
            {
                return HandType.TwoPair;
            }
            else if (hand[0].Kind == hand[1].Kind || hand[3].Kind == hand[4].Kind)
            {
                return HandType.Pair;
            }
            // Suits and Sequences
            Boolean Flush = true;
            for (int i = 0; i < hand.Count - 1; i++)
            {
                if (hand[i].Suit != hand[i + 1].Suit)
                {
                    Flush = false;
                    break;
                }
            }
            if ((Card.CardKind)((int)hand[0].Kind + 1) == hand[1].Kind)
            {
                if ((Card.CardKind)((int)hand[1].Kind + 1) == hand[2].Kind)
                {
                    if ((Card.CardKind)((int)hand[2].Kind + 1) == hand[3].Kind)
                    {
                        if ((Card.CardKind)((int)hand[3].Kind + 1) == hand[4].Kind)
                        {
                            if (Flush)
                            {
                                return HandType.StraightFlush;
                            }
                            else
                            {
                                return HandType.Straight;
                            }
                        }
                    }
                }
            }
            if (Flush)
            {
                return HandType.Flush;
            }
            return HandType.HighCard;
        }
    }
}
