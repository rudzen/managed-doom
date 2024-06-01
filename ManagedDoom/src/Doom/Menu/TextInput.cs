//
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
using System.Linq;

namespace ManagedDoom
{
    public sealed class TextInput
    {
        private readonly List<char> text;
        private readonly Action<IReadOnlyList<char>> typed;
        private readonly Action<IReadOnlyList<char>> finished;
        private readonly Action canceled;

        public TextInput(
            IReadOnlyList<char> initialText,
            Action<IReadOnlyList<char>> typed,
            Action<IReadOnlyList<char>> finished,
            Action canceled)
        {
            this.text = initialText.ToList();
            this.typed = typed;
            this.finished = finished;
            this.canceled = canceled;

            State = TextInputState.Typing;
        }

        public bool DoEvent(DoomEvent e)
        {
            var ch = e.Key.GetChar();
            if (ch != 0)
            {
                text.Add(ch);
                typed(text);
                return true;
            }

            if (e is { Key: DoomKey.Backspace, Type: EventType.KeyDown })
            {
                if (text.Count > 0)
                {
                    text.RemoveAt(text.Count - 1);
                }
                typed(text);
                return true;
            }

            if (e is { Key: DoomKey.Enter, Type: EventType.KeyDown })
            {
                finished(text);
                State = TextInputState.Finished;
                return true;
            }

            if (e is { Key: DoomKey.Escape, Type: EventType.KeyDown })
            {
                canceled();
                State = TextInputState.Canceled;
                return true;
            }

            return true;
        }

        public IReadOnlyList<char> Text => text;
        public TextInputState State { get; private set; }
    }
}
