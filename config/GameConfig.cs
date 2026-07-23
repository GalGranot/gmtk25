using Godot;

[GlobalClass]
public partial class GameConfig : Resource {
    /*=============================================================================
    * Movement Times
    =============================================================================*/
    [Export] public float card_move_time = 0.2f;

    /*=============================================================================
    * Input Map
    =============================================================================*/
    [Export] public string move_left = "choose_left";
    [Export] public string move_right = "choose_right";
}
