using Godot;
using Godot.Collections;
using System;

public class Screens : Node
{

    [Signal]
    public delegate void StartGame();

    private BaseScreen _currentScreen;
    private BaseScreen _titleScreen;
    private BaseScreen _settingsScreen;
    private BaseScreen _gameOverScreen;
    private AudioStreamPlayer _clickPlayer;

    private Dictionary<bool, StreamTexture> _soundButtons;
    private Dictionary<bool, StreamTexture> _musicButtons;


    public override void _Ready()
    {
        RegisterButtons();
        
        _titleScreen = GetNode<BaseScreen>("TitleScreen");
        _settingsScreen = GetNode<BaseScreen>("SettingsScreen");
        _gameOverScreen = GetNode<BaseScreen>("GameOverScreen");
        _clickPlayer = GetNode<AudioStreamPlayer>("Click");

        _soundButtons = new Dictionary<bool, StreamTexture>();
        _soundButtons.Add(true,GD.Load<StreamTexture>("res://assets/images/buttons/audioOn.png"));
        _soundButtons.Add(false, GD.Load<StreamTexture>("res://assets/images/buttons/audioOff.png"));

        _musicButtons = new Dictionary<bool, StreamTexture>();
        _musicButtons.Add(true, GD.Load<StreamTexture>("res://assets/images/buttons/musicOn.png"));
        _musicButtons.Add(false, GD.Load<StreamTexture>("res://assets/images/buttons/musicOff.png"));


        ChangeScreen(_titleScreen);

    }

    public void RegisterButtons()
    {
        var buttons = GetTree().GetNodesInGroup("buttons");
        foreach(TextureButton btn in buttons)
        {
            btn.Connect("pressed", this, nameof(OnButtonPressed), new Godot.Collections.Array { btn }); ;
        }
    }

    public async void OnButtonPressed(TextureButton btn)
    {
        if (GameSettings.Instance().EnableSound)
        {
            _clickPlayer.Play();
        } 
        switch(btn.Name)
        {
            case "Home":
                ChangeScreen(_titleScreen);
                break;
            case "Play":
                ChangeScreen(null);
                await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
                EmitSignal("StartGame");
                break;

            case "Return":
                ChangeScreen(_titleScreen);
                break;

            case "Settings":
                ChangeScreen(_settingsScreen);
                break;

            case "Sound":
                GameSettings.Instance().EnableSound = !GameSettings.Instance().EnableSound;
                btn.TextureNormal = _soundButtons[GameSettings.Instance().EnableSound];
                break;
            case "Music":
                GameSettings.Instance().EnableMusic = !GameSettings.Instance().EnableMusic;
                btn.TextureNormal = _musicButtons[GameSettings.Instance().EnableMusic];
                break;
        }
    }

    public async void ChangeScreen(BaseScreen screen)
    {
        if (_currentScreen!=null)
        {
            _currentScreen.Disappear();
            await ToSignal(_currentScreen.tween, "tween_completed");
        }

        _currentScreen = screen;
        if (screen!=null)
        {
            _currentScreen.Appear();
            await ToSignal(_currentScreen.tween, "tween_completed");
        }
    }

    public void GameOver()
    {
        ChangeScreen(_gameOverScreen);
    }

}
