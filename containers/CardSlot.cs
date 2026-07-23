using System.Threading.Tasks;
using Godot;

[GlobalClass]
public partial class CardSlot : Node2D {
    Card card;
    [Export] public string name { get; set; }
    float default_move_duration;
    GameConfig config;

    public override void _Ready() {
        config = GD.Load<GameConfig>("res://config/GameConfig.tres");
        default_move_duration = config.card_move_time_secs;
    }

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

    public async Task take_and_animate_card(Card card) =>
        await take_and_animate_card(card, default_move_duration);
    public async Task take_and_animate_card(Card card, float duration) {
        insert_into(card);
        await animate_to(card, duration);
    }

    public async Task animate_to(Card card) => await animate_to(card, default_move_duration);
    public async Task animate_to(Card card, float duration) {
        Tween tween = CreateTween()
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Cubic);
        tween.TweenProperty(card, "position", Position, duration);
        await ToSignal(tween, Tween.SignalName.Finished);
    }

    public bool is_occupied => card != null;
}
