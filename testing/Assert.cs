using System;
using System.IO;
using System.Runtime.CompilerServices;
using Godot;

public static class Assert {
    static void fail_assert(string file, int line, string fn, string expr) {
        const string project_root = "cambio";
        int i = file.LastIndexOf(project_root + Path.DirectorySeparatorChar);
        if (i >= 0) {
            file = file[(i + project_root.Length + 1)..];
        }
        GD.PrintErr($"{file}:{line} {fn}: Assert failed: {expr}");
        throw new System.Exception();
    }

    public static void assert(
        bool cond,
        [CallerArgumentExpression(nameof(cond))] string expr = "",
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string fn = ""
    ) {
        if (!cond) {
            fail_assert(file, line, fn, expr);
        }
    }

    public static void assert_not_null<T>(
        T t,
        [CallerArgumentExpression(nameof(t))] string expr = "",
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string fn = ""
    ) {
        if (t is null) {
            fail_assert(file, line, fn, expr);
        }
    }

    public static void assert_is_null<T>(
        T t,
        [CallerArgumentExpression(nameof(t))] string expr = "",
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string fn = ""
    ) {
        if (t is not null) {
            fail_assert(file, line, fn, expr);
        }
    }

    public static void die(
        string expr = "",
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string fn = ""
    ) => fail_assert(file, line, fn, expr);

    public static Exception die_throw(
        string expr = "",
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0,
        [CallerMemberName] string fn = ""
    ) {
        fail_assert(file, line, fn, expr);
        return new System.Exception();
    }
}
