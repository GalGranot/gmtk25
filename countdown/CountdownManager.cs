using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public partial class CountdownManager : Node {
    Main main;
    List<Countdown> countdowns = new();
    GameConfig config;
    [Export] PackedScene[] countdown_scenes;
    public event Action<CountdownResult> on_countdown_finished;

    public override void _Ready() {
        config = GD.Load<GameConfig>("res://config/GameConfig.tres");
    }

    public void _Initialize(Main main) {
        this.main = main;
        main.on_card_played += on_card_played;
        spawn_countdowns().Forget();
    }

    async Task spawn_countdowns() {
        while (true) {
            spawn_countdown();
            await Time.WaitForSeconds(this, config.countdown_spawn_delay_secs);
        }
    }

    void spawn_countdown() {
        PackedScene scene = countdown_scenes.RandomElement();
        Countdown cd = scene.Instantiate<Countdown>();
        Vector2 pos = cd.Position;
        pos.X += 20;
        cd.Position = pos;
        cd._Initialize(random_result(), config.default_countdown_secs);
        AddChild(cd);

        switch (cd) {
            case PlayXColouredCards play_x_coloured_cards:
                play_x_coloured_cards._Initialize(
                    seconds: config.default_countdown_lifetime_secs,
                    required_cards: config.play_x_coloured_cards_required,
                    colour: EnumUtils.random_enum<CardColour>()
                );
                break;

            default:
                throw die_throw();
        }
        countdowns.Add(cd);
        spawn_ticker(cd).Forget();
    }

    //! FIXME: Hardcoded currently:
    //! 30% Mult and 70% Add.
    //! x0.5 < mult < x5
    //! -0.1 * score < to add < 2 * score
    //! FIXME: Make this come from config
    CountdownResult random_result() {
        int randi = Random.int_in_range(10);
        if (randi < 3) {
            float rand_mult = Random.float_in_range(0.5f, 5f);
            rand_mult = (float)Math.Round(rand_mult, 1);
            rand_mult = ((float)Math.Round(rand_mult * 2)) / 2;
            return new CountdownResult.MultScore(rand_mult);
        } else {
            int score = main.score;
            int to_add = score < 20 ? score + 10 : Random.int_in_range(-(int)(score * 0.1f), score * 2);
            to_add = Mathf.Max(to_add, 10);
            return new CountdownResult.AddScore(to_add);
        }
    }

    async Task spawn_ticker(Countdown cd) {
        while (true) {
            await Time.WaitForSeconds(this, 1f);
            cd.tick();
            if (cd.ticks >= cd.seconds) {
                int i = countdowns.FindIndex(countdown => countdown == cd);
                finish_countdown_at(i);
                return;
            }
        }
    }

    void on_card_played(Card card) {
        for (int i = countdowns.Count - 1; i >= 0; i--) {
            Countdown cd = countdowns[i];
            switch (cd) {
                case IOnCardPlayed cd_on_card_played:
                    CountdownResult result = cd_on_card_played.on_card_played(card);
                    if (result is not CountdownResult.Running) {
                        on_countdown_finished?.Invoke(result);
                        finish_countdown_at(i);
                    }
                    break;
            }
        }
    }

    void finish_countdown_at(int i) {
        countdowns[i].QueueFree();
        countdowns.RemoveAt(i);
    }
}
