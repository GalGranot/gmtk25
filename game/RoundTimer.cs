using System;
using Godot;

public partial class RoundTimer : Node {
    [Export] Timer timer;
    [Export] Label label;
    GameConfig config;
    int secs_remaining;
    public event Action on_round_end;

    public override void _Ready() {
        config = GD.Load<GameConfig>("res://config/GameConfig.tres");
    }

    public void _Initialize(int round_len) {
        secs_remaining = round_len;
        timer.WaitTime = 1f;
        timer.Timeout += on_timer_timeout;
        timer.Start();
    }

    void on_timer_timeout() {
        secs_remaining -= 1;
        update_display();
        if(secs_remaining <= 0) { on_round_end?.Invoke(); } 
    }

    void update_display() {
        int mins = secs_remaining / 60;
        int secs = secs_remaining % 60;
        label.Text = $"{mins:D2}:{secs:D2}";
    }
}
