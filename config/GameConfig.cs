using Godot;

[GlobalClass]
public partial class GameConfig : Resource {
    /*=============================================================================
    * Movement Times
    =============================================================================*/
    [Export] public float card_move_time { get; set; } = 0.4f;
    [Export] public float deal_time { get; set; } = 0.4f;
    [Export] public float choice_window_time { get; set; } = 3f;

    /*=============================================================================
    * Speeds
    =============================================================================*/
    [Export] public float countdowns_move_down_speed = 500f;

    /*=============================================================================
    * Input Map
    =============================================================================*/
    [Export] public string move_left = "choose_left";
    [Export] public string move_right = "choose_right";
}
