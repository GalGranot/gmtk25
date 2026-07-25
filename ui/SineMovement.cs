using System;
using Godot;

public partial class SineMovement : Sprite2D {
    Vector2 center = new(928, 544);
    float radius = 10f;
    float speed = 0.5f;
    float time = 30f;

    public override void _Process(double delta) {
        time += (float)(speed * delta);
        float x = center.X + Mathf.Cos(time) * radius;
        float y = center.Y + Mathf.Sin(time) * radius;
        Position = new Vector2(x, y);
    }
}