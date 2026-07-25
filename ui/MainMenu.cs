using Godot;

public partial class MainMenu : Node {
	[Export] Button quit;
	[Export] Button start;
	public override void _Ready() {
		start.Pressed += () => {
			FadeToBlack.I.on_finished_fading += on_finished_fading;
			FadeToBlack.I.transition();
		};
		quit.Pressed += () => GetTree().Quit();
	}

	void on_finished_fading() {
		FadeToBlack.I.on_finished_fading -= on_finished_fading;
		GetTree().ChangeSceneToFile("res://Main.tscn");
	}
}
