using System.Threading.Tasks;
using Godot;

public static class Time {
    public static async Task WaitForSeconds(this Node node, double seconds) =>
        await node.ToSignal(node.GetTree().CreateTimer(seconds), SceneTreeTimer.SignalName.Timeout);
}
