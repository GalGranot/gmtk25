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
    * Events
    =============================================================================*/
    public event Action<int> on_score_change;

    /*=============================================================================
    * Misc.
    =============================================================================*/
    [Export] PackedScene card_scene;
    [Export] Hud hud;
    GameConfig config;
    int score;

    /*=============================================================================
    * Godot Callbacks
    =============================================================================*/
    public override void _Ready() {
        hud._Initialize(this);
        config = GD.Load<GameConfig>("res://config/GameConfig.tres");
        deck = CardUtils.new_deck();
        playing_slots = [lcard, rcard];

        init_with_back(deck_slot);
        init_with_back(discard_slot);
        discard_slot.peek().ZIndex = 999;

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
            Task deal_playing_cards_task = deal_playing_cards();
            Task card_choice = choose_playing_card_tcs.Task;
            Task delay = Time.WaitForSeconds(this, 3);

            Task winner = await Task.WhenAny(card_choice, delay);
            if(winner == delay) {
                GD.Print("Too slow!");
            } else {

                CardSlot chosen_slot = choose_playing_card_tcs.Task.Result;
                Card chosen_card = chosen_slot.eject();
                update_score(chosen_card.score);
                CardSlot other = chosen_slot == lcard ? rcard : lcard;
                Func<Task> kill_other_card = async () => {
                    Card to_kill = other.eject();
                    await discard_slot.animate_to(to_kill);
                    to_kill.QueueFree();
                };
                await Task.WhenAll(
                    played.take_and_animate_card(chosen_card),
                    kill_other_card()
                );
            }
        }
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
        await Task.WhenAll(
            playing_slots.Map(s => s.take_and_animate_card(spawn_card(deck.Pop(), deck_slot.Position)))
        );
    }
}
