using Godot;

public partial class Countdown : Node2D {
    [Export] public Label label;

    public int seconds;
    public int ticks = 0;
    public CountdownResult result_on_complete;

    GameConfig config;

    public override void _Ready() {
        config = GD.Load<GameConfig>("res://config/GameConfig.tres");
    }

    public void _Initialize(CountdownResult result) {
        this.result_on_complete = result;
        prettify_label();
    }

    void prettify_label() {
        label.AddThemeStyleboxOverride("normal", new StyleBoxFlat {
            BgColor = Colors.SeaGreen,
            BorderColor = new Color(0.4f, 0.4f, 0.4f),
            BorderWidthBottom = 4,
            BorderWidthLeft = 4,
            BorderWidthRight = 4,
            BorderWidthTop = 4,
            CornerRadiusBottomLeft = 8,
            CornerRadiusBottomRight = 8,
            CornerRadiusTopLeft = 8,
            CornerRadiusTopRight = 8,
            ContentMarginLeft = 12,
            ContentMarginRight = 12,
            ContentMarginTop = 8,
            ContentMarginBottom = 8,
        });
    }

    public override void _PhysicsProcess(double delta) {
        var p = Position;
        p.Y += (float)delta * config.countdowns_move_down_speed;
        Position = p;
    }
}
