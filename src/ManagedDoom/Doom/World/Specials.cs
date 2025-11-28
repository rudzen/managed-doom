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
using System.Linq;
using ManagedDoom.Audio;
using ManagedDoom.Doom.Map;
using ManagedDoom.Doom.Math;

namespace ManagedDoom.Doom.World;

public sealed class Specials
{
    private const int MaxButtonCount = 32;
    private const int ButtonTime = 35;

    private readonly Button[] buttonList;

    private readonly World world;
    private int levelTimeCount;

    private bool levelTimer;

    private LineDef[] scrollLines;

    public Specials(World world)
    {
        this.world = world;

        levelTimer = false;

        buttonList = new Button[MaxButtonCount];
        for (var i = 0; i < buttonList.Length; i++)
            buttonList[i] = new Button();

        TextureTranslation = new int[world.Map.Textures.Count];
        for (var i = 0; i < TextureTranslation.Length; i++)
            TextureTranslation[i] = i;

        FlatTranslation = new int[world.Map.Flats.Count];
        for (var i = 0; i < FlatTranslation.Length; i++)
            FlatTranslation[i] = i;
    }

    public int[] TextureTranslation { get; }

    public int[] FlatTranslation { get; }

    /// <summary>
    /// After the map has been loaded, scan for specials that spawn thinkers.
    /// </summary>
    public void SpawnSpecials(int levelTimeCount)
    {
        levelTimer = true;
        this.levelTimeCount = levelTimeCount;
        SpawnSpecials();
    }

    /// <summary>
    /// After the map has been loaded, scan for specials that spawn thinkers.
    /// </summary>
    public void SpawnSpecials()
    {
        // Init special sectors.
        var lc = world.LightingChange;
        var sa = world.SectorAction;
        foreach (var sector in world.Map.Sectors)
        {
            if (sector.Special == SectorSpecial.Normal)
                continue;
            if (sector.Special == SectorSpecial.FlickeringLightsSpawn)
                lc.SpawnLightFlash(sector);
            else if (sector.Special == SectorSpecial.StrobeFastSpawn)
                lc.SpawnStrobeFlash(sector, StrobeFlash.FastDark, false);
            else if (sector.Special == SectorSpecial.StrobeSlowSpawn)
                lc.SpawnStrobeFlash(sector, StrobeFlash.SlowDark, false);
            else if (sector.Special == SectorSpecial.StrobeFastDeathSlimeSpawn)
            {
                lc.SpawnStrobeFlash(sector, StrobeFlash.FastDark, false);
                sector.Special = SectorSpecial.StrobeFastDeathSlimeSpawn;
            }
            else if (sector.Special == SectorSpecial.GlowingLightSpawn)
                lc.SpawnGlowingLight(sector);
            else if (sector.Special == SectorSpecial.SecretSectorSpawn)
                world.TotalSecrets++;
            else if (sector.Special == SectorSpecial.DoorCloseIn30SecondsSpawn)
                sa.SpawnDoorCloseIn30(sector);
            else if (sector.Special == SectorSpecial.SyncStrobeSlowSpawn)
                lc.SpawnStrobeFlash(sector, StrobeFlash.SlowDark, true);
            else if (sector.Special == SectorSpecial.SyncStrobeFastSpawn)
                lc.SpawnStrobeFlash(sector, StrobeFlash.FastDark, true);
            else if (sector.Special == SectorSpecial.DoorRaiseIn5MinutesSpawn)
                sa.SpawnDoorRaiseIn5Mins(sector);
            else if (sector.Special == SectorSpecial.FireFlickerSpawn)
                lc.SpawnFireFlicker(sector);
        }

        scrollLines = [.. world
                      .Map
                      .Lines
                      .Where(line => line.Special == LineSpecial.TextureScroll)];
    }

    public void ChangeSwitchTexture(LineDef line, bool useAgain)
    {
        if (!useAgain)
            line.Special = 0;

        var frontSide = line.FrontSide;
        var topTexture = frontSide!.TopTexture;
        var middleTexture = frontSide.MiddleTexture;
        var bottomTexture = frontSide.BottomTexture;

        var sound = Sfx.SWTCHN;

        // Exit switch?
        if (line.Special == LineSpecial.ExitLevelSwitch)
            sound = Sfx.SWTCHX;

        var switchList = world.Map.Textures.SwitchList;

        for (var i = 0; i < switchList.Length; i++)
        {
            if (switchList[i] == topTexture)
            {
                world.StartSound(line.SoundOrigin, sound, SfxType.Misc);
                frontSide.TopTexture = switchList[i ^ 1];

                if (useAgain)
                    StartButton(line, ButtonPosition.Top, switchList[i], ButtonTime);

                return;
            }

            if (switchList[i] == middleTexture)
            {
                world.StartSound(line.SoundOrigin, sound, SfxType.Misc);
                frontSide.MiddleTexture = switchList[i ^ 1];

                if (useAgain)
                    StartButton(line, ButtonPosition.Middle, switchList[i], ButtonTime);

                return;
            }

            if (switchList[i] == bottomTexture)
            {
                world.StartSound(line.SoundOrigin, sound, SfxType.Misc);
                frontSide.BottomTexture = switchList[i ^ 1];

                if (useAgain)
                    StartButton(line, ButtonPosition.Bottom, switchList[i], ButtonTime);

                return;
            }
        }
    }

    private void StartButton(LineDef line, ButtonPosition w, int texture, int time)
    {
        var buttons = buttonList.AsSpan();
        // See if button is already pressed.
        foreach (var button in buttons)
        {
            if (button.Timer != 0 && button.Line == line)
                return;
        }

        foreach (var button in buttons)
        {
            if (button.Timer != 0)
                continue;

            button.Line = line;
            button.Position = w;
            button.Texture = texture;
            button.Timer = time;
            button.SoundOrigin = line.SoundOrigin;
            return;
        }

        throw new Exception("No button slots left!");
    }

    /// <summary>
    /// Animate planes, scroll walls, etc.
    /// </summary>
    public void Update()
    {
        // Level timer.
        if (levelTimer)
        {
            levelTimeCount--;
            if (levelTimeCount == 0)
                world.ExitLevel();
        }

        // Animate flats and textures globally.
        var animations = world.Map.Animation.Animations.AsSpan();
        foreach (var anim in animations)
        {
            for (var i = anim.BasePic; i < anim.BasePic + anim.NumPics; i++)
            {
                var pic = anim.BasePic + ((world.LevelTime / anim.Speed + i) % anim.NumPics);
                if (anim.IsTexture)
                    TextureTranslation[i] = pic;
                else
                    FlatTranslation[i] = pic;
            }
        }

        // Animate line specials.
        foreach (var line in scrollLines)
            line.FrontSide!.TextureOffset += Fixed.One;

        var buttons = buttonList.AsSpan();

        // Do buttons.
        foreach (var button in buttons)
        {
            if (button.Timer <= 0)
                continue;

            button.Timer--;

            if (button.Timer != 0)
                continue;

            if (button.Position == ButtonPosition.Top)
                button.Line!.FrontSide!.TopTexture = button.Texture;
            else if (button.Position == ButtonPosition.Middle)
                button.Line!.FrontSide!.MiddleTexture = button.Texture;
            else if (button.Position == ButtonPosition.Bottom)
                button.Line!.FrontSide!.BottomTexture = button.Texture;

            world.StartSound(button.SoundOrigin!, Sfx.SWTCHN, SfxType.Misc, 50);
            button.Clear();
        }
    }
}