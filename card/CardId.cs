public record struct CardId {
    public readonly Rank rank { get; }
    public readonly Suit suit { get; }

    public CardId(Rank rank, Suit suit) {
        this.rank = rank;
        this.suit = suit;
    }

    public static CardId random() =>
        new(EnumUtils.random_enum<Rank>(), EnumUtils.random_enum<Suit>());

    public string name => $"{rank} of {suit}";

    public int score => (rank, suit) switch {
        (Rank.Ace, _) => 11,
        (Rank.Two, _) => 2,
        (Rank.Three, _) => 3,
        (Rank.Four, _) => 4,
        (Rank.Five, _) => 5,
        (Rank.Six, _) => 6,
        (Rank.Seven, _) => 7,
        (Rank.Eight, _) => 8,
        (Rank.Nine, _) => 9,
        (Rank.Ten, _) => 10,
        (Rank.Jack, _) => 10,
        (Rank.Queen, _) => 10,
        (Rank.King, _) => 10,

        _ => throw new System.Exception("Invalid rank"),
    };
}
