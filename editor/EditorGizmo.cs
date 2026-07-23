using Godot;

[Tool]
[GlobalClass]
public partial class EditorGizmo : Node2D {
    [Export] Texture2D texture;

    public override void _Draw() {
        if (!Engine.IsEditorHint()) {
            return;
        }
        Vector2 size = texture.GetSize();
        DrawTexture(texture, -size / 2);
    }
}
