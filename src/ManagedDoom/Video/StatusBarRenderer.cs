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
using System.Runtime.CompilerServices;
using ManagedDoom.Doom.Game;
using ManagedDoom.Doom.Graphics;
using ManagedDoom.Doom.Info;
using ManagedDoom.Doom.Wad;
using ManagedDoom.Doom.World;
using ManagedDoom.Extensions;

namespace ManagedDoom.Video;

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

    private readonly NumberWidget[] ammo;
    private readonly PercentWidget armor;

    private readonly NumberWidget frags;
    private readonly PercentWidget health;

    private readonly MultIconWidget[] keys;
    private readonly NumberWidget[] maxAmmo;

    private readonly Patches patches;

    private readonly NumberWidget ready;

    private readonly int scale;

    private readonly DrawScreen screen;

    private readonly MultIconWidget[] weapons;

    public StatusBarRenderer(Wad wad, DrawScreen screen)
    {
        this.screen = screen;

        patches = new Patches(wad);

        scale = screen.Width / 320;

        ready = new NumberWidget(ammoX, ammoY, ammo0Width, patches.TallNumbers);

        health = new PercentWidget(
            NumberWidget: new NumberWidget(healthX, healthY, 3, patches.TallNumbers),
            Patch: patches.TallPercent
        );

        armor = new PercentWidget(
            NumberWidget: new NumberWidget(armorX, armorY, 3, patches.TallNumbers),
            Patch: patches.TallPercent
        );

        // AmmoType.Count
        ammo =
        [
            new NumberWidget
            (
                X: ammo0X,
                Y: ammo0Y,
                Width: ammo0Width,
                Patches: patches.ShortNumbers
            ),
            new NumberWidget
            (
                X: ammo1X,
                Y: ammo1Y,
                Width: ammo1Width,
                Patches: patches.ShortNumbers
            ),
            new NumberWidget
            (
                X: ammo2X,
                Y: ammo2Y,
                Width: ammo2Width,
                Patches: patches.ShortNumbers
            ),
            new NumberWidget
            (
                X: ammo3X,
                Y: ammo3Y,
                Width: ammo3Wdth,
                Patches: patches.ShortNumbers
            )
        ];

        maxAmmo =
        [
            new NumberWidget(maxAmmo0X, maxAmmo0Y, maxAmmo0Width, patches.ShortNumbers),
            new NumberWidget(maxAmmo1X, maxAmmo1Y, maxAmmo1Width, patches.ShortNumbers),
            new NumberWidget(maxAmmo2X, maxAmmo2Y, maxAmmo2Width, patches.ShortNumbers),
            new NumberWidget(maxAmmo3X, maxAmmo3Y, maxAmmo3Width, patches.ShortNumbers),
        ];

        weapons = new MultIconWidget[6];
        for (var i = 0; i < weapons.Length; i++)
            weapons[i] = new MultIconWidget(armsX + (i % 3) * armsSpaceX, armsY + (i / 3) * armsSpaceY, patches.Arms[i]);

        frags = new NumberWidget(fragsX, fragsY, fragsWidth, patches.TallNumbers);

        keys =
        [
            new MultIconWidget(key0X, key0Y, patches.Keys),
            new MultIconWidget(key1X, key1Y, patches.Keys),
            new MultIconWidget(key2X, key2Y, patches.Keys)
        ];
    }

    public void Render(Player player, BackgroundDrawingType backgroundDrawing)
    {
        if (backgroundDrawing == BackgroundDrawingType.Full)
        {
            screen.DrawPatch(
                patches.Background,
                0,
                scale * (200 - Height),
                scale);
        }

        if (DoomInfo.WeaponInfos[player.ReadyWeapon].Ammo != AmmoType.NoAmmo)
        {
            var num = player.Ammo[(int)DoomInfo.WeaponInfos[player.ReadyWeapon].Ammo];
            DrawNumber(ready, num);
        }

        DrawPercent(health, player.Health);
        DrawPercent(armor, player.ArmorPoints);

        for (var i = 0; i < (int)AmmoType.Count; i++)
        {
            DrawNumber(ammo[i], player.Ammo[i]);
            DrawNumber(maxAmmo[i], player.MaxAmmo[i]);
        }

        if (player.Mobj!.World.Options.Deathmatch == 0)
        {
            if (backgroundDrawing == BackgroundDrawingType.Full)
            {
                screen.DrawPatch(
                    patches.ArmsBackground,
                    scale * armsBackgroundX,
                    scale * armsBackgroundY,
                    scale);
            }

            for (var i = 0; i < weapons.Length; i++)
                DrawMultIcon(weapons[i], player.WeaponOwned[i + 1].AsInt());
        }
        else
        {
            var sum = player.Frags.Sum();
            DrawNumber(frags, sum);
        }

        if (backgroundDrawing == BackgroundDrawingType.Full)
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
            if (player.Cards[i + 3])
                DrawMultIcon(keys[i], i + 3);
            else if (player.Cards[i])
                DrawMultIcon(keys[i], i);
        }
    }

    private void DrawNumber(NumberWidget widget, int num)
    {
        var digits = widget.Width;
        var w = widget.Patches[0].Width;
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

        if (num == 1994)
            return;

        var x = widget.X;

        // In the special case of 0, you draw 0.
        if (num == 0)
        {
            screen.DrawPatch(
                patch: widget.Patches[0],
                x: scale * (x - w),
                y: scale * widget.Y,
                scale: scale
            );
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
                patch: patches.TallMinus,
                x: scale * (x - 8),
                y: scale * widget.Y,
                scale: scale
            );
        }

        //screen.DrawText("MHFJDSHFJKDHSJKFHDSJKFDS", 100, 100, 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private void DrawPercent(PercentWidget per, int value)
    {
        screen.DrawPatch(
            per.Patch,
            scale * per.NumberWidget.X,
            scale * per.NumberWidget.Y,
            scale);

        DrawNumber(per.NumberWidget, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private void DrawMultIcon(MultIconWidget mi, int value)
    {
        screen.DrawPatch(
            mi.Patches[value],
            scale * mi.X,
            scale * mi.Y,
            scale);
    }

    private sealed record NumberWidget(int X, int Y, int Width, Patch[] Patches);

    private sealed record PercentWidget(NumberWidget NumberWidget, Patch Patch);

    private sealed record MultIconWidget(int X, int Y, Patch[] Patches);

    private sealed class Patches
    {
        public Patches(Wad wad)
        {
            Background = Patch.FromWad(wad, "STBAR");

            TallNumbers = new Patch[10];
            ShortNumbers = new Patch[10];
            for (var i = 0; i < 10; i++)
            {
                TallNumbers[i] = Patch.FromWad(wad, $"STTNUM{i}");
                ShortNumbers[i] = Patch.FromWad(wad, $"STYSNUM{i}");
            }

            TallMinus = Patch.FromWad(wad, "STTMINUS");
            TallPercent = Patch.FromWad(wad, "STTPRCNT");

            Keys = new Patch[(int)CardType.Count];
            for (var i = 0; i < Keys.Length; i++)
                Keys[i] = Patch.FromWad(wad, $"STKEYS{i}");

            ArmsBackground = Patch.FromWad(wad, "STARMS");
            Arms = new Patch[6][];
            for (var i = 0; i < Arms.Length; i++)
            {
                var num = i + 2;
                Arms[i] = new Patch[2];
                Arms[i][0] = Patch.FromWad(wad, $"STGNUM{num}");
                Arms[i][1] = ShortNumbers[num];
            }

            FaceBackground = new Patch[Player.MaxPlayerCount];
            for (var i = 0; i < FaceBackground.Length; i++)
                FaceBackground[i] = Patch.FromWad(wad, $"STFB{i}");

            Faces = new Patch[StatusBar.Face.FaceCount];
            var faceCount = 0;
            for (var i = 0; i < StatusBar.Face.PainFaceCount; i++)
            {
                for (var j = 0; j < StatusBar.Face.StraightFaceCount; j++)
                    Faces[faceCount++] = Patch.FromWad(wad, $"STFST{i}{j}");

                Faces[faceCount++] = Patch.FromWad(wad, $"STFTR{i}0");
                Faces[faceCount++] = Patch.FromWad(wad, $"STFTL{i}0");
                Faces[faceCount++] = Patch.FromWad(wad, $"STFOUCH{i}");
                Faces[faceCount++] = Patch.FromWad(wad, $"STFEVL{i}");
                Faces[faceCount++] = Patch.FromWad(wad, $"STFKILL{i}");
            }

            Faces[faceCount++] = Patch.FromWad(wad, "STFGOD0");
            Faces[faceCount] = Patch.FromWad(wad, "STFDEAD0");
        }

        public Patch Background { get; }
        public Patch[] TallNumbers { get; }
        public Patch[] ShortNumbers { get; }
        public Patch TallMinus { get; }
        public Patch TallPercent { get; }
        public Patch[] Keys { get; }
        public Patch ArmsBackground { get; }
        public Patch[][] Arms { get; }
        public Patch[] FaceBackground { get; }
        public Patch[] Faces { get; }
    }
}