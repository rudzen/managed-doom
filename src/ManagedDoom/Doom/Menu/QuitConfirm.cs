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
using ManagedDoom.Audio;
using ManagedDoom.Doom.Common;
using ManagedDoom.Doom.Event;
using ManagedDoom.Doom.Game;
using ManagedDoom.Doom.Info;
using ManagedDoom.UserInput;

namespace ManagedDoom.Doom.Menu;

public sealed class QuitConfirm(DoomMenu menu, Doom app) : MenuDef(menu)
{
    private static Sfx[] doomQuitSoundList =>
    [
        Sfx.PLDETH,
        Sfx.DMPAIN,
        Sfx.POPAIN,
        Sfx.SLOP,
        Sfx.TELEPT,
        Sfx.POSIT1,
        Sfx.POSIT3,
        Sfx.SGTATK
    ];

    private static Sfx[] doom2QuitSoundList =>
    [
        Sfx.VILACT,
        Sfx.GETPOW,
        Sfx.BOSCUB,
        Sfx.SLOP,
        Sfx.SKESWG,
        Sfx.KNTDTH,
        Sfx.BSPACT,
        Sfx.SGTATK
    ];

    private readonly DoomRandom random = new(DateTime.Now.Millisecond);
    private string[] text = [];

    private int endCount = -1;

    public ReadOnlySpan<string> Text => text;

    public override void Open()
    {
        var list = app.Options.GameMode == GameMode.Commercial
            ? app.Options.MissionPack == MissionPack.Doom2 ? DoomInfo.QuitMessages.Doom2 : DoomInfo.QuitMessages.FinalDoom
            : DoomInfo.QuitMessages.Doom;

        text = (list[random.Next() % list.Count] + "\n\n" + DoomInfo.Strings.PRESSYN).Split('\n');
    }

    public override bool DoEvent(DoomEvent e)
    {
        if (endCount != -1)
            return true;

        if (e.Type != EventType.KeyDown)
            return true;

        switch (e.Key)
        {
            case DoomKey.Y or DoomKey.Enter or DoomKey.Space:
            {
                endCount = 0;

                var sfx = Menu.Options.GameMode == GameMode.Commercial
                    ? doom2QuitSoundList[random.Next() % doom2QuitSoundList.Length]
                    : doomQuitSoundList[random.Next() % doomQuitSoundList.Length];
                Menu.StartSound(sfx);
                break;
            }
            case DoomKey.N or DoomKey.Escape:
                Menu.Close();
                Menu.StartSound(Sfx.SWTCHX);
                break;
        }

        return true;
    }

    public override void Update()
    {
        if (endCount != -1)
            endCount++;

        if (endCount == 50)
            app.Quit();
    }
}