using Godot;

public partial class MainMenu : Control {
	[Export] Button quit;
	[Export] Button start;
	public override void _Ready() {
		start.Pressed += () => GetTree().ChangeSceneToFile("res://main.tscn");
		quit.Pressed += () => GetTree().Quit();
	}
}
