using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class CardUtils {
    public static List<CardId> new_deck() {
        var ranks = Enum.GetValues(typeof(Rank));
        var suits = Enum.GetValues(typeof(Suit));
        List<CardId> deck = new(ranks.Length * suits.Length);
        foreach (Rank rank in ranks) {
            foreach (Suit suit in suits) {
                deck.Add(new(rank, suit));

                //! Uncomment for all cards the same
                // deck.Add(new(Rank.Ace, Suit.Clubs));
            }
        }
        Random.randomize_list(deck);
        return deck;
    }

    static public bool hand_fits(PokerHand required, PokerHand present) {
        GD.Print($"required = {required}, present = {present}"); //! FIXME: rmv
        if (required is PokerHand.None) {
            return false;
        }

        return required switch {
            PokerHand.Pair => present is PokerHand.Pair or PokerHand.TwoPair or PokerHand.ThreeOfAKind or PokerHand.FullHouse or PokerHand.FourOfAKind,
            PokerHand.TwoPair => present is PokerHand.TwoPair or PokerHand.ThreeOfAKind or PokerHand.FullHouse or PokerHand.FourOfAKind,
            PokerHand.ThreeOfAKind => present is PokerHand.ThreeOfAKind or PokerHand.FullHouse or PokerHand.FourOfAKind,
            PokerHand.Straight => present is PokerHand.Straight or PokerHand.StraightFlush,
            PokerHand.Flush => present is PokerHand.Flush or PokerHand.StraightFlush,
            PokerHand.FullHouse => present is PokerHand.FullHouse,
            PokerHand.FourOfAKind => present is PokerHand.FourOfAKind,
            PokerHand.StraightFlush => present is PokerHand.StraightFlush,
            _ => false,
        };
    }

    public static PokerHand assign_hand(List<Card> cards) {
        assert(cards.Count() == 5);
        bool flush = false;
        bool straight = false;
        if(!cards.Any(c => c is null)) {
            flush = is_flush(cards);
            straight = is_straight(cards);
        }
        List<int> freqs = count_freqs(cards);
        string msg = $"flush = {flush}, straight = {straight}, freqs = ";
        foreach(int i in freqs) { msg += $"{i}, "; }
        GD.Print(msg); //! FIXME: rmv

        return (flush, straight, freqs) switch {
            (true, true, _) => PokerHand.StraightFlush,
            (_, _, [4, 1, ..]) => PokerHand.FourOfAKind,
            (_, _, [3, 2, ..]) => PokerHand.FullHouse,
            (true, _, _) => PokerHand.Flush,
            (_, true, _) => PokerHand.Straight,
            (_, _, [3, ..]) => PokerHand.ThreeOfAKind,
            (_, _, [2, 2, ..]) => PokerHand.TwoPair,
            (_, _, [2, ..]) => PokerHand.Pair,
            _ => PokerHand.None,
        };
    }

    static bool is_flush(List<Card> cards) => cards.All(c => c.suit == cards[0].suit);
    static bool is_straight(List<Card> cards) {
        cards.Sort((c1, c2) => c1.rank - c2.rank);
        List<Rank> ranks = cards.Map(c => c.rank);
        if (ranks == new List<Rank> {
            Rank.Ace, Rank.Two, Rank.Three, Rank.Four, Rank.Five,
        } || ranks == new List<Rank> {
            Rank.Ace, Rank.Ten, Rank.Jack, Rank.Queen, Rank.King,
        }) {
            return true;   
        }
        for(int i = 1; i < 5; i++) {
            if (ranks[i] - 1 != ranks[i - 1]) {
                return false;
            }
        }
        return true;
    }

    static List<int> count_freqs(List<Card> cards) {
        return cards
            .Where(c => c is not null)
            .GroupBy(c => c.rank)
            .Select(g => g.Count())
            .OrderByDescending(x => x)
            .ToList();
    }
}

public enum PokerHand {
    None,
    Pair,
    TwoPair,
    ThreeOfAKind,
    Straight,
    Flush,
    FullHouse,
    FourOfAKind,
    StraightFlush,
}
