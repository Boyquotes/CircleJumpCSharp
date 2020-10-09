using Godot;
using System;

public class BaseScreen : CanvasLayer
{

    public Tween tween;

    public override void _Ready()
    {
        tween = GetNode<Tween>("Tween");
    }

    public void Appear()
    {
        GetTree().CallGroup("buttons", "set_disabled", false);

        tween.InterpolateProperty(
            this,
            "offset:x",
            500,
            0,
            0.5f,
            Tween.TransitionType.Back,
            Tween.EaseType.InOut
         );

        tween.Start();
    }

    public void Disappear()
    {
        GetTree().CallGroup("buttons", "set_disabled", true);
        tween.InterpolateProperty(
            this,
            "offset:x",
            0,
            500,
            0.5f,
            Tween.TransitionType.Back,
            Tween.EaseType.InOut
         );

        tween.Start();
    }
}
