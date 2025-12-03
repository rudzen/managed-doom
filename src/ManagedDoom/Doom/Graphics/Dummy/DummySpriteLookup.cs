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
using System.Collections.Generic;
using System.Linq;
using ManagedDoom.Doom.Info;

namespace ManagedDoom.Doom.Graphics.Dummy;

public sealed class DummySpriteLookup : ISpriteLookup
{
    private readonly SpriteDef[] spriteDefs;

    public DummySpriteLookup(Wad.Wad wad)
    {
        var temp = new Dictionary<string, List<SpriteInfo>>();
        var tempLookup = temp.GetAlternateLookup<ReadOnlySpan<char>>();

        for (var i = 0; i < (int)Sprite.Count; i++)
            tempLookup.TryAdd(DoomInfo.SpriteNames[i], []);

        foreach (var lumpNumber in EnumerateSprites(wad))
        {
            var lump = wad.LumpInfos[lumpNumber];
            var lumpName = lump.Name.AsSpan();
            var namePrefix = lumpName[..4];

            if (!tempLookup.TryGetValue(namePrefix, out var list))
                continue;

            var frame = lumpName[4] - 'A';
            var rotation = lumpName[5] - '0';

            while (list.Count < frame + 1)
                list.Add(new SpriteInfo());

            if (rotation == 0)
            {
                for (var i = 0; i < list[frame].Patches.Length; i++)
                {
                    if (list[frame].Patches[i] is null)
                    {
                        list[frame].Patches[i] = DummyData.GetPatch();
                        list[frame].Flip[i] = false;
                    }
                }
            }
            else
            {
                if (list[frame].Patches[rotation - 1] is null)
                {
                    list[frame].Patches[rotation - 1] = DummyData.GetPatch();
                    list[frame].Flip[rotation - 1] = false;
                }
            }

            if (lumpName.Length != 8) continue;

            frame = lumpName[6] - 'A';
            rotation = lumpName[7] - '0';

            while (list.Count < frame + 1)
                list.Add(new SpriteInfo());

            if (rotation == 0)
            {
                for (var i = 0; i < 8; i++)
                {
                    if (list[frame].Patches[i] is null)
                    {
                        list[frame].Patches[i] = DummyData.GetPatch();
                        list[frame].Flip[i] = true;
                    }
                }
            }
            else if (list[frame].Patches[rotation - 1] is null)
            {
                list[frame].Patches[rotation - 1] = DummyData.GetPatch();
                list[frame].Flip[rotation - 1] = true;
            }
        }

        spriteDefs = new SpriteDef[(int)Sprite.Count];
        for (var i = 0; i < spriteDefs.Length; i++)
        {
            var list = tempLookup[DoomInfo.SpriteNames[i]];

            var frames = new SpriteFrame[list.Count];
            for (var j = 0; j < frames.Length; j++)
            {
                list[j].CheckCompletion();

                var frame = new SpriteFrame(list[j].HasRotation(), list[j].Patches, list[j].Flip);
                frames[j] = frame;
            }

            spriteDefs[i] = new SpriteDef(frames);
        }
    }

    private static IEnumerable<int> EnumerateSprites(Wad.Wad wad)
    {
        var spriteSection = false;

        for (var lumpNumber = wad.LumpInfos.Length - 1; lumpNumber >= 0; lumpNumber--)
        {
            var lump = wad.LumpInfos[lumpNumber];
            var name = lump.Name.AsSpan();

            if (name[0] == 'S')
            {
                if (name.EndsWith("_END"))
                {
                    spriteSection = true;
                    continue;
                }

                if (name.EndsWith("_START"))
                {
                    spriteSection = false;
                    continue;
                }
            }

            if (spriteSection)
            {
                var length = lump.Data?.Length ?? -1;
                if (length > 0)
                    yield return lumpNumber;
            }
        }
    }

    public SpriteDef this[Sprite sprite] => spriteDefs[(int)sprite];

    private sealed class SpriteInfo
    {
        public readonly Patch?[] Patches = new Patch[8];
        public readonly bool[] Flip = new bool[8];

        public void CheckCompletion()
        {
            if (Patches.Any(patch => patch == null))
                throw new Exception("Missing sprite!");
        }

        public bool HasRotation()
        {
            var front = Patches[0];
            return Patches.Any(patch => patch != front);
        }
    }
}