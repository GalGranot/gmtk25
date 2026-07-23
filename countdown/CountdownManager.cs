using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public partial class CountdownManager : Node {
    Main main;
    List<Countdown> countdowns = new();
    GameConfig config;
    [Export] PackedScene[] countdown_scenes;

    public override void _Ready() {
        config = GD.Load<GameConfig>("res://config/GameConfig.tres");
    }

    public void _Initialize(Main main) {
        this.main = main;
        main.on_card_played += on_card_played;
        tick().Forget();
        spawn_countdowns().Forget();
    }

    async Task spawn_countdowns() {
        while (true) {
            await Time.WaitForSeconds(this, config.countdown_spawn_delay_secs);
            spawn_countdown();
        }
    }

    void spawn_countdown() {
        PackedScene scene = countdown_scenes.RandomElement();
        Countdown cd = scene.Instantiate<Countdown>();
        AddChild(cd);
        countdowns.Add(cd);

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
    }

    async Task tick() {
        while (true) {
            await Time.WaitForSeconds(this, 5f);
            for (int i = countdowns.Count - 1; i >= 0; i--) {
                Countdown cd = countdowns[i];
                cd.ticks += 1;
                if (cd.ticks >= cd.seconds) {
                    delete_countdown_at(i);
                }
            }
        }
    }

    void on_card_played(Card card) {
        for (int i = countdowns.Count - 1; i >= 0; i--) {
            Countdown cd = countdowns[i];
            switch (cd) {
                case IOnCardPlayed cd_on_card_played:
                    if (cd_on_card_played.on_card_played(card) is CountdownState.Finished) {
                        delete_countdown_at(i);
                    }
                    break;
            }
        }
    }

    void delete_countdown_at(int i) {
        countdowns[i].QueueFree();
        countdowns.RemoveAt(i);
    }
}
