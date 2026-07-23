using System.Linq;
using System.Threading.Tasks;
using Godot;

public static class TaskExt {
    public static void Forget(this Task task) {
        task.ContinueWith(t => {
            if (t.IsFaulted) {
                var ex = t.Exception?.Flatten().InnerException;
                var trace = ex?.StackTrace?
                    .Split('\n')
                    .Select(line => System.Text.RegularExpressions.Regex.Replace(
                        line, @" in /.+/(\w+\.cs)", " in $1"))
                    .Aggregate((a, b) => a + "\n" + b);
                GD.PrintErr($"Async error: {ex?.Message}\n{trace}");
            }
        });
    }
}
