public partial class DontPlaySuit : Countdown, IOnCardPlayed, IOnCountdownExpired {
	Suit suit;
	public void _Initialize(int seconds, Suit suit) {
		this.seconds = seconds;
		this.suit = suit;
		update_label();
	}

	void update_label() {
		perk_text.Text = $"Don't play {suit} cards";
	}

    public CountdownResult on_card_played(CardPlayedInfo card_played_info) {
        if(suit == card_played_info.card_played.suit) {
			return new CountdownResult.Failed();
		}
		return new CountdownResult.Running();
    }

    public CountdownResult on_countdown_expired() {
        return result_on_complete;
    }
}
