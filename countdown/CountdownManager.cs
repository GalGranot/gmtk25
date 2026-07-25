using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public partial class CountdownManager : Node {
    Main main;
    List<Countdown> countdowns = new();
    GameConfig config;
    [Export] PackedScene[] countdown_scenes;
    [Export] AudioStreamPlayer explosion;
    [Export] AudioStreamPlayer success;
    public event Action<CountdownResult> on_countdown_finished;

    public override void _Ready() {
        config = GD.Load<GameConfig>("res://config/GameConfig.tres");
    }

    public void _Initialize(Main main) {
        this.main = main;
        main.on_card_played += on_card_played;
        spawn_countdowns().Forget();
        spawn_difficulty_spiker().Forget();
    }

    async Task spawn_difficulty_spiker() {
        while(true) {
            await Time.WaitForSeconds(this, 30f);
            config.play_x_cards_required += 3;
        }
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
        int seconds = cd is DontPlaySuit ? config.short_countdown_lifetime_secs : config.default_countdown_lifetime_secs;
        cd._Initialize(random_result(), seconds);
        AddChild(cd);

        switch (cd) {
            case PlayXColouredCards play_x_coloured_cards:
                play_x_coloured_cards._Initialize(
                    seconds: config.default_countdown_lifetime_secs,
                    required_cards: config.play_x_cards_required,
                    colour: EnumUtils.random_enum<CardColour>()
                );
                break;

            case PlayPokerHand play_poker_hand:
                play_poker_hand._Initialize(
                    seconds: config.default_countdown_lifetime_secs,
                    hand: CardUtils.weighted_rand_poker_hand()
                );
                countdowns.Add(cd);
                spawn_ticker(cd).Forget();
                on_card_played2_for_play_poker_hand(main.on_card_played_info(null), play_poker_hand);
                return;
            
            case DontPlaySuit dont_play_suit:
                dont_play_suit._Initialize(
                    seconds: config.short_countdown_lifetime_secs,
                    suit: EnumUtils.random_enum<Suit>()
                );
                break;

            case PlayFaceCards play_face_cards:
                play_face_cards._Initialize(config.play_x_cards_required);
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
            rand_mult = Mathf.Max(rand_mult, 1.5f);
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
            if(!IsInstanceValid(cd)) {
                return;
            }
            cd.tick();
            bool success = false;
            if (cd.ticks >= cd.seconds) {
                if(cd is IOnCountdownExpired on_cd_expired) {
                    CountdownResult result = on_cd_expired.on_countdown_expired();
                    if (result is not CountdownResult.Failed) {
                        on_countdown_finished?.Invoke(result);
                        success = true;
                    }
                }
                int i = countdowns.FindIndex(countdown => countdown == cd);
                finish_countdown_at(i, success);
                return;
            }
        }
    }

    void on_card_played(CardPlayedInfo on_card_played) {
        for (int i = countdowns.Count - 1; i >= 0; i--) {
            Countdown cd = countdowns[i];
            switch (cd) {
                case IOnCardPlayed cd_on_card_played:
                    CountdownResult result = cd_on_card_played.on_card_played(on_card_played);
                    if (result is not CountdownResult.Running) {
                        on_countdown_finished?.Invoke(result);
                        finish_countdown_at(i, true);
                    }
                    break;
            }
        }
    }

    void on_card_played2_for_play_poker_hand(CardPlayedInfo on_card_played, Countdown cd) {
            switch (cd) {
                case IOnCardPlayed cd_on_card_played:
                    CountdownResult result = cd_on_card_played.on_card_played(on_card_played);
                    if (result is not CountdownResult.Running) {
                        on_countdown_finished?.Invoke(result);
                        int i = countdowns.FindIndex(countdown => countdown == cd);
                        if(i == -1) { GD.Print($"not found"); } //! FIXME: rmv
                        GD.PrintErr($"Immediately finishing play poker hand countdown\nWe should later make a delay here"); //! FIXME: rmv
                        finish_countdown_at(i, true);
                        return;
                    }
                    break;
            }
        }

    void finish_countdown_at(int i, bool is_successful) {
        if(is_successful) {
            success.Play(0.5f);
        } else {
            explosion.Play();
        }
        if(i < countdowns.Count && i >= 0) {
            countdowns[i].kill_countdown(is_successful).Forget();
            countdowns.RemoveAt(i);
        }
    }
}
