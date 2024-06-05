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
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using ManagedDoom.Doom.Info;

namespace ManagedDoom.Doom.Graphics
{
    public sealed class TextureAnimation
    {
        public TextureAnimation(ITextureLookup textures, IFlatLookup flats)
        {
            try
            {
                Console.Write("Load texture animation info: ");
                var start = Stopwatch.GetTimestamp();

                var list = new List<TextureAnimationInfo>(DoomInfo.TextureAnimation.Length);

                foreach (var animDef in DoomInfo.TextureAnimation.AsSpan())
                {
                    int picNum;
                    int basePic;
                    if (animDef.IsTexture)
                    {
                        if (textures.GetNumber(animDef.StartName) == -1)
                            continue;

                        picNum = textures.GetNumber(animDef.EndName);
                        basePic = textures.GetNumber(animDef.StartName);
                    }
                    else
                    {
                        if (flats.GetNumber(animDef.StartName) == -1)
                            continue;

                        picNum = flats.GetNumber(animDef.EndName);
                        basePic = flats.GetNumber(animDef.StartName);
                    }

                    var anim = new TextureAnimationInfo(
                        animDef.IsTexture,
                        picNum,
                        basePic,
                        picNum - basePic + 1,
                        animDef.Speed);

                    if (anim.NumPics < 2)
                        throw new Exception($"Bad animation cycle from {animDef.StartName} to {animDef.EndName}!");

                    list.Add(anim);
                }

                Animations = list.ToArray();

                Console.WriteLine($"OK [{Stopwatch.GetElapsedTime(start)}]");
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed");
                ExceptionDispatchInfo.Throw(e);
            }
        }

        public TextureAnimationInfo[] Animations { get; }
    }
}
