using Godot;

public static class EnumUtils {
    public static T random_enum<T>() {
        var values = System.Enum.GetValues(typeof(T));
        int randi = Random.int_in_range(values.Length);
        return (T)values.GetValue(randi);
    }
}
