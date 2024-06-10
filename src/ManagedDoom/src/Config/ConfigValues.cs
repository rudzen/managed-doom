using System.Text.Json.Serialization;
using ManagedDoom.UserInput;

namespace ManagedDoom.Config;

public sealed class ConfigValues
{
    [JsonPropertyName("key_forward")]
    public KeyBinding KeyForward { get; set; } = null!;

    [JsonPropertyName("key_backward")]
    public KeyBinding KeyBackward { get; set; } = null!;

    [JsonPropertyName("key_strafeleft")]
    public KeyBinding KeyStrafeLeft { get; set; } = null!;

    [JsonPropertyName("key_straferight")]
    public KeyBinding KeyStrafeRight { get; set; } = null!;

    [JsonPropertyName("key_turnleft")]
    public KeyBinding KeyTurnLeft { get; set; } = null!;

    [JsonPropertyName("key_turnright")]
    public KeyBinding KeyTurnRight { get; set; } = null!;

    [JsonPropertyName("key_fire")]
    public KeyBinding KeyFire { get; set; } = null!;

    [JsonPropertyName("key_use")]
    public KeyBinding KeyUse { get; set; } = null!;

    [JsonPropertyName("key_run")]
    public KeyBinding KeyRun { get; set; } = null!;

    [JsonPropertyName("key_strafe")]
    public KeyBinding KeyStrafe { get; set; } = null!;

    [JsonPropertyName("mouse_sensitivity")]
    public int MouseSensitivity { get; set; }

    [JsonPropertyName("mouse_disableyaxis")]
    public bool MouseDisableYAxis { get; set; }

    [JsonPropertyName("game_alwaysrun")]
    public bool GameAlwaysRun { get; set; }

    [JsonPropertyName("video_screenwidth")]
    public int VideoScreenWidth { get; set; }

    [JsonPropertyName("video_screenheight")]
    public int VideoScreenHeight { get; set; }

    [JsonPropertyName("video_fullscreen")]
    public bool VideoFullscreen { get; set; }

    [JsonPropertyName("video_highresolution")]
    public bool VideoHighResolution { get; set; }

    [JsonPropertyName("video_displaymessage")]
    public bool VideoDisplayMessage { get; set; }

    [JsonPropertyName("video_gamescreensize")]
    public int VideoGameScreenSize { get; set; }

    [JsonPropertyName("video_gammacorrection")]
    public int VideoGammaCorrection { get; set; }

    [JsonPropertyName("video_fpsscale")]
    public int VideoFpsScale { get; set; }

    [JsonPropertyName("video_vsync")]
    public bool VideoVsync { get; set; }

    [JsonPropertyName("audio_soundvolume")]
    public int AudioSoundVolume { get; set; }

    [JsonPropertyName("audio_musicvolume")]
    public int AudioMusicVolume { get; set; }

    [JsonPropertyName("audio_randompitch")]
    public bool AudioRandomPitch { get; set; }

    [JsonPropertyName("audio_soundfont")]
    public string AudioSoundfont { get; set; } = null!;

    [JsonPropertyName("audio_musiceffect")]
    public bool AudioMusicEffect { get; set; }

    [JsonPropertyName("wad_directory")]
    public string WadDirectory { get; set; } = null!;

    public static ConfigValues CreateDefaults()
    {
        DoomMouseButton[] emptyMouseButtons = [];

        return new ConfigValues
        {
            KeyForward = new KeyBinding([DoomKey.Up, DoomKey.W], emptyMouseButtons),
            KeyBackward = new KeyBinding([DoomKey.Down, DoomKey.S], emptyMouseButtons),
            KeyStrafeLeft = new KeyBinding([DoomKey.A], emptyMouseButtons),
            KeyStrafeRight = new KeyBinding([DoomKey.D], emptyMouseButtons),
            KeyTurnLeft = new KeyBinding([DoomKey.Left], emptyMouseButtons),
            KeyTurnRight = new KeyBinding([DoomKey.Right], emptyMouseButtons),
            KeyFire = new KeyBinding([DoomKey.LControl, DoomKey.RControl], [DoomMouseButton.Mouse1]),
            KeyUse = new KeyBinding([DoomKey.Space], [DoomMouseButton.Mouse2]),
            KeyRun = new KeyBinding([DoomKey.LShift, DoomKey.RShift], emptyMouseButtons),
            KeyStrafe = new KeyBinding([DoomKey.LAlt, DoomKey.RAlt], emptyMouseButtons),
            MouseSensitivity = 8,
            MouseDisableYAxis = false,
            GameAlwaysRun = true,
            VideoScreenWidth = 640,
            VideoScreenHeight = 400,
            VideoFullscreen = false,
            VideoHighResolution = true,
            VideoGameScreenSize = 7,
            VideoDisplayMessage = true,
            VideoGammaCorrection = 2,
            VideoFpsScale = 2,
            VideoVsync = false,
            AudioSoundVolume = 8,
            AudioMusicVolume = 8,
            AudioRandomPitch = true,
            AudioSoundfont = "Touhou.sf2",
            AudioMusicEffect = true,
            WadDirectory = string.Empty
        };
    }
}