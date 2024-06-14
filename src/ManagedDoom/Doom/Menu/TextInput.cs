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
using System.Text;
using ManagedDoom.Doom.Event;
using ManagedDoom.UserInput;

namespace ManagedDoom.Doom.Menu;

public sealed class TextInput(
    string initialText,
    Action<StringBuilder> typed,
    Action<StringBuilder> finished,
    Action canceled)
{
    public StringBuilder Text { get; } = new(initialText);
    public TextInputState State { get; private set; } = TextInputState.Typing;

    public bool DoEvent(in DoomEvent e)
    {
        var ch = e.Key.GetChar();
        if (ch != 0)
        {
            Text.Append(ch);
            typed(Text);
            return true;
        }

        switch (e)
        {
            case { Key: DoomKey.Backspace, Type: EventType.KeyDown }:
            {
                if (Text.Length > 0)
                    Text.Remove(Text.Length - 1, 1);

                typed(Text);
                return true;
            }
            case { Key: DoomKey.Enter, Type: EventType.KeyDown }:
                finished(Text);
                State = TextInputState.Finished;
                return true;
            case { Key: DoomKey.Escape, Type: EventType.KeyDown }:
                canceled();
                State = TextInputState.Canceled;
                return true;
            default:
                return true;
        }
    }
}