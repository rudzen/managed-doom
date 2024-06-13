//
// Copyright (C) 1993-1996 Id Software, Inc.
// Copyright (C) 2019-2020 Nobuaki Tanaka
// Copyright (C)      2024 Rudy Alex Kohn
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//

using System;
using System.IO;
using DrippyAL;
using ManagedDoom.Audio;
using ManagedDoom.Config;
using ManagedDoom.Doom.Game;

namespace ManagedDoom.Silk;

public sealed class AudioFactory : IAudioFactory
{
    private readonly ISound sound;
    private readonly IMusic music;

    public AudioFactory(IConfig config, ICommandLineArgs args, IGameContent gameContent)
    {
        if (!args.NoSound.Present && !(args.NoSfx.Present && args.NoMusic.Present))
        {
            var audioDevice = new AudioDevice();
            if (!args.NoSfx.Present)
                sound = new SilkSound(config.Values, gameContent, audioDevice);
            if (!args.NoMusic.Present)
                music = GetMusicInstance(config.Values, gameContent, audioDevice) ?? NullMusic.GetInstance();
        }
        else
        {
            sound = NullSound.GetInstance();
            music = NullMusic.GetInstance();
        }
    }

    public ISound GetSound() => sound;

    public IMusic GetMusic() => music;

    private static SilkMusic? GetMusicInstance(ConfigValues configValues, IGameContent content, AudioDevice device)
    {
        var sfPath = Path.Combine(ConfigUtilities.GetExeDirectory, configValues.AudioSoundfont);
        if (File.Exists(sfPath))
            return new SilkMusic(configValues, content, device, sfPath);

        Console.WriteLine($"SoundFont '{configValues.AudioSoundfont}' was not found!");
        return null;
    }
}