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
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ManagedDoom.UserInput;

public enum DoomKey : sbyte
{
    Unknown = -1,
    A = 0,
    B,
    C,
    D,
    E,
    F,
    G,
    H,
    I,
    J,
    K,
    L,
    M,
    N,
    O,
    P,
    Q,
    R,
    S,
    T,
    U,
    V,
    W,
    X,
    Y,
    Z,
    Num0,
    Num1,
    Num2,
    Num3,
    Num4,
    Num5,
    Num6,
    Num7,
    Num8,
    Num9,
    Escape,
    LControl,
    LShift,
    LAlt,
    LSystem,
    RControl,
    RShift,
    RAlt,
    RSystem,
    Menu,
    LBracket,
    RBracket,
    Semicolon,
    Comma,
    Period,
    Quote,
    Slash,
    Backslash,
    Tilde,
    Equal,
    Hyphen,
    Space,
    Enter,
    Backspace,
    Tab,
    PageUp,
    PageDown,
    End,
    Home,
    Insert,
    Delete,
    Add,
    Subtract,
    Multiply,
    Divide,
    Left,
    Right,
    Up,
    Down,
    Numpad0,
    Numpad1,
    Numpad2,
    Numpad3,
    Numpad4,
    Numpad5,
    Numpad6,
    Numpad7,
    Numpad8,
    Numpad9,
    F1,
    F2,
    F3,
    F4,
    F5,
    F6,
    F7,
    F8,
    F9,
    F10,
    F11,
    F12,
    F13,
    F14,
    F15,
    Pause,
    Count
}

public static class DoomKeyEx
{
    private static readonly FrozenDictionary<string, DoomKey> KeyMap;
    private static readonly FrozenDictionary<string, DoomKey>.AlternateLookup<ReadOnlySpan<char>> KeyMapLookup;

    private static readonly FrozenDictionary<DoomKey, string> ReverseKeyMap;

    static DoomKeyEx()
    {
        var keyMap = new Dictionary<string, DoomKey>((int)DoomKey.Count)
        {
            { "a", DoomKey.A },
            { "b", DoomKey.B },
            { "c", DoomKey.C },
            { "d", DoomKey.D },
            { "e", DoomKey.E },
            { "f", DoomKey.F },
            { "g", DoomKey.G },
            { "h", DoomKey.H },
            { "i", DoomKey.I },
            { "j", DoomKey.J },
            { "k", DoomKey.K },
            { "l", DoomKey.L },
            { "m", DoomKey.M },
            { "n", DoomKey.N },
            { "o", DoomKey.O },
            { "p", DoomKey.P },
            { "q", DoomKey.Q },
            { "r", DoomKey.R },
            { "s", DoomKey.S },
            { "t", DoomKey.T },
            { "u", DoomKey.U },
            { "v", DoomKey.V },
            { "w", DoomKey.W },
            { "x", DoomKey.X },
            { "y", DoomKey.Y },
            { "z", DoomKey.Z },
            { "num0", DoomKey.Num0 },
            { "num1", DoomKey.Num1 },
            { "num2", DoomKey.Num2 },
            { "num3", DoomKey.Num3 },
            { "num4", DoomKey.Num4 },
            { "num5", DoomKey.Num5 },
            { "num6", DoomKey.Num6 },
            { "num7", DoomKey.Num7 },
            { "num8", DoomKey.Num8 },
            { "num9", DoomKey.Num9 },
            { "escape", DoomKey.Escape },
            { "lcontrol", DoomKey.LControl },
            { "lshift", DoomKey.LShift },
            { "lalt", DoomKey.LAlt },
            { "lsystem", DoomKey.LSystem },
            { "rcontrol", DoomKey.RControl },
            { "rshift", DoomKey.RShift },
            { "ralt", DoomKey.RAlt },
            { "rsystem", DoomKey.RSystem },
            { "menu", DoomKey.Menu },
            { "lbracket", DoomKey.LBracket },
            { "rbracket", DoomKey.RBracket },
            { "semicolon", DoomKey.Semicolon },
            { "comma", DoomKey.Comma },
            { "period", DoomKey.Period },
            { "quote", DoomKey.Quote },
            { "slash", DoomKey.Slash },
            { "backslash", DoomKey.Backslash },
            { "tilde", DoomKey.Tilde },
            { "equal", DoomKey.Equal },
            { "hyphen", DoomKey.Hyphen },
            { "space", DoomKey.Space },
            { "enter", DoomKey.Enter },
            { "backspace", DoomKey.Backspace },
            { "tab", DoomKey.Tab },
            { "pageup", DoomKey.PageUp },
            { "pagedown", DoomKey.PageDown },
            { "end", DoomKey.End },
            { "home", DoomKey.Home },
            { "insert", DoomKey.Insert },
            { "delete", DoomKey.Delete },
            { "add", DoomKey.Add },
            { "subtract", DoomKey.Subtract },
            { "multiply", DoomKey.Multiply },
            { "divide", DoomKey.Divide },
            { "left", DoomKey.Left },
            { "right", DoomKey.Right },
            { "up", DoomKey.Up },
            { "down", DoomKey.Down },
            { "numpad0", DoomKey.Numpad0 },
            { "numpad1", DoomKey.Numpad1 },
            { "numpad2", DoomKey.Numpad2 },
            { "numpad3", DoomKey.Numpad3 },
            { "numpad4", DoomKey.Numpad4 },
            { "numpad5", DoomKey.Numpad5 },
            { "numpad6", DoomKey.Numpad6 },
            { "numpad7", DoomKey.Numpad7 },
            { "numpad8", DoomKey.Numpad8 },
            { "numpad9", DoomKey.Numpad9 },
            { "f1", DoomKey.F1 },
            { "f2", DoomKey.F2 },
            { "f3", DoomKey.F3 },
            { "f4", DoomKey.F4 },
            { "f5", DoomKey.F5 },
            { "f6", DoomKey.F6 },
            { "f7", DoomKey.F7 },
            { "f8", DoomKey.F8 },
            { "f9", DoomKey.F9 },
            { "f10", DoomKey.F10 },
            { "f11", DoomKey.F11 },
            { "f12", DoomKey.F12 },
            { "f13", DoomKey.F13 },
            { "f14", DoomKey.F14 },
            { "f15", DoomKey.F15 },
            { "pause", DoomKey.Pause }
        };
        KeyMap = keyMap.ToFrozenDictionary();
        KeyMapLookup = KeyMap.GetAlternateLookup<ReadOnlySpan<char>>();
        ReverseKeyMap = KeyMap.ToFrozenDictionary(pair => (pair.Value), pair => pair.Key);
    }

    public static char GetChar(this DoomKey key)
    {
        return key switch
        {
            <= DoomKey.Z                              => (char)('a' + (key - DoomKey.A)),
            <= DoomKey.Num9                           => (char)('0' + (key - DoomKey.Num0)),
            >= DoomKey.Numpad0 and <= DoomKey.Numpad9 => (char)('0' + (key - DoomKey.Numpad0)),
            _ => key switch
            {
                DoomKey.LBracket  => '[',
                DoomKey.RBracket  => ']',
                DoomKey.Semicolon => ';',
                DoomKey.Comma     => ',',
                DoomKey.Period    => '.',
                DoomKey.Quote     => '"',
                DoomKey.Slash     => '/',
                DoomKey.Backslash => '\\',
                DoomKey.Equal     => '=',
                DoomKey.Hyphen    => '-',
                DoomKey.Space     => ' ',
                DoomKey.Add       => '+',
                DoomKey.Subtract  => '-',
                DoomKey.Multiply  => '*',
                DoomKey.Divide    => '/',
                _                 => '\0'
            }
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToString(DoomKey key) => ReverseKeyMap.TryGetValue(key, out var value) ? value : "unknown";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DoomKey Parse(ReadOnlySpan<char> value) => KeyMapLookup.TryGetValue(value, out var key) ? key : DoomKey.Unknown;
}