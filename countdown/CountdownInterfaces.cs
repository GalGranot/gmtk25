using Godot;

public interface IOnCardPlayed {
    public CountdownResult on_card_played(CardPlayedInfo card_played_info);
}

public interface IOnCountdownExpired {
    public CountdownResult on_countdown_expired();
}

public abstract record CountdownResult {
    public bool success;
    public CountdownResult(bool success) => this.success = success;

    public sealed record Running() : CountdownResult(false);
    public sealed record AddScore(bool success, int to_add) : CountdownResult(success);
    public sealed record MultScore(bool success, float mult_score) : CountdownResult(success);

    public string name => this switch {
        Running => "",
        AddScore(_, int to_add) => $" +{to_add}",
        MultScore(_, float mult_score) => $" x{mult_score}",
        _ => throw die_throw(),
    };

}
