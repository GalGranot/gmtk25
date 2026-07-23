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
    * Misc.
    =============================================================================*/
    [Export] PackedScene card_scene;
    GameConfig config;

    public override void _Ready() {
        config = GD.Load<GameConfig>("res://config/GameConfig.tres");
        deck = CardUtils.new_deck();
        playing_slots = [lcard, rcard];
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
                CardSlot chosen = choose_playing_card_tcs.Task.Result;
                CardSlot other = chosen == lcard ? rcard : lcard;
                Func<Task> kill_other_card = async () => {
                    Card to_kill = other.eject();
                    await discard_slot.animate_to(to_kill);
                    to_kill.QueueFree();
                };
                await Task.WhenAll(
                    played.take_and_animate_card(chosen.eject()),
                    kill_other_card()
                );
            }
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
        await Task.WhenAll(
            playing_slots.Map(s => s.take_and_animate_card(spawn_card(deck.Pop(), deck_slot.Position)))
        );
    }
}
