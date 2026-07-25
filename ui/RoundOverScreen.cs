using System;
using Godot;

public partial class RoundOverScreen : Node {
	[Export] Label label;
	[Export] Button btn;
	public override void _Ready() {
		label.Text = $"Round over!\nScore: {Singleton.score}";
		btn.Pressed += () => GetTree().ChangeSceneToFile("res://main.tscn");
	}
}
