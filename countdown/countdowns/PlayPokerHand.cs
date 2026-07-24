using System.Linq;
using System.Text.RegularExpressions;
using Godot;

public partial class PlayPokerHand : Countdown, IOnCardPlayed {
    PokerHand hand;

    public void _Initialize(int seconds, PokerHand hand) {
        this.seconds = seconds;
        this.hand = hand;
        update_label();
    }

    void update_label() {
        string name = Regex.Replace(hand.ToString(), @"(?<!^)(?=[A-Z])", " ");
        perk_text.Text = $"Play a {name}{result_on_complete.name}";
    }

    public CountdownResult on_card_played(CardPlayedInfo card_played_info) {
        if (CardUtils.hand_fits(
            hand,
            CardUtils.assign_hand(card_played_info.last_cards_played)
        )) {
            return result_on_complete;
        }
        return new CountdownResult.Running();
    }
}
