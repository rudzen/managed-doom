﻿//
// Copyright (C) 1993-1996 Id Software, Inc.
// Copyright (C) 2019-2020 Nobuaki Tanaka
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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ManagedDoom
{
    public sealed class Config
    {
        public KeyBinding key_forward { get; init; }
        public KeyBinding key_backward { get; init; }
        public KeyBinding key_strafeleft { get; init; }
        public KeyBinding key_straferight { get; init; }
        public KeyBinding key_turnleft { get; init; }
        public KeyBinding key_turnright { get; init; }
        public KeyBinding key_fire { get; init; }
        public KeyBinding key_use { get; init; }
        public KeyBinding key_run { get; init; }
        public KeyBinding key_strafe { get; init; }

        public int mouse_sensitivity { get; set; }
        public bool mouse_disableyaxis { get; init; }

        public bool game_alwaysrun { get; init; }

        public int video_screenwidth { get; set; }
        public int video_screenheight { get; set; }
        public bool video_fullscreen { get; init; }
        public bool video_highresolution { get; init; }
        public bool video_displaymessage { get; set; }
        public int video_gamescreensize { get; set; }
        public int video_gammacorrection { get; set; }
        public int video_fpsscale { get; set; }

        public int audio_soundvolume { get; set; }
        public int audio_musicvolume { get; set; }
        public bool audio_randompitch { get; init; }
        public string audio_soundfont { get; init; }
        public bool audio_musiceffect { get; init; }

        // Default settings.
        public Config()
        {
            key_forward = new KeyBinding([DoomKey.Up, DoomKey.W]);
            key_backward = new KeyBinding([DoomKey.Down, DoomKey.S]);
            key_strafeleft = new KeyBinding([DoomKey.A]);
            key_straferight = new KeyBinding([DoomKey.D]);
            key_turnleft = new KeyBinding([DoomKey.Left]);
            key_turnright = new KeyBinding([DoomKey.Right]);
            key_fire = new KeyBinding([DoomKey.LControl, DoomKey.RControl], [DoomMouseButton.Mouse1]);
            key_use = new KeyBinding([DoomKey.Space], [DoomMouseButton.Mouse2]);
            key_run = new KeyBinding([DoomKey.LShift, DoomKey.RShift]);
            key_strafe = new KeyBinding([DoomKey.LAlt, DoomKey.RAlt]);

            mouse_sensitivity = 8;
            mouse_disableyaxis = false;

            game_alwaysrun = true;

            video_screenwidth = 640;
            video_screenheight = 400;
            video_fullscreen = false;
            video_highresolution = true;
            video_gamescreensize = 7;
            video_displaymessage = true;
            video_gammacorrection = 2;
            video_fpsscale = 2;

            audio_soundvolume = 8;
            audio_musicvolume = 8;
            audio_randompitch = true;
            audio_soundfont = "TimGM6mb.sf2";
            audio_musiceffect = true;

            IsRestoredFromFile = false;
        }

        public Config(string path) : this()
        {
            try
            {
                Console.Write("Restore settings: ");
                var start = Stopwatch.GetTimestamp();

                var dic = new Dictionary<string, string>();
                foreach (var line in File.ReadLines(path))
                {
                    var split = line.Split('=', StringSplitOptions.RemoveEmptyEntries);
                    if (split.Length == 2)
                    {
                        dic[split[0].Trim()] = split[1].Trim();
                    }
                }

                key_forward = GetKeyBinding(dic, nameof(key_forward), key_forward);
                key_backward = GetKeyBinding(dic, nameof(key_backward), key_backward);
                key_strafeleft = GetKeyBinding(dic, nameof(key_strafeleft), key_strafeleft);
                key_straferight = GetKeyBinding(dic, nameof(key_straferight), key_straferight);
                key_turnleft = GetKeyBinding(dic, nameof(key_turnleft), key_turnleft);
                key_turnright = GetKeyBinding(dic, nameof(key_turnright), key_turnright);
                key_fire = GetKeyBinding(dic, nameof(key_fire), key_fire);
                key_use = GetKeyBinding(dic, nameof(key_use), key_use);
                key_run = GetKeyBinding(dic, nameof(key_run), key_run);
                key_strafe = GetKeyBinding(dic, nameof(key_strafe), key_strafe);

                mouse_sensitivity = GetInt(dic, nameof(mouse_sensitivity), mouse_sensitivity);
                mouse_disableyaxis = GetBool(dic, nameof(mouse_disableyaxis), mouse_disableyaxis);

                game_alwaysrun = GetBool(dic, nameof(game_alwaysrun), game_alwaysrun);

                video_screenwidth = GetInt(dic, nameof(video_screenwidth), video_screenwidth);
                video_screenheight = GetInt(dic, nameof(video_screenheight), video_screenheight);
                video_fullscreen = GetBool(dic, nameof(video_fullscreen), video_fullscreen);
                video_highresolution = GetBool(dic, nameof(video_highresolution), video_highresolution);
                video_displaymessage = GetBool(dic, nameof(video_displaymessage), video_displaymessage);
                video_gamescreensize = GetInt(dic, nameof(video_gamescreensize), video_gamescreensize);
                video_gammacorrection = GetInt(dic, nameof(video_gammacorrection), video_gammacorrection);
                video_fpsscale = GetInt(dic, nameof(video_fpsscale), video_fpsscale);

                audio_soundvolume = GetInt(dic, nameof(audio_soundvolume), audio_soundvolume);
                audio_musicvolume = GetInt(dic, nameof(audio_musicvolume), audio_musicvolume);
                audio_randompitch = GetBool(dic, nameof(audio_randompitch), audio_randompitch);
                audio_soundfont = GetString(dic, nameof(audio_soundfont), audio_soundfont);
                audio_musiceffect = GetBool(dic, nameof(audio_musiceffect), audio_musiceffect);

                IsRestoredFromFile = true;

                Console.WriteLine("OK [" + Stopwatch.GetElapsedTime(start) + ']');
            }
            catch
            {
                Console.WriteLine("Failed");
            }
        }

        public void Save(string path)
        {
            try
            {
                using var writer = new StreamWriter(path);
                writer.WriteLine(nameof(key_forward) + " = " + key_forward);
                writer.WriteLine(nameof(key_backward) + " = " + key_backward);
                writer.WriteLine(nameof(key_strafeleft) + " = " + key_strafeleft);
                writer.WriteLine(nameof(key_straferight) + " = " + key_straferight);
                writer.WriteLine(nameof(key_turnleft) + " = " + key_turnleft);
                writer.WriteLine(nameof(key_turnright) + " = " + key_turnright);
                writer.WriteLine(nameof(key_fire) + " = " + key_fire);
                writer.WriteLine(nameof(key_use) + " = " + key_use);
                writer.WriteLine(nameof(key_run) + " = " + key_run);
                writer.WriteLine(nameof(key_strafe) + " = " + key_strafe);

                writer.WriteLine(nameof(mouse_sensitivity) + " = " + mouse_sensitivity);
                writer.WriteLine(nameof(mouse_disableyaxis) + " = " + BoolToString(mouse_disableyaxis));

                writer.WriteLine(nameof(game_alwaysrun) + " = " + BoolToString(game_alwaysrun));

                writer.WriteLine(nameof(video_screenwidth) + " = " + video_screenwidth);
                writer.WriteLine(nameof(video_screenheight) + " = " + video_screenheight);
                writer.WriteLine(nameof(video_fullscreen) + " = " + BoolToString(video_fullscreen));
                writer.WriteLine(nameof(video_highresolution) + " = " + BoolToString(video_highresolution));
                writer.WriteLine(nameof(video_displaymessage) + " = " + BoolToString(video_displaymessage));
                writer.WriteLine(nameof(video_gamescreensize) + " = " + video_gamescreensize);
                writer.WriteLine(nameof(video_gammacorrection) + " = " + video_gammacorrection);
                writer.WriteLine(nameof(video_fpsscale) + " = " + video_fpsscale);

                writer.WriteLine(nameof(audio_soundvolume) + " = " + audio_soundvolume);
                writer.WriteLine(nameof(audio_musicvolume) + " = " + audio_musicvolume);
                writer.WriteLine(nameof(audio_randompitch) + " = " + BoolToString(audio_randompitch));
                writer.WriteLine(nameof(audio_soundfont) + " = " + audio_soundfont);
                writer.WriteLine(nameof(audio_musiceffect) + " = " + BoolToString(audio_musiceffect));
            }
            catch
            {
            }
        }

        private static int GetInt(Dictionary<string, string> dic, string name, int defaultValue)
        {
            if (dic.TryGetValue(name, out var stringValue))
            {
                if (int.TryParse(stringValue, out var value))
                {
                    return value;
                }
            }

            return defaultValue;
        }

        private static string GetString(Dictionary<string, string> dic, string name, string defaultValue)
        {
            if (dic.TryGetValue(name, out var stringValue))
            {
                return stringValue;
            }

            return defaultValue;
        }

        private static bool GetBool(Dictionary<string, string> dic, string name, bool defaultValue)
        {
            if (dic.TryGetValue(name, out var stringValue))
            {
                if (stringValue == "true")
                    return true;

                if (stringValue == "false")
                    return false;
            }

            return defaultValue;
        }

        private static KeyBinding GetKeyBinding(Dictionary<string, string> dic, string name, KeyBinding defaultValue)
        {
            if (dic.TryGetValue(name, out var stringValue))
                return KeyBinding.Parse(stringValue);

            return defaultValue;
        }

        private static string BoolToString(bool value)
        {
            return value ? "true" : "false";
        }

        public bool IsRestoredFromFile { get; }
    }
}