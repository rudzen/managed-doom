//
// Copyright (C) 1993-1996 Id Software, Inc.
// Copyright (C) 2019-2020 Nobuaki Tanaka
// Copyright (C)      2025 Rudy Alex Kohn
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
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using ManagedDoom.Doom.Common;
using ManagedDoom.Doom.Info;
using ManagedDoom.Doom.Wad;

namespace ManagedDoom.Doom.Graphics;

public static class GraphicsFactory
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Flat CreateFlat(LumpInfo lumpInfo)
    {
        return new Flat(lumpInfo.Name, lumpInfo.Data);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Palette CreatePalette(Wad.Wad wad)
    {
        try
        {
            Console.Write("Load palette: ");
            var start = Stopwatch.GetTimestamp();

            var data = wad.ReadLump("PLAYPAL");

            var count = data.Length / (3 * 256);
            var palettes = new uint[count][];
            for (var i = 0; i < palettes.Length; i++)
                palettes[i] = new uint[256];

            var end = Stopwatch.GetElapsedTime(start);
            Console.WriteLine($"OK [{end}]");

            return new Palette(data, palettes);
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed");
            ExceptionDispatchInfo.Throw(e);
        }

        return null!;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Texture CreateTexture(ReadOnlySpan<byte> data, int offset, ReadOnlySpan<Patch> patchLookup)
    {
        const int texturePatchDataSize = 10;

        var root = data[offset..];
        var name = DoomInterop.ToString(root);
        var masked = BitConverter.ToInt32(root[8..]);
        var width = BitConverter.ToInt16(root[12..]);
        var height = BitConverter.ToInt16(root[14..]);
        var patchCount = BitConverter.ToInt16(root[20..]);
        var patches = new TexturePatch[patchCount];
        var baseOffset = offset + 22;

        for (var i = 0; i < patches.Length; i++)
        {
            var patchOffset = baseOffset + texturePatchDataSize * i;
            patches[i] = CreateTexturePatch(data[patchOffset..], patchLookup);
        }

        return new Texture(
            name,
            masked != 0,
            width,
            height,
            patches);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static TexturePatch CreateTexturePatch(ReadOnlySpan<byte> data, ReadOnlySpan<Patch> patches)
    {
        var originX = BitConverter.ToInt16(data);
        var originY = BitConverter.ToInt16(data[2..]);
        var patchNum = BitConverter.ToInt16(data[4..]);

        return new TexturePatch(
            originX,
            originY,
            patches[patchNum]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Patch CreatePatch(string name, byte[] data)
    {
        var width = BitConverter.ToInt16(data, 0);
        var height = BitConverter.ToInt16(data, 2);
        var leftOffset = BitConverter.ToInt16(data, 4);
        var topOffset = BitConverter.ToInt16(data, 6);

        PadPatchData(ref data, width);

        var columns = new Column[width][];
        var cs = new List<Column>(width);
        for (var x = 0; x < width; x++)
        {
            cs.Clear();
            var p = BitConverter.ToInt32(data, 8 + 4 * x);
            while (true)
            {
                var topDelta = data[p];
                if (topDelta == Column.Last)
                    break;
                var length = data[p + 1];
                var offset = p + 3;
                cs.Add(new Column(topDelta, data, offset, length));
                p += length + 4;
            }

            columns[x] = [.. cs];
        }

        return new Patch(
            name,
            width,
            height,
            leftOffset,
            topOffset,
            columns);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Patch CreatePatch(Wad.Wad wad, string name)
    {
        return CreatePatch(name, wad.ReadLump(name));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void PadPatchData(ref byte[] data, int width)
    {
        var need = 0;
        for (var x = 0; x < width; x++)
        {
            var p = BitConverter.ToInt32(data, 8 + 4 * x);
            while (true)
            {
                var topDelta = data[p];
                if (topDelta == Column.Last)
                    break;
                var length = data[p + 1];
                var offset = p + 3;
                need = System.Math.Max(offset + 128, need);
                p += length + 4;
            }
        }

        if (data.Length < need)
            Array.Resize(ref data, need);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TextureAnimationInfo[] CreateTextureAnimations(ILookup<Texture> textures, IFlatLookup flats)
    {
        Console.Write("Load texture animation info: ");
        var list = new List<TextureAnimationInfo>(DoomInfo.TextureAnimation.Length);
        var start = Stopwatch.GetTimestamp();

        try
        {
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

            var end = Stopwatch.GetElapsedTime(start);
            Console.WriteLine($"OK [{end}]");
            return [.. list];
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed");
            ExceptionDispatchInfo.Throw(e);
        }

        return [];
    }
}