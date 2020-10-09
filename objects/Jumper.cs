using Godot;
using System;

public class Jumper : Area2D
{

    [Signal]
    public delegate void Captured(Circle circle);
    [Signal]
    public delegate void Died();

    private Vector2 _velocity = new Vector2(100, 0);
    private Line2D _trail;
    private AudioStreamPlayer _jumpSoundPlayer;
    private AudioStreamPlayer _captureSoundPlayer;
    private Sprite _sprite;

    private int _trailLength = 25;
    private float _jumpSpeed = 1000;

    public Circle Target = null;


    public override void _Ready()
    {
        _trail = GetNode<Line2D>("Trail/Points");
        _jumpSoundPlayer = GetNode<AudioStreamPlayer>("Jump");
        _captureSoundPlayer = GetNode<AudioStreamPlayer>("Capture");
        _sprite = GetNode<Sprite>("Sprite");
        (_sprite.Material as ShaderMaterial).SetShaderParam("color", GameSettings.Instance().ColorSchemes[GameSettings.Instance().Theme].PlayerBody);
        _trail.DefaultColor = GameSettings.Instance().ColorSchemes[GameSettings.Instance().Theme].PlayerTrail;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (Target!=null && @event is InputEventScreenTouch _event)
        {
            if (_event.IsPressed())
            {
                Jump();
            }
        } 
    }

    public override void _PhysicsProcess(float delta)
    {
        if (_trail.Points.Length> _trailLength)
        {
            _trail.RemovePoint(0);
        }

        _trail.AddPoint(Position);

        if (Target!=null)
        {
            Transform = Target.OrbitPosition.GlobalTransform;
        } else
        {
            Position += _velocity * delta;
        }
    }

    public void Jump()
    {
        Target.Implode();
        Target = null;
        _velocity = Transform.x * _jumpSpeed;
        if (GameSettings.Instance().EnableSound)
        {
            _jumpSoundPlayer.Play();
        }
    }

    public void OnJumperAreaEntered(Area2D area)
    {
        Target = (Circle)area;
        _velocity = Vector2.Zero;
        EmitSignal(nameof(Captured), new[] { Target });
        if (GameSettings.Instance().EnableSound)
        {
            _captureSoundPlayer.Play();
        }
    }

    public void Die()
    {
        Target = null;
        QueueFree();
    }

    public void OnJumperScreenExited()
    {
        if (Target == null)
        {
            EmitSignal(nameof(Died));
            Die();
        }
    }
}
