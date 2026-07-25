public enum Rank {
    Ace = 0,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King,
}

public static class RankExt {
    public static Rank up(this Rank rank) {
        if (rank is not Rank.King) {
            return rank + 1;
        }
        return Rank.Ace;
    }

    public static Rank down(this Rank rank) {
        if(rank is not Rank.Ace) {
            return rank - 1;
        }
        return Rank.King;
    }

    public static bool is_face_card(this Rank rank) => rank switch {
        Rank.Jack or Rank.Queen or Rank.King => true,
        _ => false,
    };
}