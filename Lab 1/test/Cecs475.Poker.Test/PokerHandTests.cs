using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;
using FluentAssertions;
using Cecs475.Poker.Cards;

namespace Cecs475.Poker.Test
{
    public class Program
    {
        [Fact]
        public void FourOfAKind()
        {
            PokerHand h = new PokerHand(new Card[] {
                new Card(Card.CardKind.Ace, Card.CardSuit.Clubs),
                new Card(Card.CardKind.Ace, Card.CardSuit.Spades),
                new Card(Card.CardKind.Ace, Card.CardSuit.Hearts),
                new Card(Card.CardKind.Ace, Card.CardSuit.Diamonds),
                new Card(Card.CardKind.Three, Card.CardSuit.Clubs)});

            var type = h.PokerHandType;
            type.Should().Be(PokerHand.HandType.FourOfKind);
        }

        [Fact]
        public void ThreeOfAKind()
        {
            PokerHand h = new PokerHand(new Card[] {
                new Card(Card.CardKind.Four, Card.CardSuit.Clubs),
                new Card(Card.CardKind.Nine, Card.CardSuit.Clubs),
                new Card(Card.CardKind.Four, Card.CardSuit.Hearts),
                new Card(Card.CardKind.Ace, Card.CardSuit.Diamonds),
                new Card(Card.CardKind.Four, Card.CardSuit.Spades)});

            var type = h.PokerHandType;
            type.Should().Be(PokerHand.HandType.ThreeOfKind);
        }

        [Fact]
        public void FullHouse()
        {
            PokerHand h = new PokerHand(new Card[] {
                new Card(Card.CardKind.Four, Card.CardSuit.Clubs),
                new Card(Card.CardKind.Nine, Card.CardSuit.Clubs),
                new Card(Card.CardKind.Four, Card.CardSuit.Hearts),
                new Card(Card.CardKind.Nine, Card.CardSuit.Diamonds),
                new Card(Card.CardKind.Four, Card.CardSuit.Spades)});

            var type = h.PokerHandType;
            type.Should().Be(PokerHand.HandType.FullHouse);
        }

        [Fact]
        public void Pair()
        {
            PokerHand h = new PokerHand(new Card[] {
                new Card(Card.CardKind.Two, Card.CardSuit.Clubs),
                new Card(Card.CardKind.Jack, Card.CardSuit.Clubs),
                new Card(Card.CardKind.Jack, Card.CardSuit.Hearts),
                new Card(Card.CardKind.Nine, Card.CardSuit.Diamonds),
                new Card(Card.CardKind.Four, Card.CardSuit.Spades)});

            var type = h.PokerHandType;
            type.Should().Be(PokerHand.HandType.Pair);
        }

        [Fact]
        public void TwoPair()
        {
            PokerHand h = new PokerHand(new Card[] {
                new Card(Card.CardKind.Queen, Card.CardSuit.Clubs),
                new Card(Card.CardKind.Jack, Card.CardSuit.Clubs),
                new Card(Card.CardKind.Ten, Card.CardSuit.Hearts),
                new Card(Card.CardKind.Queen, Card.CardSuit.Diamonds),
                new Card(Card.CardKind.Ten, Card.CardSuit.Spades)});

            var type = h.PokerHandType;
            type.Should().Be(PokerHand.HandType.TwoPair);
        }

        [Fact]
        public void Straight()
        {
            PokerHand h = new PokerHand(new Card[] {
                new Card(Card.CardKind.Nine, Card.CardSuit.Diamonds),
                new Card(Card.CardKind.Queen, Card.CardSuit.Clubs),
                new Card(Card.CardKind.Jack, Card.CardSuit.Clubs),
                new Card(Card.CardKind.Eight, Card.CardSuit.Spades),
                new Card(Card.CardKind.Ten, Card.CardSuit.Hearts)});

            var type = h.PokerHandType;
            type.Should().Be(PokerHand.HandType.Straight);
        }

        [Fact]
        public void Flush()
        {
            PokerHand h = new PokerHand(new Card[] {
                new Card(Card.CardKind.Queen, Card.CardSuit.Spades),
                new Card(Card.CardKind.Jack, Card.CardSuit.Spades),
                new Card(Card.CardKind.Ace, Card.CardSuit.Spades),
                new Card(Card.CardKind.Three, Card.CardSuit.Spades),
                new Card(Card.CardKind.Five, Card.CardSuit.Spades)});

            var type = h.PokerHandType;
            type.Should().Be(PokerHand.HandType.Flush);
        }

        [Fact]
        public void StraightFlush()
        {
            PokerHand h = new PokerHand(new Card[] {
                new Card(Card.CardKind.Nine, Card.CardSuit.Diamonds),
                new Card(Card.CardKind.Queen, Card.CardSuit.Diamonds),
                new Card(Card.CardKind.Jack, Card.CardSuit.Diamonds),
                new Card(Card.CardKind.Eight, Card.CardSuit.Diamonds),
                new Card(Card.CardKind.Ten, Card.CardSuit.Diamonds)});

            var type = h.PokerHandType;
            type.Should().Be(PokerHand.HandType.StraightFlush);
        }

        [Fact]
        public void HighCard()
        {
            PokerHand h = new PokerHand(new Card[] {
                new Card(Card.CardKind.Queen, Card.CardSuit.Clubs),
                new Card(Card.CardKind.Jack, Card.CardSuit.Clubs),
                new Card(Card.CardKind.Ten, Card.CardSuit.Hearts),
                new Card(Card.CardKind.Two, Card.CardSuit.Diamonds),
                new Card(Card.CardKind.Five, Card.CardSuit.Spades)});

            var type = h.PokerHandType;
            type.Should().Be(PokerHand.HandType.HighCard);
        }

        [Fact]
        public void HandTypeOrder()
        {
            var types = Enum.GetValues(typeof(PokerHand.HandType));
            types.Should().ContainInOrder(PokerHand.HandType.HighCard,
                PokerHand.HandType.Pair,
                PokerHand.HandType.TwoPair,
                PokerHand.HandType.ThreeOfKind,
                PokerHand.HandType.Straight,
                PokerHand.HandType.Flush,
                PokerHand.HandType.FullHouse,
                PokerHand.HandType.FourOfKind,
                PokerHand.HandType.StraightFlush);
        }
    }
}
