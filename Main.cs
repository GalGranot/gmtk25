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
    [Export] QuestionMark question_mark_icon;
    [Export] PackedScene instructions_scene;
    CanvasLayer instructions;

    /*=============================================================================
    * Events
    =============================================================================*/
    public event Action<int> on_score_change;
    public event Action<CardPlayedInfo> on_card_played;

    /*=============================================================================
    * Misc.
    =============================================================================*/
    [Export] Hud hud;
    [Export] PackedScene card_scene;
    [Export] CountdownManager countdown_manager;

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
        question_mark_icon.on_click += on_question_mark_clicked;
        hud._Initialize(this);

        // Game Initialization
        deck = CardUtils.new_deck();
        playing_slots = [lcard, rcard];
        init_with_back(deck_slot);
        init_with_back(discard_slot);
        discard_slot.peek().ZIndex = 999;
        played.take_and_animate_card(spawn_card(deck.Pop(), deck_slot.Position)).Forget();
        last_played.spawn_worker();

        main().Forget();
    }

    public override void _Process(double delta) {
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
                update_score(score - (playing_slots[0].peek().score + playing_slots[1].peek().score));
                await Task.WhenAll(playing_slots.Map(s => discard_card(s.eject())));
                continue;
            }
            // Player made selection within window
            CardSlot chosen_slot = choose_playing_card_tcs.Task.Result;
            Card chosen_card = chosen_slot.eject();

            update_score(score + chosen_card.score);
            Task old_played_task = Task.CompletedTask;
            if(played.is_occupied) {
                Card old_played = played.eject();
                old_played_task = last_played.take_and_animate_card(old_played);
            }
            played.take_and_animate_card(chosen_card, config.card_move_time_secs).Forget();
            old_played_task.Forget();
            // await Task.WhenAll(
            //     played.take_and_animate_card(chosen_card, config.card_move_time_secs),
            //     old_played_task
            // );
            on_card_played?.Invoke(on_card_played_info(chosen_card));
        }
    }

    async Task discard_card(Card card) {
        await discard_slot.animate_to(card);
        card.QueueFree();
    }

    void update_score(int new_score) {
        score = new_score;
        on_score_change?.Invoke(score);
    }

    void on_countdown_finished(CountdownResult result) {
        switch (result) {
            case CountdownResult.AddScore(int to_add):
                update_score(score + to_add);
                break;
            case CountdownResult.MultScore(float mult_score):
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

    /*=============================================================================
    * UI
    =============================================================================*/
    void on_question_mark_clicked() {
        if (is_paused) {
            instructions.Hide();
            GetTree().Paused = false;
        } else {
            GetTree().Paused = true;
            instructions.Show();
        }
        is_paused = !is_paused;
    }

    public CardPlayedInfo on_card_played_info(Card card) => new CardPlayedInfo {
        card_played = card,
        last_cards_played = last_played.peek(),
    };

    /*=============================================================================
    * Misc
    =============================================================================*/
    void deck_refill() {
        deck = CardUtils.new_deck();
        GD.PrintErr($"Deck empty! maybe do something with it"); //! FIXME: rmv
    }
}

public record CardPlayedInfo {
    public Card card_played;
    public List<Card> last_cards_played;
}
