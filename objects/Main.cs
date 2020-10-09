using Godot;
using System;

public class Main : Node
{
    private PackedScene _circleScene;
    private PackedScene _jumperScene; 
    private Circle _circle;
    private Jumper _player;
    private AudioStreamPlayer _musicPlayer;
    private HUD _hud;

    private int _score = 0;
    private int _level = 0;

    public override void _Ready()
    {
        GD.Randomize();

        _circleScene = GD.Load<PackedScene>("res://objects/Circle.tscn");
        _jumperScene = GD.Load<PackedScene>("res://objects/Jumper.tscn");
        _musicPlayer = GetNode<AudioStreamPlayer>("Music");
        _hud = GetNode<HUD>("HUD");
        _hud.Hide();
    }



    public void NewGame()
    {
        _score = 0;
        _level = 1;
        _hud.UpdateScore(_score);
        var camera = GetNode<Camera2D>("Camera2D");
        var startPosition = GetNode<Position2D>("StartPosition");

        _player = (Jumper)_jumperScene.Instance();
        _player.Position = startPosition.Position;
        AddChild(_player);

        _player.Connect("Captured", this, nameof(OnJumperCaptured));
        _player.Connect("Died", this, nameof(OnJumperDied));
        SpawnCircle(startPosition.Position);

        _hud.Show();
        _hud.ShowMessage("GO!!");

        if (GameSettings.Instance().EnableMusic)
        {
            _musicPlayer.Play();
        }
    }

    public void SpawnCircle(Vector2? position)
    {
        var c = (Circle)_circleScene.Instance();
        if (position==null)
        {
            var x = GD.RandRange(-150, 150);
            var y = GD.RandRange(-500, -400);
            position = _player.Target.Position + new Vector2((float)x, (float)y);
        }

        AddChild(c);
        c.Init(position,_level);
    }

    public void OnJumperCaptured(Circle circle)
    {
        GetNode<Camera2D>("Camera2D").Position = circle.Position;
        circle.Capture(_player);
        CallDeferred(nameof(SpawnCircle), new[] { Vector2.Zero });

        SetScore(_score+1);


    }

    public void OnJumperDied()
    {
        GetTree().CallGroup("circles", "Implode");
        GetNode<Screens>("Screens").GameOver();

        _hud.Hide();
        if (GameSettings.Instance().EnableMusic)
        {
            _musicPlayer.Stop();
        }
    }

    public void SetScore(int score)
    {
        _score = score;
        _hud.UpdateScore(score);
        if (score>0 && score % GameSettings.Instance().CirclesPerLevel==0)
        {
            _level += 1;
            _hud.ShowMessage($"Level {_level}");
        }
    }

}
