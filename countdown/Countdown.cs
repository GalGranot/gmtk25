using Godot;

public partial class Countdown : Node2D {
    [Export] public Label label;

    public int seconds;
    public int ticks = 0;

    GameConfig config;

    public override void _Ready() {
        config = GD.Load<GameConfig>("res://config/GameConfig.tres");
    }

    public override void _PhysicsProcess(double delta) {
        var p = Position;
        p.Y += (float)delta * config.countdowns_move_down_speed;
        Position = p;
    }
}
