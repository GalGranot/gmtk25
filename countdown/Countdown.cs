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
    }

    public override void _PhysicsProcess(double delta) {
        var p = Position;
        p.Y += (float)delta * config.countdowns_move_down_speed;
        Position = p;
    }
}
