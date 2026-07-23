using Godot;

public interface IOnCardPlayed {
    public CountdownState on_card_played(Card card);
}

public enum CountdownState {
    Finished,
    Running,
}