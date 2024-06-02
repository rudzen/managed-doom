﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace ManagedDoom
{
    public sealed class DummyTextureLookup : ITextureLookup
    {
        private List<Texture> textures;
        private Dictionary<string, Texture> nameToTexture;
        private Dictionary<string, int> nameToNumber;

        private int[] switchList;

        public DummyTextureLookup(Wad wad)
        {
            InitLookup(wad);
            InitSwitchList();
        }

        private void InitLookup(Wad wad)
        {
            textures = new List<Texture>();
            nameToTexture = new Dictionary<string, Texture>();
            nameToNumber = new Dictionary<string, int>();

            for (var n = 1; n <= 2; n++)
            {
                var lumpNumber = wad.GetLumpNumber("TEXTURE" + n);
                if (lumpNumber == -1)
                    break;

                var data = wad.ReadLump(lumpNumber);
                var count = BitConverter.ToInt32(data, 0);
                for (var i = 0; i < count; i++)
                {
                    var offset = BitConverter.ToInt32(data, 4 + 4 * i);
                    var name = Texture.GetName(data, offset);
                    var height = Texture.GetHeight(data, offset);
                    var texture = DummyData.GetTexture(height);
                    nameToNumber.TryAdd(name, textures.Count);
                    textures.Add(texture);
                    nameToTexture.TryAdd(name, texture);
                }
            }
        }

        private void InitSwitchList()
        {
            var list = new List<int>();
            foreach (var (tex1, tex2) in DoomInfo.SwitchNames)
            {
                var texNum1 = GetNumber(tex1);
                var texNum2 = GetNumber(tex2);
                if (texNum1 != -1 && texNum2 != -1)
                {
                    list.Add(texNum1);
                    list.Add(texNum2);
                }
            }
            switchList = list.ToArray();
        }

        public int GetNumber(string name)
        {
            if (name[0] == '-')
                return 0;

            if (nameToNumber.TryGetValue(name, out var number))
                return number;

            return -1;
        }

        public IEnumerator<Texture> GetEnumerator()
        {
            return textures.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return textures.GetEnumerator();
        }

        public int Count => textures.Count;
        public Texture this[int num] => textures[num];
        public Texture this[string name] => nameToTexture[name];
        public int[] SwitchList => switchList;
    }
}
