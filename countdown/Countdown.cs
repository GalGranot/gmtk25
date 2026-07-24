using System.Threading.Tasks;
using Godot;

public partial class Countdown : Node2D {
    [Export] public Label perk_text;
    [Export] public Label cd_text;

    public int seconds;
    public int ticks = 0;
    public CountdownResult result_on_complete;

    GameConfig config;

    public override void _Ready() {
        config = GD.Load<GameConfig>("res://config/GameConfig.tres");
    }

    public void _Initialize(CountdownResult result, int seconds) {
        this.seconds = seconds;
        this.result_on_complete = result;

        var container = new HBoxContainer {
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
            SizeFlagsVertical = Control.SizeFlags.ExpandFill
        };
        AddChild(container);
        perk_text.Reparent(container);
        cd_text.Reparent(container);

        container.AddThemeConstantOverride("separation", 5);
        update_cd_text();
        prettify_label(perk_text);
        prettify_label(cd_text);

        Tween color_modulate = CreateTween();
        color_modulate.TweenProperty(this, "modulate", Colors.Red, 5f);
    }

    void prettify_label(Label label) {
        label.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        label.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
        label.AddThemeStyleboxOverride("normal", new StyleBoxFlat {
            BgColor = Colors.SeaGreen,
            BorderColor = new Color(0.4f, 0.4f, 0.4f),
            BorderWidthBottom = 4,
            BorderWidthLeft = 4,
            BorderWidthRight = 4,
            BorderWidthTop = 4,
            CornerRadiusBottomLeft = 8,
            CornerRadiusBottomRight = 8,
            CornerRadiusTopLeft = 8,
            CornerRadiusTopRight = 8,
            ContentMarginLeft = 12,
            ContentMarginRight = 12,
            ContentMarginTop = 8,
            ContentMarginBottom = 8,
        });
        label.AddThemeFontSizeOverride("font_size", 40);
    }

    public override void _PhysicsProcess(double delta) {
        var p = Position;
        p.Y += (float)delta * config.countdowns_move_down_speed;
        Position = p;
    }

    public async Task kill_countdown(bool is_successful) {
        Vector2 scatter_direction = new Vector2(
            Random.float_in_range(-50f, 50f),
            Random.float_in_range(-80f, -20f)
        );
        Tween tween = CreateTween().SetParallel(true);

        tween.TweenProperty(this, "scale", Scale * 1.7f, 0.3f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Back);

        tween.TweenProperty(this, "position", Position + scatter_direction, 0.4f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Quad);
            
        Modulate = is_successful ? Colors.Green : Colors.Red;
        Color transparent_colour = Modulate;
        
        transparent_colour.A = 0f;
        tween.TweenProperty(this, "modulate", transparent_colour, 1)
            .SetDelay(0.1f);
        tween.Chain().TweenCallback(Callable.From(QueueFree));
    }

    public void tick() {
        ticks += 1;
        update_cd_text();
    }

    void update_cd_text() {
        cd_text.Text = $"{seconds - ticks}...";
    }
}
