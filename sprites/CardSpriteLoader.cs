using Godot;

public static class CardSpriteLoader {
    const string sprites_path = "res://sprites/";

    public static Texture2D from_id(CardId id) {
        string name = $"{id.rank}_{id.suit}".ToLower();
        return from_name(name);
    }

    public static Texture2D from_name(string name) {
        string path = $"{sprites_path}{name}.png";
        Texture2D texture = ResourceLoader.Load<Texture2D>(path);
        assert_not_null(texture);
        return texture;
    }
}
