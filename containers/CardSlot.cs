using System.Threading.Tasks;
using Godot;

[GlobalClass]
public partial class CardSlot : Node2D {
    Card card;
    [Export] public string name { get; set; }

    public void insert_into(Card card) {
		card.slot = this;
        this.card = card;
    }

    public Card eject() {
        Card card = this.card;
        this.card = null;
        return card;
    }

    public Card peek() {
        assert_not_null(card);
        return card;
    }

    public Card peek_unchecked() => card;

    public async Task take_and_animate_card(Card card) {
        Tween tween = CreateTween()
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Cubic);
        tween.TweenProperty(card, "position", Position, 1f); //! FIXME: get duration
        await ToSignal(tween, Tween.SignalName.Finished);
    }
}
