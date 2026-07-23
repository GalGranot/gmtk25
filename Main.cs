using System.Collections.Generic;
using Godot;

public partial class Main : Node {
    /*=============================================================================
    * Slots
    =============================================================================*/
    [Export] CardSlot deck_slot;
    [Export] CardSlot lcard;
    [Export] CardSlot rcard;
    [Export] CardSlot played;


    /*=============================================================================
    * Containers
    =============================================================================*/
    List<CardId> deck;

    [Export] PackedScene card_scene;

    public override void _Ready() {
        deck = CardUtils.new_deck();
    }

    Card spawn_card(CardId id, Vector2 at) {
        Card card = card_scene.Instantiate<Card>();
        card.Position = at;
        card._Initialize(id);
        AddChild(card);
        return card;
    }
}
