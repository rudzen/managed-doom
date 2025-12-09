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
using System.Runtime.InteropServices;
using ManagedDoom.Doom.Common;
using ManagedDoom.Doom.Info;
using ManagedDoom.Doom.Wad;

namespace ManagedDoom.Doom.Graphics;

public static class GraphicsFactory
{
    public static SpriteLookup CreateSpriteLookup(Wad.Wad wad)
    {
        Console.Write("Load sprites: ");
        var start = Stopwatch.GetTimestamp();

        var temp = new Dictionary<string, List<SpriteInfo>>();
        var tempLookup = temp.GetAlternateLookup<ReadOnlySpan<char>>();

        for (var i = 0; i < (int)Sprite.Count; i++)
            temp.TryAdd(DoomInfo.SpriteNames[i], []);

        var cache = new Dictionary<int, Patch>();
        var sprites = EnumerateSprites(wad);
        var spritesSpan = CollectionsMarshal.AsSpan(sprites);

        foreach (var lumpNumber in spritesSpan)
        {
            var lumpInfo = wad.LumpInfos[lumpNumber];
            var lumpName = lumpInfo.Name.AsSpan();
            var name = lumpName[..4];

            if (!tempLookup.TryGetValue(name, out var list))
                continue;

            var frameIndex = lumpName[4] - 'A';
            var rotationIndex = lumpName[5] - '0';

            while (list.Count < frameIndex + 1)
                list.Add(new SpriteInfo(new Patch[8], new bool[8]));

            var patch = CachedRead(lumpNumber, wad, cache);

            if (rotationIndex == 0)
            {
                for (var i = 0; i < 8; i++)
                {
                    if (list[frameIndex].Patches[i] == null)
                    {
                        list[frameIndex].Patches[i] = patch;
                        list[frameIndex].Flip[i] = false;
                    }
                }
            }
            else
            {
                if (list[frameIndex].Patches[rotationIndex - 1] == null)
                {
                    list[frameIndex].Patches[rotationIndex - 1] = patch;
                    list[frameIndex].Flip[rotationIndex - 1] = false;
                }
            }

            if (lumpName.Length == 8)
            {
                frameIndex = lumpName[6] - 'A';
                rotationIndex = lumpName[7] - '0';

                while (list.Count < frameIndex + 1)
                    list.Add(new SpriteInfo(new Patch[8], new bool[8]));

                if (rotationIndex == 0)
                {
                    for (var i = 0; i < 8; i++)
                    {
                        if (list[frameIndex].Patches[i] == null)
                        {
                            list[frameIndex].Patches[i] = patch;
                            list[frameIndex].Flip[i] = true;
                        }
                    }
                }
                else
                {
                    if (list[frameIndex].Patches[rotationIndex - 1] == null)
                    {
                        list[frameIndex].Patches[rotationIndex - 1] = patch;
                        list[frameIndex].Flip[rotationIndex - 1] = true;
                    }
                }
            }
        }

        var spriteDefs = new SpriteDef[(int)Sprite.Count];

        try
        {
            for (var i = 0; i < spriteDefs.Length; i++)
            {
                var list = temp[DoomInfo.SpriteNames[i]];

                var frames = new SpriteFrame[list.Count];
                for (var j = 0; j < frames.Length; j++)
                {
                    var currentList = list[j];
                    currentList.Patches.CheckCompletion();
                    var hasRotation = currentList.Patches.HasRotation();

                    var frame = new SpriteFrame(hasRotation, currentList.Patches, currentList.Flip);
                    frames[j] = frame;
                }

                spriteDefs[i] = new SpriteDef(frames);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed");
            ExceptionDispatchInfo.Throw(e);
        }

        var end = Stopwatch.GetElapsedTime(start);
        Console.WriteLine($"OK ({cache.Count} sprites) [{end}]");

        return new SpriteLookup(spriteDefs);

        static List<int> EnumerateSprites(Wad.Wad wad)
        {
            var spriteSection = false;
            var result = new List<int>(2024);

            for (var lump = wad.LumpInfos.Length - 1; lump >= 0; lump--)
            {
                var name = wad.LumpInfos[lump].Name.AsSpan();

                if (name.StartsWith('S'))
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
                    if (wad.LumpInfos[lump].Data!.Length > 0)
                        result.Add(lump);
                }
            }

            return result;
        }

        static Patch CachedRead(int lump, Wad.Wad wad, Dictionary<int, Patch> cache)
        {
            ref var value = ref CollectionsMarshal.GetValueRefOrAddDefault(cache, lump, out var exists);
            if (exists) return value!;
            var name = wad.LumpInfos[lump].Name;
            return value = CreatePatch(name, wad.ReadLump(lump));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ColorMap CreateColorMap(Wad.Wad wad)
    {
        const string lump = "COLORMAP";
        const int blockSize = 256;

        Console.Write("Load color map: ");

        var start = Stopwatch.GetTimestamp();

        try
        {
            var (lumpNumber, lumpSize) = wad.GetLumpNumberAndSize(lump);
            var num = lumpSize / blockSize;

            var lumpData = wad.GetLumpData(lumpNumber);

            var data = new byte[num][];
            for (var i = 0; i < data.Length; i++)
                data[i] = lumpData.Slice(blockSize * i, blockSize).ToArray();

            var end = Stopwatch.GetElapsedTime(start);
            Console.WriteLine($"OK ({num} maps) [{end}]");

            return new ColorMap(data);
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed");
            ExceptionDispatchInfo.Throw(e);
        }

        return null!;
    }

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
    public static Texture CreateTexture(
        string name,
        bool masked,
        int width,
        int height,
        params ReadOnlySpan<TexturePatch> patches)
    {
        var compositePatch = CreateCompositePatch(name, width, height, patches);
        return new Texture(masked, compositePatch);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Texture CreateTexture(ReadOnlySpan<byte> data, int offset, ReadOnlySpan<Patch> patchLookup)
    {
        const int texturePatchDataSize = 10;

        var root = data[offset..];
        var name = DoomInterop.ToString(root);
        var masked = BitConverter.ToInt32(root[8..]) != 0;
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

        var compositePatch = CreateCompositePatch(name, width, height, patches);

        return new Texture(masked, compositePatch);
    }

    private static Patch CreateCompositePatch(string name, int width, int height, params ReadOnlySpan<TexturePatch> patches)
    {
        var patchCount = new int[width];
        var columns = new Column[width][];
        var compositeColumnCount = 0;

        foreach (var (left, _, patch) in patches)
        {
            var right = left + patch.Width;

            var start = System.Math.Max(left, 0);
            var end = System.Math.Min(right, width);

            for (var x = start; x < end; x++)
            {
                patchCount[x]++;
                if (patchCount[x] == 2)
                    compositeColumnCount++;

                columns[x] = patch.Columns[x - left];
            }
        }

        var padding = System.Math.Max(128 - height, 0);
        var data = new byte[height * compositeColumnCount + padding];
        var i = 0;
        for (var x = 0; x < width; x++)
        {
            if (patchCount[x] == 0)
                columns[x] = [];
            else if (patchCount[x] >= 2)
            {
                var column = new Column(0, data, height * i, height);

                foreach (var patch in patches)
                {
                    var px = x - patch.OriginX;
                    if (px < 0 || px >= patch.Patch.Width)
                        continue;

                    var patchColumn = patch.Patch.Columns[px];
                    DrawColumnInCache(
                        source: patchColumn,
                        destination: column.Data,
                        destinationOffset: column.Offset,
                        destinationY: patch.OriginY,
                        destinationHeight: height
                    );
                }

                columns[x] = [column];

                i++;
            }
        }

        return new Patch(name, width, height, 0, 0, columns);

        static void DrawColumnInCache(
            ReadOnlySpan<Column> source,
            Span<byte> destination,
            int destinationOffset,
            int destinationY,
            int destinationHeight)
        {
            foreach (var column in source)
            {
                var sourceIndex = column.Offset;
                var destinationIndex = destinationOffset + destinationY + column.TopDelta;
                var length = column.Length;

                var topExceedance = -(destinationY + column.TopDelta);
                if (topExceedance > 0)
                {
                    sourceIndex += topExceedance;
                    destinationIndex += topExceedance;
                    length -= topExceedance;
                }

                var bottomExceedance = destinationY + column.TopDelta + column.Length - destinationHeight;

                if (bottomExceedance > 0)
                    length -= bottomExceedance;

                if (length > 0)
                    column.Data.AsSpan(sourceIndex, length).CopyTo(destination[destinationIndex..]);
            }
        }
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