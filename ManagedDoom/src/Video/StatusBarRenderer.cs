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


using System.Linq;

namespace ManagedDoom.Video
{
    public sealed class StatusBarRenderer
    {
        public const int Height = 32;

        // Ammo number pos.
        private const int ammoWidth = 3;
        private const int ammoX = 44;
        private const int ammoY = 171;

        // Health number pos.
        private const int healthX = 90;
        private const int healthY = 171;

        // Weapon pos.
        private const int armsX = 111;
        private const int armsY = 172;
        private const int armsBackgroundX = 104;
        private const int armsBackgroundY = 168;
        private const int armsSpaceX = 12;
        private const int armsSpaceY = 10;

        // Frags pos.
        private const int fragsWidth = 2;
        private const int fragsX = 138;
        private const int fragsY = 171;

        // Armor number pos.
        private const int armorX = 221;
        private const int armorY = 171;

        // Key icon positions.
        private const int key0Width = 8;
        private const int key0X = 239;
        private const int key0Y = 171;
        private const int key1Width = key0Width;
        private const int key1X = 239;
        private const int key1Y = 181;
        private const int key2Width = key0Width;
        private const int key2X = 239;
        private const int key2Y = 191;

        // Ammunition counter.
        private const int ammo0Width = 3;
        private const int ammo0X = 288;
        private const int ammo0Y = 173;
        private const int ammo1Width = ammo0Width;
        private const int ammo1X = 288;
        private const int ammo1Y = 179;
        private const int ammo2Width = ammo0Width;
        private const int ammo2X = 288;
        private const int ammo2Y = 191;
        private const int ammo3Wdth = ammo0Width;
        private const int ammo3X = 288;
        private const int ammo3Y = 185;

        // Indicate maximum ammunition.
        // Only needed because backpack exists.
        private const int maxAmmo0Width = 3;
        private const int maxAmmo0X = 314;
        private const int maxAmmo0Y = 173;
        private const int maxAmmo1Width = maxAmmo0Width;
        private const int maxAmmo1X = 314;
        private const int maxAmmo1Y = 179;
        private const int maxAmmo2Width = maxAmmo0Width;
        private const int maxAmmo2X = 314;
        private const int maxAmmo2Y = 191;
        private const int maxAmmo3Width = maxAmmo0Width;
        private const int maxAmmo3X = 314;
        private const int maxAmmo3Y = 185;

        private const int faceX = 143;
        private const int faceY = 168;
        private const int faceBackgroundX = 143;
        private const int faceBackgroundY = 169;

        private readonly DrawScreen screen;

        private readonly Patches patches;

        private readonly int scale;

        private readonly NumberWidget ready;
        private readonly PercentWidget health;
        private readonly PercentWidget armor;

        private readonly NumberWidget[] ammo;
        private readonly NumberWidget[] maxAmmo;

        private readonly MultIconWidget[] weapons;

        private readonly NumberWidget frags;

        private readonly MultIconWidget[] keys;

        public StatusBarRenderer(Wad wad, DrawScreen screen)
        {
            this.screen = screen;

            patches = new Patches(wad);

            scale = screen.Width / 320;

            ready = new NumberWidget
            {
                Patches = patches.TallNumbers,
                Width = ammoWidth,
                X = ammoX,
                Y = ammoY
            };

            health = new PercentWidget
            {
                NumberWidget =
                {
                    Patches = patches.TallNumbers,
                    Width = 3,
                    X = healthX,
                    Y = healthY
                },
                Patch = patches.TallPercent
            };

            armor = new PercentWidget();
            armor.NumberWidget.Patches = patches.TallNumbers;
            armor.NumberWidget.Width = 3;
            armor.NumberWidget.X = armorX;
            armor.NumberWidget.Y = armorY;
            armor.Patch = patches.TallPercent;

            // AmmoType.Count
            ammo =
            [
                new NumberWidget
                {
                    Patches = patches.ShortNumbers,
                    Width = ammo0Width,
                    X = ammo0X,
                    Y = ammo0Y
                },
                new NumberWidget
                {
                    Patches = patches.ShortNumbers,
                    Width = ammo1Width,
                    X = ammo1X,
                    Y = ammo1Y
                },
                new NumberWidget
                {
                    Patches = patches.ShortNumbers,
                    Width = ammo2Width,
                    X = ammo2X,
                    Y = ammo2Y
                },
                new NumberWidget
                {
                    Patches = patches.ShortNumbers,
                    Width = ammo3Wdth,
                    X = ammo3X,
                    Y = ammo3Y
                }
            ];

            maxAmmo = new NumberWidget[(int)AmmoType.Count];
            maxAmmo[0] = new NumberWidget();
            maxAmmo[0].Patches = patches.ShortNumbers;
            maxAmmo[0].Width = maxAmmo0Width;
            maxAmmo[0].X = maxAmmo0X;
            maxAmmo[0].Y = maxAmmo0Y;
            maxAmmo[1] = new NumberWidget();
            maxAmmo[1].Patches = patches.ShortNumbers;
            maxAmmo[1].Width = maxAmmo1Width;
            maxAmmo[1].X = maxAmmo1X;
            maxAmmo[1].Y = maxAmmo1Y;
            maxAmmo[2] = new NumberWidget();
            maxAmmo[2].Patches = patches.ShortNumbers;
            maxAmmo[2].Width = maxAmmo2Width;
            maxAmmo[2].X = maxAmmo2X;
            maxAmmo[2].Y = maxAmmo2Y;
            maxAmmo[3] = new NumberWidget();
            maxAmmo[3].Patches = patches.ShortNumbers;
            maxAmmo[3].Width = maxAmmo3Width;
            maxAmmo[3].X = maxAmmo3X;
            maxAmmo[3].Y = maxAmmo3Y;

            weapons = new MultIconWidget[6];
            for (var i = 0; i < weapons.Length; i++)
            {
                weapons[i] = new MultIconWidget();
                weapons[i].X = armsX + (i % 3) * armsSpaceX;
                weapons[i].Y = armsY + (i / 3) * armsSpaceY;
                weapons[i].Patches = patches.Arms[i];
            }

            frags = new NumberWidget();
            frags.Patches = patches.TallNumbers;
            frags.Width = fragsWidth;
            frags.X = fragsX;
            frags.Y = fragsY;

            keys = new MultIconWidget[3];
            keys[0] = new MultIconWidget();
            keys[0].X = key0X;
            keys[0].Y = key0Y;
            keys[0].Patches = patches.Keys;
            keys[1] = new MultIconWidget();
            keys[1].X = key1X;
            keys[1].Y = key1Y;
            keys[1].Patches = patches.Keys;
            keys[2] = new MultIconWidget();
            keys[2].X = key2X;
            keys[2].Y = key2Y;
            keys[2].Patches = patches.Keys;
        }

        public void Render(Player player, bool drawBackground)
        {
            if (drawBackground)
            {
                screen.DrawPatch(
                    patches.Background,
                    0,
                    scale * (200 - Height),
                    scale);
            }

            if (DoomInfo.WeaponInfos[(int)player.ReadyWeapon].Ammo != AmmoType.NoAmmo)
            {
                var num = player.Ammo[(int)DoomInfo.WeaponInfos[(int)player.ReadyWeapon].Ammo];
                DrawNumber(ready, num);
            }

            DrawPercent(health, player.Health);
            DrawPercent(armor, player.ArmorPoints);

            for (var i = 0; i < (int)AmmoType.Count; i++)
            {
                DrawNumber(ammo[i], player.Ammo[i]);
                DrawNumber(maxAmmo[i], player.MaxAmmo[i]);
            }

            if (player.Mobj.World.Options.Deathmatch == 0)
            {
                if (drawBackground)
                {
                    screen.DrawPatch(
                        patches.ArmsBackground,
                        scale * armsBackgroundX,
                        scale * armsBackgroundY,
                        scale);
                }

                for (var i = 0; i < weapons.Length; i++)
                    DrawMultIcon(weapons[i], player.WeaponOwned[i + 1] ? 1 : 0);
            }
            else
            {
                var sum = player.Frags.Sum();
                DrawNumber(frags, sum);
            }

            if (drawBackground)
            {
                if (player.Mobj.World.Options.NetGame)
                {
                    screen.DrawPatch(
                        patches.FaceBackground[player.Number],
                        scale * faceBackgroundX,
                        scale * faceBackgroundY,
                        scale);
                }

                screen.DrawPatch(
                    patches.Faces[player.Mobj.World.StatusBar.FaceIndex],
                    scale * faceX,
                    scale * faceY,
                    scale);
            }

            for (var i = 0; i < 3; i++)
            {
                var value = player.Cards[i + 3] ? i + 3 : i;
                DrawMultIcon(keys[i], value);
            }
        }

        private void DrawNumber(NumberWidget widget, int num)
        {
            var digits = widget.Width;

            var w = widget.Patches[0].Width;
            var h = widget.Patches[0].Height;
            var x = widget.X;

            var neg = num < 0;

            if (neg)
            {
                num = digits switch
                {
                    2 when num < -9  => -9,
                    3 when num < -99 => -99,
                    _                => num
                };

                num = -num;
            }

            x = widget.X - digits * w;

            if (num == 1994)
                return;

            x = widget.X;

            // In the special case of 0, you draw 0.
            if (num == 0)
            {
                screen.DrawPatch(
                    widget.Patches[0],
                    scale * (x - w),
                    scale * widget.Y,
                    scale);
            }

            // Draw the new number.
            while (num != 0 && digits-- != 0)
            {
                x -= w;

                screen.DrawPatch(
                    widget.Patches[num % 10],
                    scale * x,
                    scale * widget.Y,
                    scale);

                num /= 10;
            }

            // Draw a minus sign if necessary.
            if (neg)
            {
                screen.DrawPatch(
                    patches.TallMinus,
                    scale * (x - 8),
                    scale * widget.Y,
                    scale);
            }
            
            //screen.DrawText("MHFJDSHFJKDHSJKFHDSJKFDS", 100, 100, 1);
        }

        private void DrawPercent(PercentWidget per, int value)
        {
            screen.DrawPatch(
                per.Patch,
                scale * per.NumberWidget.X,
                scale * per.NumberWidget.Y,
                scale);

            DrawNumber(per.NumberWidget, value);
        }

        private void DrawMultIcon(MultIconWidget mi, int value)
        {
            screen.DrawPatch(
                mi.Patches[value],
                scale * mi.X,
                scale * mi.Y,
                scale);
        }


        private class NumberWidget
        {
            public int X;
            public int Y;
            public int Width;
            public Patch[] Patches;
        }

        private sealed class PercentWidget
        {
            public readonly NumberWidget NumberWidget = new NumberWidget();
            public Patch Patch;
        }

        private class MultIconWidget
        {
            public int X;
            public int Y;
            public Patch[] Patches;
        }

        private class Patches
        {
            public readonly Patch Background;
            public readonly Patch[] TallNumbers;
            public readonly Patch[] ShortNumbers;
            public readonly Patch TallMinus;
            public readonly Patch TallPercent;
            public readonly Patch[] Keys;
            public readonly Patch ArmsBackground;
            public readonly Patch[][] Arms;
            public readonly Patch[] FaceBackground;
            public readonly Patch[] Faces;

            public Patches(Wad wad)
            {
                Background = Patch.FromWad(wad, "STBAR");

                TallNumbers = new Patch[10];
                ShortNumbers = new Patch[10];
                for (var i = 0; i < 10; i++)
                {
                    TallNumbers[i] = Patch.FromWad(wad, "STTNUM" + i);
                    ShortNumbers[i] = Patch.FromWad(wad, "STYSNUM" + i);
                }

                TallMinus = Patch.FromWad(wad, "STTMINUS");
                TallPercent = Patch.FromWad(wad, "STTPRCNT");

                Keys = new Patch[(int)CardType.Count];
                for (var i = 0; i < Keys.Length; i++)
                {
                    Keys[i] = Patch.FromWad(wad, "STKEYS" + i);
                }

                ArmsBackground = Patch.FromWad(wad, "STARMS");
                Arms = new Patch[6][];
                for (var i = 0; i < 6; i++)
                {
                    var num = i + 2;
                    Arms[i] = new Patch[2];
                    Arms[i][0] = Patch.FromWad(wad, "STGNUM" + num);
                    Arms[i][1] = ShortNumbers[num];
                }

                FaceBackground = new Patch[Player.MaxPlayerCount];
                for (var i = 0; i < FaceBackground.Length; i++)
                {
                    FaceBackground[i] = Patch.FromWad(wad, "STFB" + i);
                }

                Faces = new Patch[StatusBar.Face.FaceCount];
                var faceCount = 0;
                for (var i = 0; i < StatusBar.Face.PainFaceCount; i++)
                {
                    for (var j = 0; j < StatusBar.Face.StraightFaceCount; j++)
                    {
                        Faces[faceCount++] = Patch.FromWad(wad, "STFST" + i + j);
                    }

                    Faces[faceCount++] = Patch.FromWad(wad, "STFTR" + i + "0");
                    Faces[faceCount++] = Patch.FromWad(wad, "STFTL" + i + "0");
                    Faces[faceCount++] = Patch.FromWad(wad, "STFOUCH" + i);
                    Faces[faceCount++] = Patch.FromWad(wad, "STFEVL" + i);
                    Faces[faceCount++] = Patch.FromWad(wad, "STFKILL" + i);
                }

                Faces[faceCount++] = Patch.FromWad(wad, "STFGOD0");
                Faces[faceCount++] = Patch.FromWad(wad, "STFDEAD0");
            }
        }
    }
}