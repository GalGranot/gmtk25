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

    CardSlot[] playing_slots;
    /*=============================================================================
    * Containers
    =============================================================================*/
    List<CardId> deck;

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
        if(Input.IsActionPressed("choose_left")) {
            GD.Print($"move_left"); //! FIXME: rmv
        } else if(Input.IsActionPressed("choose_right")) {
            GD.Print($"move_right"); //! FIXME: rmv       
        }
    }

    /*=============================================================================
    * Game
    =============================================================================*/
    async Task main() {
        while(true) {
            Task deal_playing_cards_task = deal_playing_cards();
            await Task.Delay(1000);
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
