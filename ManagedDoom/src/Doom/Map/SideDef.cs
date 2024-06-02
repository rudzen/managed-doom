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
using System.Buffers;

namespace ManagedDoom
{
    public sealed class SideDef
    {
        private static readonly int dataSize = 30;

        public SideDef(
            Fixed textureOffset,
            Fixed rowOffset,
            int topTexture,
            int bottomTexture,
            int middleTexture,
            Sector sector)
        {
            this.TextureOffset = textureOffset;
            this.RowOffset = rowOffset;
            this.TopTexture = topTexture;
            this.BottomTexture = bottomTexture;
            this.MiddleTexture = middleTexture;
            this.Sector = sector;
        }

        private static SideDef FromData(ReadOnlySpan<byte> data, ITextureLookup textures, ReadOnlySpan<Sector> sectors)
        {
            var textureOffset = BitConverter.ToInt16(data[..2]);
            var rowOffset = BitConverter.ToInt16(data.Slice(2, 2));
            var topTextureName = DoomInterop.ToString(data.Slice(4, 8));
            var bottomTextureName = DoomInterop.ToString(data.Slice(12, 8));
            var middleTextureName = DoomInterop.ToString(data.Slice(20, 8));
            var sectorNum = BitConverter.ToInt16(data.Slice(28, 2));

            return new SideDef(
                Fixed.FromInt(textureOffset),
                Fixed.FromInt(rowOffset),
                textures.GetNumber(topTextureName),
                textures.GetNumber(bottomTextureName),
                textures.GetNumber(middleTextureName),
                sectorNum != -1 ? sectors[sectorNum] : null);
        }

        public static SideDef[] FromWad(Wad wad, int lump, ITextureLookup textures, ReadOnlySpan<Sector> sectors)
        {
            var lumpSize = wad.GetLumpSize(lump);
            if (lumpSize % dataSize != 0)
                throw new Exception();

            var lumpData = ArrayPool<byte>.Shared.Rent(lumpSize);

            try
            {
                var lumpBuffer = lumpData.AsSpan(0, lumpSize);
                wad.ReadLump(lump, lumpBuffer);
                
                var count = lumpSize / dataSize;
                var sides = new SideDef[count];

                for (var i = 0; i < count; i++)
                {
                    var offset = dataSize * i;
                    sides[i] = FromData(lumpBuffer[offset..], textures, sectors);
                }

                return sides;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(lumpData);
            }
        }

        public Fixed TextureOffset { get; set; }

        public Fixed RowOffset { get; set; }

        public int TopTexture { get; set; }

        public int BottomTexture { get; set; }

        public int MiddleTexture { get; set; }

        public Sector Sector { get; }
    }
}
