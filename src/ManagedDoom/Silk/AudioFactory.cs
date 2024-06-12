using System;
using System.IO;
using DrippyAL;
using ManagedDoom.Audio;
using ManagedDoom.Config;
using ManagedDoom.Doom.Game;

namespace ManagedDoom.Silk;

public sealed class AudioFactory : IAudioFactory
{
    private readonly ISound? sound;
    private readonly IMusic? music;

    public AudioFactory(IConfig config, ICommandLineArgs args, IGameContent gameContent)
    {
        if (!args.NoSound.Present && !(args.NoSfx.Present && args.NoMusic.Present))
        {
            var audioDevice = new AudioDevice();
            if (!args.NoSfx.Present)
                sound = new SilkSound(config.Values, gameContent, audioDevice);
            if (!args.NoMusic.Present)
                music = GetMusicInstance(config.Values, gameContent, audioDevice);
        }
        else
        {
            sound = null;
            music = null;
        }
    }

    public ISound? GetSound() => sound;

    public IMusic? GetMusic() => music;

    private static SilkMusic? GetMusicInstance(ConfigValues configValues, IGameContent content, AudioDevice device)
    {
        var sfPath = Path.Combine(ConfigUtilities.GetExeDirectory, configValues.AudioSoundfont);
        if (File.Exists(sfPath))
            return new SilkMusic(configValues, content, device, sfPath);

        Console.WriteLine($"SoundFont '{configValues.AudioSoundfont}' was not found!");
        return null;
    }
}