using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public partial class Main : Node {
    /*=============================================================================
    * Slots
    =============================================================================*/
    [Export] CardSlot deck_slot;
    [Export] CardSlot lcard;
    [Export] CardSlot rcard;
    [Export] CardSlot played;
    [Export] CardSlot discard_slot;
    
    [Export] LastPlayed last_played;

    CardSlot[] playing_slots;
    /*=============================================================================
    * Containers
    =============================================================================*/
    List<CardId> deck;

    /*=============================================================================
    * Task Completion Sources
    =============================================================================*/
    TaskCompletionSource<CardSlot> choose_playing_card_tcs;

    /*=============================================================================
    * UI
    =============================================================================*/
    [Export] PackedScene instructions_scene;
    [Export] RoundTimer round_timer;
    CanvasLayer instructions;

    /*=============================================================================
    * Events
    =============================================================================*/
    public event Action<int> on_score_change;
    public event Action<CardPlayedInfo> on_card_played;

    /*=============================================================================
    * Sound
    =============================================================================*/
    AudioStreamPlayer card_move_sound = new();
    AudioStreamPlayer shuffle_sound = new();

    /*=============================================================================
    * Misc.
    =============================================================================*/
    [Export] Hud hud;
    [Export] PackedScene card_scene;
    [Export] CountdownManager countdown_manager;
    [Export] PackedScene round_over_screen;

    GameConfig config;
    bool is_paused = false;
    public int score { get; private set; }

    /*=============================================================================
    * Godot Callbacks
    =============================================================================*/
    public override void _Ready() {
        config = GD.Load<GameConfig>("res://config/GameConfig.tres");

        // Instructions Initialization
        instructions = instructions_scene.Instantiate<CanvasLayer>();
        AddChild(instructions);
        instructions.Hide();

        // Countdown Initialization
        countdown_manager._Initialize(this);
        countdown_manager.on_countdown_finished += on_countdown_finished;

        // UI Initialization
        hud._Initialize(this);
        round_timer._Initialize(config.round_len_secs);
        round_timer.on_round_end += on_round_end;

        // Sound Initialization
        card_move_sound.Stream = ResourceLoader.Load<AudioStream>("res://sound/card-take.mp3");
        shuffle_sound.Stream = ResourceLoader.Load<AudioStream>("res://sound/shuffle.mp3");
        AddChild(card_move_sound);
        AddChild(shuffle_sound);

        // Game Initialization
        deck = CardUtils.new_deck();
        shuffle_sound.Play();
        playing_slots = [lcard, rcard];
        init_with_back(deck_slot);
        init_with_back(discard_slot);
        discard_slot.peek().ZIndex = 999;
        played.take_and_animate_card(spawn_card(deck.Pop(), deck_slot.Position)).Forget();
        card_move_sound.Play();
        last_played.spawn_worker();

        main().Forget();
    }

    public override void _Process(double delta) {
        if(Input.IsActionJustPressed(config.escape)) {
            goto_main_menu();
        }
        if (Input.IsActionJustPressed(config.move_left)) {
            choose_playing_card_tcs.TrySetResult(lcard);
        }
        if (Input.IsActionJustPressed(config.move_right)) {
            choose_playing_card_tcs.TrySetResult(rcard);
        }
        if (!played.is_occupied) { return; }
        Card card = played.peek();
        if(Input.IsActionJustPressed(config.choose_up)) {
            card.up();
        }
        if(Input.IsActionJustPressed(config.choose_down)) {
            card.down();
        }
        if(Input.IsActionJustPressed(config.spacebar)) {
            card.change_suit();
        }
    }

    /*=============================================================================
    * Init
    =============================================================================*/
    void init_with_back(CardSlot slot) {
        Card card = spawn_card(CardId.random(), slot.Position);
        card.show_back();
        slot.insert_into(card);
    }

    /*=============================================================================
    * Game
    =============================================================================*/
    async Task main() {
        while (true) {
            choose_playing_card_tcs = new();
            deal_playing_cards().Forget();
            Task card_choice = choose_playing_card_tcs.Task;
            Task choice_window = Time.WaitForSeconds(this, config.choice_window_time_secs);

            Task winner = await Task.WhenAny(card_choice, choice_window);
            if (winner == choice_window) {
                GD.Print("Too slow!");
                await Task.WhenAll(playing_slots.Map(s => discard_card(s.eject())));
                continue;
            }
            // Player made selection within window
            CardSlot chosen_slot = choose_playing_card_tcs.Task.Result;
            Card chosen_card = chosen_slot.eject();

            Task old_played_task = Task.CompletedTask;
            Card old_played = null;
            if(played.is_occupied) {
                old_played = played.eject();
                old_played_task = last_played.take_and_animate_card(old_played);
                card_move_sound.Play();
            }
            played.take_and_animate_card(chosen_card, config.card_move_time_secs).Forget();
            card_move_sound.Play();
            old_played_task.Forget();
            on_card_played?.Invoke(on_card_played_info(old_played));
        }
    }

    async Task discard_card(Card card) {
        await discard_slot.animate_to(card);
        card.QueueFree();
    }

    void update_score(int new_score) {
        score = Math.Max(0, new_score);
        on_score_change?.Invoke(score);
    }

    void on_countdown_finished(CountdownResult result) {
        bool success = result.success;
        switch (result) {
            case CountdownResult.AddScore(_, int to_add):
                to_add *= success ? 1 : -1;
                update_score(score + to_add);
                break;
            case CountdownResult.MultScore(_, float mult_score):
                if(!success) { mult_score = 1 / mult_score; }
                update_score((int)(score * mult_score));
                break;
        }
    }

    /*=============================================================================
    * Cards
    =============================================================================*/
    Card spawn_card(CardId id, Vector2 at) {
        Card card = card_scene.Instantiate<Card>();
        card.Position = at;
        card._Initialize(id);
        AddChild(card);
        return card;
    }

    async Task deal_playing_cards() {
        async Task deal(CardSlot slot) {
            CardId id;
            while(true) {
                try {
                    id = deck.Pop();
                    break;
                } catch(Exception) {
                    deck_refill();
                }
            }
            if (!slot.is_occupied) {
                try {
                    card_move_sound.Play();
                    await slot.take_and_animate_card(
                        spawn_card(deck.Pop(), deck_slot.Position),
                        config.deal_time_secs
                    );
                } catch(Exception) {
                    deck_refill();
                }
            }
        }
        await Task.WhenAll(playing_slots.Map(deal));
    }

    void on_round_end() {
        Singleton.score = score;
        FadeToBlack.I.on_finished_fading += on_finished_fading_round_end;
        FadeToBlack.I.transition();
    }

    void on_finished_fading_round_end() {
        FadeToBlack.I.on_finished_fading -= on_finished_fading_round_end;
        GetTree().ChangeSceneToFile("res://ui/RoundOverScreen.tscn");
    }

    void goto_main_menu() {
        FadeToBlack.I.on_finished_fading += on_finished_fading_goto_main_menu;
        FadeToBlack.I.transition();
    }

    void on_finished_fading_goto_main_menu() {
        FadeToBlack.I.on_finished_fading -= on_finished_fading_goto_main_menu;
        GetTree().ChangeSceneToFile("res://ui/MainMenu.tscn");
    }

    /*=============================================================================
    * UI
    =============================================================================*/
    public CardPlayedInfo on_card_played_info(Card card) => new CardPlayedInfo {
        card_played = card,
        last_cards_played = last_played.peek(),
    };

    /*=============================================================================
    * Misc
    =============================================================================*/
    void deck_refill() {
        shuffle_sound.Play();
        deck = CardUtils.new_deck();
        GD.PrintErr($"Deck empty! maybe do something with it"); //! FIXME: rmv
    }
}

public record CardPlayedInfo {
    public Card card_played;
    public List<Card> last_cards_played;
}
