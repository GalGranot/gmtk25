using Godot;

public partial class PlayXColouredCards : Countdown, IOnCardPlayed {
    CardColour colour;
    int required_cards;
    int played_cards = 0;

    public void _Initialize(int seconds, int required_cards, CardColour colour)  {
        this.seconds = seconds;
        this.required_cards = required_cards;
        this.colour = colour;
        update_label();
    }

    void update_label() {
        GD.Print($"in update label"); //! FIXME: rmv
        label.Text = $"Play {played_cards}/{required_cards} {colour} cards";
    }

    public CountdownState on_card_played(Card card) {
        if (card.colour == colour) {
            played_cards += 1;
            update_label();
            if (played_cards >= required_cards) {
                return CountdownState.Finished;
            }
        }
        return CountdownState.Running;
    }
}