using System;
using Godot;

public partial class RoundOverScreen : Node {
	[Export] Label label;
	[Export] Button btn;
	public override void _Ready() {
		label.Text = $"Round over!\nScore: {Singleton.score}";
		btn.Pressed += async () => {
			FadeToBlack.I.on_finished_fading += after_fading;
			FadeToBlack.I.transition();
		};
	}

	void after_fading() {
		GetTree().ChangeSceneToFile("res://main.tscn");
	}
}
