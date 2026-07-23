using Godot;

public partial class Countdown : Node2D {
    [Export] public Label text;
    public CountdownBase countdown;
    public CountdownState tick() => countdown.tick();

    public void _Initialize(CountdownBase countdown) {
        this.countdown = countdown;
        this.countdown.on_text_change
    }

    void update_label(string text) => this.text.Text = text;
}
