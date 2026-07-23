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
    public event Action<Card> on_card_played;

    /*=============================================================================
    * Misc.
    =============================================================================*/
    [Export] Hud hud;
    [Export] PackedScene card_scene;
    [Export] CountdownManager countdown_manager;

    GameConfig config;
    bool is_paused = false;
    int score;

    /*=============================================================================
    * Godot Callbacks
    =============================================================================*/
    public override void _Ready() {
        instructions = instructions_scene.Instantiate<CanvasLayer>();
        AddChild(instructions);
        instructions.Hide();
        countdown_manager._Initialize(this);

        question_mark_icon.on_click += on_question_mark_clicked;

        hud._Initialize(this);
        config = GD.Load<GameConfig>("res://config/GameConfig.tres");
        deck = CardUtils.new_deck();
        playing_slots = [lcard, rcard];

        init_with_back(deck_slot);
        init_with_back(discard_slot);
        discard_slot.peek().ZIndex = 999;

        played.take_and_animate_card(spawn_card(deck.Pop(), deck_slot.Position));

        main();
    }

    public override void _Process(double delta) {
        if(Input.IsActionPressed(config.move_left)) {
            choose_playing_card_tcs.TrySetResult(lcard);
        } else if(Input.IsActionPressed(config.move_right)) {
            choose_playing_card_tcs.TrySetResult(rcard);
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
        while(true) {
            choose_playing_card_tcs = new();
            await deal_playing_cards();
            Task card_choice = choose_playing_card_tcs.Task;
            Task choice_window = Time.WaitForSeconds(this, config.choice_window_time);

            Task winner = await Task.WhenAny(card_choice, choice_window);
            if(winner == choice_window) {
                GD.Print("Too slow!");
                //! FIXME: Accum fn?
                update_score(-(playing_slots[0].peek().score + playing_slots[1].peek().score));
                await Task.WhenAll(playing_slots.Map(s => discard_card(s.eject())));
                continue;
            }
            CardSlot chosen_slot = choose_playing_card_tcs.Task.Result;
            Card chosen_card = chosen_slot.eject();
            on_card_played?.Invoke(chosen_card);

            update_score(chosen_card.score);
            Card old_played = played.eject();
            await played.take_and_animate_card(chosen_card, config.card_move_time);
            old_played.QueueFree();
        }
    }

    async Task discard_card(Card card) {
        await discard_slot.animate_to(card);
        card.QueueFree();
    }

    void update_score(int to_add) {
        score += to_add;
        on_score_change?.Invoke(score);
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
            if(!slot.is_occupied) {
                await slot.take_and_animate_card(
                    spawn_card(deck.Pop(), deck_slot.Position),
                    config.deal_time
                );
            }
        }
        await Task.WhenAll(playing_slots.Map(deal));
    }

    /*=============================================================================
    * UI
    =============================================================================*/
    void on_question_mark_clicked() {
        if(is_paused) {
            instructions.Hide();
            GetTree().Paused = false;
        } else {
            GetTree().Paused = true;
            instructions.Show();
        }
        is_paused = !is_paused;
    }
}
