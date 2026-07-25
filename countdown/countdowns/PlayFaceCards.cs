using Godot;
using System;

public partial class PlayFaceCards : Countdown, IOnCardPlayed {
	int played = 0;
	int required;

	public void _Initialize(int required) {
		this.required = required;
		update_label();
	}

	void update_label() {
		perk_text.Text = $"Play {played}/{required} face cards";
	}

    public CountdownResult on_card_played(CardPlayedInfo card_played_info) {
        Card card = card_played_info.card_played;
		if(card.rank.is_face_card()) {
			played += 1;
			update_label();
			if(played >= required) {
				return result_on_complete;
			}
		}
		return new CountdownResult.Running();
    }
}
