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

namespace ManagedDoom.UserInput
{
    public static class DoomMouseButtonEx
    {
        public static string ToString(DoomMouseButton button)
        {
            switch (button)
            {
                case DoomMouseButton.Mouse1:
                    return "mouse1";
                case DoomMouseButton.Mouse2:
                    return "mouse2";
                case DoomMouseButton.Mouse3:
                    return "mouse3";
                case DoomMouseButton.Mouse4:
                    return "mouse4";
                case DoomMouseButton.Mouse5:
                    return "mouse5";
                default:
                    return "unknown";
            }
        }

        public static DoomMouseButton Parse(ReadOnlySpan<char> value)
        {
            return value switch
            {
                "mouse1" => DoomMouseButton.Mouse1,
                "mouse2" => DoomMouseButton.Mouse2,
                "mouse3" => DoomMouseButton.Mouse3,
                "mouse4" => DoomMouseButton.Mouse4,
                "mouse5" => DoomMouseButton.Mouse5,
                _        => DoomMouseButton.Unknown
            };
        }
    }
}
