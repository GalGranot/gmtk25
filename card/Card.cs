using System;
using System.Threading.Tasks;
using Godot;

[GlobalClass]
public partial class Card : Node2D {
    public CardId id { get; private set; }
    [Export] Sprite2D front;
    [Export] Sprite2D back;
    [Export] Area2D area;
    public CardSlot slot;

    public static event Action<Card> on_click;

    public override void _Ready() {
        area.InputEvent += on_clicked;
    }

    public void _Initialize(CardId id) {
        this.id = id;
        front.Texture = CardSpriteLoader.from_id(id);
        back.Texture = CardSpriteLoader.from_name("cardBack_red1");
        change_orientation(Orientation.Front);
    }

    void on_clicked(Node viewport, InputEvent @event, long shape_idx) {
        if (@event is InputEventMouseButton mouse &&
            mouse.ButtonIndex == MouseButton.Left &&
            mouse.Pressed
        ) {
            GD.Print(name);
            on_click?.Invoke(this);
        }
    }

    public string name => id.name;

    /*=============================================================================
    * Movement
    =============================================================================*/
    // public Lerp new_lerp(Vector2 end, float duration) =>
    //     LerpFactory.I.create(this, new LerpParams(end, duration));

    /*=============================================================================
    * Display
    =============================================================================*/
    public enum Orientation { Front, Back }
    Orientation orientation;

    void change_orientation(Orientation orientation) {
        if (orientation is Orientation.Front) {
            back.Hide();
            front.Show();
        } else {
            front.Hide();
            back.Show();
        }
        this.orientation = orientation;
    }
    void flip() => change_orientation(orientation is Orientation.Front ? Orientation.Back : Orientation.Front);
    public void show_front() => change_orientation(Orientation.Front);
    public void show_back() => change_orientation(Orientation.Back);
    public int score => id.score;
    public Suit suit => id.suit;
    public Rank rank => id.rank;
    public CardColour colour => id.suit.Colour();

    public void up() {
        id = new CardId(id.rank.up(), id.suit);
        _Initialize(id);
    }

    public void down() {
        id = new CardId(id.rank.down(), id.suit);
        _Initialize(id);
    }

    public void change_suit() {
        Suit new_suit = id.suit switch {
            Suit.Clubs => Suit.Diamonds,
            Suit.Diamonds => Suit.Spades,
            Suit.Spades => Suit.Hearts,
            Suit.Hearts => Suit.Clubs,
            _ => throw die_throw(),
        };
        id = new CardId(id.rank, new_suit);
        _Initialize(id);
    }
}
