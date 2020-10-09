using Godot;
using System;


public enum CircleMode
{
    STATIC=0,LIMITED=1
}

public class Circle : Area2D
{
    public Position2D OrbitPosition;
    private AnimationPlayer _animationPlayer;
    private Jumper _jumper = null;
    private Node2D _pivot;
    private Label _label;
    private Sprite _sprite;
    private Sprite _spriteEffect;
    private Tween _moveTween;
    private AudioStreamPlayer _beepPlayer;

    private CircleMode _mode = CircleMode.STATIC;

    private float _moveRange = 0;
    private float _moveSpeed = 2;
    private int _numOrbits=3;
    private int _currentOrbits = 0;
    private float _orbitStart = 0;
    private float _radius = 100;
    private float _rotationSpeed = (float)Math.PI;

    public override void _Ready()
    {
        OrbitPosition = GetNode<Position2D>("Pivot/OrbitPosition");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _pivot = GetNode<Node2D>("Pivot");
        _label = GetNode<Label>("Label");
        _beepPlayer = GetNode<AudioStreamPlayer>("Beep");
        _sprite = GetNode<Sprite>("Sprite");
        _spriteEffect = GetNode<Sprite>("SpriteEffect");
        _moveTween = GetNode<Tween>("MoveTween");
    }

    public override void _Process(float delta)
    {
        _pivot.Rotation += _rotationSpeed * delta;

        if (_mode==CircleMode.LIMITED && _jumper!=null)
        {
            CheckOrbits();
            Update();
        }
    }

    public override void _Draw()
    {
        if (_jumper!=null)
        {
            var r = ((_radius - 50) / _numOrbits) * (1 + _numOrbits - _currentOrbits);
            DrawCircleArcPoly(
                Vector2.Zero,
                r + 10,
                _orbitStart + (float)Math.PI/2,
                _pivot.Rotation + (float)Math.PI/2,
                GameSettings.Instance().ColorSchemes[GameSettings.Instance().Theme].CircleFill
            );
        }
    }

    public void CheckOrbits()
    {

        if (Math.Abs(_pivot.Rotation - _orbitStart)>2 * Math.PI )
        {
            _currentOrbits -= 1;
            if (GameSettings.Instance().EnableSound) {
                _beepPlayer.Play();
            }

            _label.Text = _currentOrbits.ToString();

            if (_currentOrbits<=0)
            {
                _jumper.Die();
                _jumper = null;
                Implode();
            }


            _orbitStart = _pivot.Rotation;
        }
    }

    public void SetMode(CircleMode mode)
    {
        _mode = mode;
        var color = new Color();
       
        switch(_mode)
        {
            case CircleMode.STATIC:
                _label.Hide();
                color = GameSettings.Instance().ColorSchemes[GameSettings.Instance().Theme].CircleStatic;
                break;
            case CircleMode.LIMITED:
                _currentOrbits = _numOrbits;
                _label.Text = _currentOrbits.ToString();
                _label.Show();
                color = GameSettings.Instance().ColorSchemes[GameSettings.Instance().Theme].CircleLimited;

                break;
            default:
                break;
        }
        (_sprite.Material as ShaderMaterial).SetShaderParam("color", color);
        
    }

    public void Init(Vector2? position,int level= 1)
    {
        var mode = GameSettings.RandWeighted(new int[] { 10, level - 1 });
        if (mode == 0)
        {
            SetMode(CircleMode.STATIC);
        } else
        {
            SetMode(CircleMode.LIMITED);
        }
        if (position != null) Position = position.Value;

        var moveChance = Mathf.Clamp(level - 10, 0, 9)/10.0f;

        if (GD.Randf()<moveChance)
        {
            _moveRange = (float)(Mathf.Max(25.0f, (float)(100.0f * GD.RandRange(0.75f, 1.25f) * moveChance)) * Math.Pow(-1,GD.Randi() % 2));
            _moveSpeed = Mathf.Max(2.5f - Mathf.Ceil(level / 5.0f) * 0.25f, 0.75f);

        }

        var smallChance = Mathf.Min(0.9f, Mathf.Max(0, (level - 10.0f) / 20.0f));
        if (GD.Randf()<smallChance)
        {
            _radius = Mathf.Max(50.0f, (float)(_radius - level * GD.RandRange(0.75, 1.25)));
        }

        _sprite.Material = (ShaderMaterial)_sprite.Material.Duplicate();
        _spriteEffect.Material = _sprite.Material;


        var collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        collisionShape.Shape = (CircleShape2D)collisionShape.Shape.Duplicate();
        ((CircleShape2D)collisionShape.Shape).Radius = _radius;

        var imgSize = _sprite.Texture.GetSize().x / 2;
        _sprite.Scale = Vector2.One * _radius / imgSize;

        OrbitPosition.Position = new Vector2(_radius + 25, OrbitPosition.Position.y);

        _rotationSpeed *= (float)Math.Pow(-1, GD.Randi() % 2);
        MoveTween(null, null);
    }

    public async void Implode()
    {
        _animationPlayer.Play("implode");
        await ToSignal(_animationPlayer, "animation_finished");
        QueueFree();
    }

    public void Capture(Jumper target)
    {
        _jumper = target;
        _animationPlayer.Play("capture");
      
        _pivot.Rotation = (_jumper.Position - Position).Angle();
        _orbitStart = _pivot.Rotation;
    }

    public void DrawCircleArcPoly(Vector2 center, float radius, float angleFrom, float angleTo, Color color)
    {
        int nbPoints = 32;
        var pointsArc = new Vector2[nbPoints + 1];
        pointsArc[0] = center;
        var colors = new Color[] { color };

        for (int i = 0; i < nbPoints; ++i)
        {
            float anglePoint = angleFrom + i * (angleTo - angleFrom) / nbPoints - (float)Math.PI/2;
            pointsArc[i + 1] = center + new Vector2(Mathf.Cos(anglePoint), Mathf.Sin(anglePoint)) * radius;
        }

        DrawPolygon(pointsArc, colors,antialiased:true);
    }

    public void MoveTween(Godot.Object @object, NodePath key)
    {
        if (_moveRange==0)
        {
            return;
        }
        _moveRange *= -1;
        _moveTween.InterpolateProperty(
            this,
            "position:x",
            Position.x,
            Position.x + _moveRange,
            _moveSpeed,
            Tween.TransitionType.Quad,
            Tween.EaseType.InOut
        );

        _moveTween.Start();
    }
}
