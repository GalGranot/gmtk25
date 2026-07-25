using Godot;

[GlobalClass]
public partial class GameConfig : Resource {
    /*=============================================================================
    * Movement Times
    =============================================================================*/
    [Export] public float card_move_time_secs { get; set; } = 0.4f;
    [Export] public float deal_time_secs { get; set; } = 0.4f;
    [Export] public float choice_window_time_secs { get; set; } = 3f;

    /*=============================================================================
    * Speeds & Delays
    =============================================================================*/
    [Export] public float countdowns_move_down_speed { get; set; } = 100f;
    [Export] public float countdown_spawn_delay_secs { get; set; } = 4f;
    [Export] public int default_countdown_secs { get; set; } = 5;

    /*=============================================================================
    * Gameplay
    =============================================================================*/
    [Export] public int play_x_coloured_cards_required { get; set; } = 3;
    [Export] public int default_countdown_lifetime_secs { get; set; } = 5;
    [Export] public int short_countdown_lifetime_secs { get; set; } = 3;
    [Export] public int round_len_secs { get; set; } = 120;

    /*=============================================================================
    * Input Map
    =============================================================================*/
    [Export] public string move_left = "choose_left";
    [Export] public string move_right = "choose_right";
    [Export] public string choose_up = "choose_up";
    [Export] public string choose_down = "choose_down";
    [Export] public string spacebar = "spacebar";
    [Export] public string escape = "goto_main_menu";
}
