using System;

public class CdPlayCardsOfSameSuit : CountdownBase, IOnCardPlayed {
    readonly int required_cards;
    int played_cards_of_suit = 0;
    Suit suit;
    string text;
    public event Action<string> on_text_change;

    public CdPlayCardsOfSameSuit(int seconds, int required_cards) : base(seconds) {
        this.required_cards = required_cards;
        update_text();
    }

    void update_text() {
        text = $"Play {played_cards_of_suit} out of {required_cards} {suit} cards";
        on_text_change?.Invoke(text);
    }

    public CountdownState on_card_selected(Card card) {
        if(card.suit != suit) {
            return CountdownState.Running;
        }
        played_cards_of_suit += 1;
        update_text();
        assert(played_cards_of_suit <= required_cards);
        if(played_cards_of_suit == required_cards) {
            return CountdownState.Finished;
        }
        return CountdownState.Running;
    }

    public override string text_display() => text;
}