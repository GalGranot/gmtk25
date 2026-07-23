using System;
using System.Collections.Generic;

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
}
