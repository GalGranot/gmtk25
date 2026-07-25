using Godot;
using System;

public partial class FadeToBlack : CanvasLayer {
	[Export] ColorRect rect;
	[Export] AnimationPlayer anim;

	public static FadeToBlack I;
	public event Action on_finished_fading;

	public override void _Ready() {
		I = this;
		rect.Visible = false;
		anim.AnimationFinished += on_anim_finished;
	}

	void on_anim_finished(StringName anim_name) {
		if (anim_name == "fade_to_black") {
			GD.Print($"here"); //! FIXME: rmv
			on_finished_fading?.Invoke();
			anim.Play("fade_to_black_2");
		} else {
			rect.Visible = false;
		}
	}

	public void transition() {
		rect.Visible = true;
		anim.Play("fade_to_black");
	}
}
