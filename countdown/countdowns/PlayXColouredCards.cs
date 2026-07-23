using Godot;

public partial class PlayXColouredCards : Countdown, IOnCardPlayed {
    CardColour colour;
    int required_cards;
    int played_cards = 0;

    public void _Initialize(int seconds, int required_cards, CardColour colour) {
        this.seconds = seconds;
        this.required_cards = required_cards;
        this.colour = colour;
        update_label();
    }

    void update_label() {
        label.Text = $"Play {played_cards}/{required_cards} {colour} cards{result_on_complete.name}";
    }

    public CountdownResult on_card_played(Card card) {
        if (card.colour == colour) {
            played_cards += 1;
            update_label();
            if (played_cards >= required_cards) {
                return result_on_complete;
            }
        }
        return new CountdownResult.Running();
    }
}
