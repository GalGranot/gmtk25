using Godot;

public interface IOnCardPlayed {
    public CountdownResult on_card_played(Card card);
}

public abstract record CountdownResult {
    public sealed record Running : CountdownResult;
    public sealed record AddScore(int to_add) : CountdownResult;
    public sealed record MultScore(float mult_score) : CountdownResult;

    public string name => this switch {
        Running => "",
        AddScore(int to_add) => $" +{to_add}",
        MultScore(float mult_score) => $" x{mult_score}",
        _ => throw die_throw(),
    };
}


