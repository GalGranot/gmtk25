using Godot;

public partial class Countdown : Node2D {
    [Export] public Label label;

    public int seconds;
    public int ticks = 0;

    public override void _PhysicsProcess(double delta) {
        var p = Position;
        p.Y += (float)delta * 100f; //! FIXME: Get from config
        Position = p;
    }
}