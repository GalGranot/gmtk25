public enum Suit {
    Spades,
    Clubs,
    Hearts,
    Diamonds,
}

public enum CardColour { Red, Black }

public static class SuitExt {
    public static CardColour Colour(this Suit suit) => suit switch {
        Suit.Spades or Suit.Clubs => CardColour.Black,
        Suit.Diamonds or Suit.Hearts => CardColour.Red,
        _ => throw die_throw(),
    };
}