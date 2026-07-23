using System;
using System.Collections;
using System.Collections.Generic;

public static class ListExt {
    public static bool IsEmpty<T>(this List<T> ls) => ls.Count == 0;

    public static T Pop<T>(this List<T> ls) {
        int last = ls.Count - 1;
        assert(last >= 0);
        T item = ls[last];
        ls.RemoveAt(last);
        return item;
    }

    public static List<U> Map<T, U>(this List<T> ls, Func<T, U> fn) {
        List<U> mapped = new(ls.Count);
        foreach (T item in ls) {
            mapped.Add(fn(item));
        }
        return mapped;
    }

    public static List<U> Map<T, U>(this IEnumerable<T> items, Func<T, U> fn) {
        List<U> mapped = new();
        foreach (T item in items) {
            mapped.Add(fn(item));
        }
        return mapped;
    }

    public static void ForEach<T>(this List<T> ls, Action<T> fn) {
        foreach (T item in ls) { fn(item); }
    }

    public static void ForEach<T>(this IEnumerable<T> items, Action<T> fn) {
        foreach (T item in items) { fn(item); }
    }

    public static List<T> Collect<T>(params object[] inputs) {
        List<T> ls = new();
        foreach (var input in inputs) {
            switch (input) {
                case null:
                    continue;
                case T item:
                    ls.Add(item);
                    break;
                case IEnumerable enumerable:
                    foreach (var element in enumerable) {
                        if (element is T item) { ls.Add(item); }
                    }
                    break;
            }
        }
        return ls;
    }

    public static T Find<T>(this IEnumerable<T> ls, Func<T, bool> predicate) {
        foreach (T item in ls) {
            if (predicate(item)) {
                return item;
            }
        }
        return default;
    }
}
