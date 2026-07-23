using System.Collections.Generic;
using Godot;

public static class Random {
    public static int in_range(int high) => in_range(0, high);
    public static int in_range(int low, int high) {
        assert(low < high);
        return (int)(GD.Randi() % high) + low;
    }

    public static void randomize_list<T>(List<T> ls) {
        for (int i = ls.Count - 1; i > 0; i--) {
            int j = (int)(GD.Randi() % (i + 1));
            (ls[i], ls[j]) = (ls[j], ls[i]);
        }
    }
}
