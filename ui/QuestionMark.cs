using Godot;
using System;

public partial class QuestionMark : Node2D {
	[Export] Area2D area;
	public event Action on_click;
	public override void _Ready() {
		area.InputEvent += on_area_clicked;
	}

	void on_area_clicked(Node viewport, InputEvent @event, long shape_idx) {
        if (@event is InputEventMouseButton mouse &&
            mouse.ButtonIndex == MouseButton.Left &&
            mouse.Pressed
        ) {
			GD.Print("Question mark clicked");
            on_click?.Invoke();
        }
    }
}
