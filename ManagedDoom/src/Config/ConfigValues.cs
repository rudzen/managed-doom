using System;
using System.Text.Json.Serialization;

namespace ManagedDoom;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(ConfigValues))]
public sealed class ConfigValues
{
    public KeyBinding key_forward { get; set; }
    public KeyBinding key_backward { get; set; }
    public KeyBinding key_strafeleft { get; set; }
    public KeyBinding key_straferight { get; set; }
    public KeyBinding key_turnleft { get; set; }
    public KeyBinding key_turnright { get; set; }
    public KeyBinding key_fire { get; set; }
    public KeyBinding key_use { get; set; }
    public KeyBinding key_run { get; set; }
    public KeyBinding key_strafe { get; set; }

    public int mouse_sensitivity { get; set; }
    public bool mouse_disableyaxis { get; set; }

    public bool game_alwaysrun { get; set; }

    public int video_screenwidth { get; set; }
    public int video_screenheight { get; set; }
    public bool video_fullscreen { get; set; }
    public bool video_highresolution { get; set; }
    public bool video_displaymessage { get; set; }
    public int video_gamescreensize { get; set; }
    public int video_gammacorrection { get; set; }
    public int video_fpsscale { get; set; }

    public int audio_soundvolume { get; set; }
    public int audio_musicvolume { get; set; }
    public bool audio_randompitch { get; set; }
    public string audio_soundfont { get; set; }
    public bool audio_musiceffect { get; set; }

    public string wad_directory { get; set; }

    public static ConfigValues CreateDefaults()
    {
        DoomMouseButton[] emptyMouseButtons = [];
        
        var configValues = new ConfigValues
        {
            key_forward = new KeyBinding([DoomKey.Up, DoomKey.W], emptyMouseButtons),
            key_backward = new KeyBinding([DoomKey.Down, DoomKey.S], emptyMouseButtons),
            key_strafeleft = new KeyBinding([DoomKey.A], emptyMouseButtons),
            key_straferight = new KeyBinding([DoomKey.D], emptyMouseButtons),
            key_turnleft = new KeyBinding([DoomKey.Left], emptyMouseButtons),
            key_turnright = new KeyBinding([DoomKey.Right], emptyMouseButtons),
            key_fire = new KeyBinding([DoomKey.LControl, DoomKey.RControl], [DoomMouseButton.Mouse1]),
            key_use = new KeyBinding([DoomKey.Space], [DoomMouseButton.Mouse2]),
            key_run = new KeyBinding([DoomKey.LShift, DoomKey.RShift], emptyMouseButtons),
            key_strafe = new KeyBinding([DoomKey.LAlt, DoomKey.RAlt], emptyMouseButtons),
            mouse_sensitivity = 8,
            mouse_disableyaxis = false,
            game_alwaysrun = true,
            video_screenwidth = 640,
            video_screenheight = 400,
            video_fullscreen = false,
            video_highresolution = true,
            video_gamescreensize = 7,
            video_displaymessage = true,
            video_gammacorrection = 2,
            video_fpsscale = 2,
            audio_soundvolume = 8,
            audio_musicvolume = 8,
            audio_randompitch = true,
            audio_soundfont = "Touhou.sf2",
            audio_musiceffect = true,
            wad_directory = string.Empty
        };

        return configValues;
    }
}