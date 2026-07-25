using System.Threading;
using System.Threading.Tasks;
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

    public async Task vibrate_score(bool success, float duration = 0.3f, float scale_amount = 1.2f) {
        var orig_colour = score_text.Modulate;
        var orig_scale = score_text.Scale;

        var tween = CreateTween();
        tween.SetTrans(Tween.TransitionType.Quad);
        tween.SetEase(Tween.EaseType.Out);

        Color tgt_colour = success ? Colors.Lime : Colors.DarkRed;

        tween.Parallel()
            .TweenProperty(score_text, "modulate", tgt_colour, duration * 0.5f);
        tween.Parallel()
            .TweenProperty(score_text, "scale", orig_scale * scale_amount, duration * 0.5f);

        tween.TweenProperty(score_text, "scale", orig_scale, duration * 0.5f);
        tween.TweenProperty(score_text, "modulate", orig_colour, duration * 0.3f);
    }
}
