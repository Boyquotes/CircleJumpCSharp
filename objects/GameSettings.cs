using System;
using System.Collections.Generic;
using Godot;

public class GameSettings
{
    public bool EnableSound { get; set; }
    public bool EnableMusic { get; set; }
    public int CirclesPerLevel { get; set; }
    public string Theme { get; set; }

    public Dictionary<String,ColorScheme> ColorSchemes { get; set; }

    private static GameSettings settings;

    public GameSettings()
    {
        ColorSchemes = new Dictionary<string, ColorScheme>();
        generateColorSchemes();

        EnableMusic = true;
        EnableSound = true;
        Theme = "NEON1";
        CirclesPerLevel = 5;

    }

    public static GameSettings Instance()
    {
        if (settings != null)
        {
            return settings;
        }

        settings = new GameSettings();
        return settings;
    }

    private void generateColorSchemes()
    {

        ColorSchemes.Add("NEON1", new ColorScheme
        {
            Background = new Color(0, 0, 0),
            PlayerBody = new Color(203, 255, 0),
            PlayerTrail = new Color(204, 0, 255),
            CircleFill = new Color(255, 0, 110),
            CircleStatic = new Color(0, 255, 102),
            CircleLimited = new Color(204, 0, 255)
        });

        ColorSchemes.Add("NEON2", new ColorScheme
        {
            Background = new Color(0, 0, 0),
            PlayerBody = new Color(246, 255, 0),
            PlayerTrail = new Color(255, 255, 255),
            CircleFill = new Color(255, 0, 110),
            CircleStatic = new Color(151, 255, 48),
            CircleLimited = new Color(127, 0, 255)
        });
        ColorSchemes.Add("NEON3", new ColorScheme
        {
            Background = new Color(76, 84, 95),
            PlayerBody = new Color(255, 0, 187),
            PlayerTrail = new Color(255, 148, 0),
            CircleFill = new Color(255, 148, 0),
            CircleStatic = new Color(170, 255, 0),
            CircleLimited = new Color(204, 0, 255)
        });
    }

    public static int RandWeighted(int[] weights)
    {
        var sum = 0;
        foreach(var w in weights)
        {
            sum += w;
        }

        var num = GD.RandRange(0, sum);
        int ret = 0;
        for (int i=0;i<weights.Length;i++)
        {
            if (num<weights[i])
            {
                ret = i;
                break;
            }
            num -= weights[i];
        }
        return ret;
    }
}