using System.Linq;
using System.Text.RegularExpressions;

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
        if(card_played_info.last_cards_played.Any(card => card is null)) {
            return new CountdownResult.Running();
        }
        if(CardUtils.hand_fits(
            CardUtils.assign_hand(card_played_info.last_cards_played),
            hand
        )) {
            return result_on_complete;
        }
        return new CountdownResult.Running();
    }
}
