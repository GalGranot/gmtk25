using Godot;

public partial class MainMenu : Node {
	[Export] Button quit;
	[Export] Button start;
	public override void _Ready() {
		start.Pressed += () => GetTree().ChangeSceneToFile("res://main.tscn");
		quit.Pressed += () => GetTree().Quit();
	}
}
