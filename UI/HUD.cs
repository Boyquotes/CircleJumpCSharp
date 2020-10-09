using Godot;
using System;

public class HUD : CanvasLayer
{
    private AnimationPlayer _player;
    private Label _messageLabel;
    private MarginContainer _scoreBox;
    private Label _scoreLabel;


    public override void _Ready()
    {
        _player = GetNode<AnimationPlayer>("AnimationPlayer");
        _messageLabel = GetNode<Label>("Message");
        _scoreBox = GetNode<MarginContainer>("ScoreBox");
        _scoreLabel = GetNode<Label>("ScoreBox/HBoxContainer/Score");
    }

    public void ShowMessage(string message)
    {
        _messageLabel.Text = message;
        _player.Play("show_message");
    }

    public void Hide()
    {
        _scoreBox.Hide();
    }

    public void Show()
    {
        _scoreBox.Show();
    }

    public void UpdateScore(int score)
    {
        _scoreLabel.Text = score.ToString();
    }
}
