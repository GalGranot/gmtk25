using Godot;

public partial class Hud : CanvasLayer {
	[Export] Label score_text;
	public override void _Ready() {
		update_score(0);
	}

	public void _Initialize(Main main) {
		main.on_score_change += update_score;
	}

	void update_score(int score) {
		score_text.Text = $"SCORE: {score}";
	}
}
