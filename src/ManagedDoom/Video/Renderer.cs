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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ManagedDoom.Config;
using ManagedDoom.Doom;
using ManagedDoom.Doom.Game;
using ManagedDoom.Doom.Graphics;
using ManagedDoom.Doom.Info;
using ManagedDoom.Doom.Math;
using ManagedDoom.Doom.Opening;
using ManagedDoom.Doom.World;
using ManagedDoom.Video.Renders.ThreeDee;

namespace ManagedDoom.Video;

public sealed class Renderer
{
    private static readonly double[] gammaCorrectionParameters =
    [
        1.00,
        0.95,
        0.90,
        0.85,
        0.80,
        0.75,
        0.70,
        0.65,
        0.60,
        0.55,
        0.50
    ];

    private readonly AutoMapRenderer autoMap;

    private readonly ConfigValues config;
    private readonly FinaleRenderer finale;
    private readonly IntermissionRenderer intermission;

    private readonly MenuRenderer menu;
    private readonly OpeningSequenceRenderer openingSequence;

    private readonly Palette palette;

    private readonly Patch pause;

    private readonly DrawScreen screen;
    private readonly StatusBarRenderer statusBar;
    private readonly ThreeDeeRenderer threeDee;

    private readonly int wipeBandWidth;
    private readonly byte[] wipeBuffer;

    public Renderer(ConfigValues config, GameContent content)
    {
        this.config = config;

        palette = content.Palette;

        screen = config.VideoHighResolution
            ? new DrawScreen(content.Wad, 640, 400)
            : new DrawScreen(content.Wad, 320, 200);

        config.VideoGameScreenSize = Math.Clamp(config.VideoGameScreenSize, 0, MaxWindowSize);
        config.VideoGammaCorrection = Math.Clamp(config.VideoGammaCorrection, 0, MaxGammaCorrectionLevel);

        var patchCache = new PatchCache(content.Wad);
        menu = new MenuRenderer(patchCache, screen);
        threeDee = new ThreeDeeRenderer(content, screen, config.VideoGameScreenSize);
        statusBar = new StatusBarRenderer(content.Wad, screen);
        intermission = new IntermissionRenderer(content.Wad, patchCache, screen);
        openingSequence = new OpeningSequenceRenderer(patchCache, screen, this);
        autoMap = new AutoMapRenderer(content.Wad, screen);
        finale = new FinaleRenderer(content.Flats, content.Sprites, patchCache, screen);

        pause = Patch.FromWad(content.Wad, "M_PAUSE");

        var scale = screen.Width / 320;
        wipeBandWidth = 2 * scale;
        WipeBandCount = screen.Width / wipeBandWidth + 1;
        WipeHeight = screen.Height / scale;
        wipeBuffer = new byte[screen.Data.Length];

        palette.ResetColors(in gammaCorrectionParameters[config.VideoGammaCorrection]);
    }

    public int Width => screen.Width;
    public int Height => screen.Height;

    public int WipeBandCount { get; }

    public int WipeHeight { get; }

    public int MaxWindowSize => ThreeDeeRenderer.MaxScreenSize;

    public int WindowSize
    {
        get => threeDee.WindowSize;

        set
        {
            config.VideoGameScreenSize = value;
            threeDee.WindowSize = value;
        }
    }

    public bool DisplayMessage
    {
        get => config.VideoDisplayMessage;
        set => config.VideoDisplayMessage = value;
    }

    public int MaxGammaCorrectionLevel => gammaCorrectionParameters.Length - 1;

    public int GammaCorrectionLevel
    {
        get => config.VideoGammaCorrection;
        set
        {
            config.VideoGammaCorrection = value;
            palette.ResetColors(in gammaCorrectionParameters[config.VideoGammaCorrection]);
        }
    }

    private void RenderDoom(Doom.Doom doom, Fixed frameFrac)
    {
        switch (doom.State)
        {
            case DoomState.Game:
                RenderGame(doom.Game, frameFrac);
                break;
            case DoomState.Opening:
                openingSequence.Render(doom.Opening, frameFrac);
                break;
            case DoomState.DemoPlayback:
                RenderGame(doom.DemoPlayback.Game, frameFrac);
                break;
        }

        if (doom.Menu.Active) return;

        if (doom.State == DoomState.Game &&
            doom.Game.State == GameState.Level &&
            doom.Game.Paused)
        {
            var scale = screen.Width / 320;
            screen.DrawPatch(
                pause,
                (screen.Width - scale * pause.Width) / 2,
                4 * scale,
                scale);
        }
    }

    private void RenderMenu(Doom.Doom doom)
    {
        if (doom.Menu.Active)
        {
            menu.Render(doom.Menu);
        }
    }

    public void RenderGame(DoomGame game, Fixed frameFrac)
    {
        if (game.Paused)
        {
            frameFrac = Fixed.One;
        }

        switch (game.State)
        {
            case GameState.Level:
            {
                var consolePlayer = game.World.ConsolePlayer;
                var displayPlayer = game.World.DisplayPlayer;

                if (game.World.AutoMap.Visible)
                {
                    autoMap.Render(consolePlayer);
                    statusBar.Render(consolePlayer, BackgroundDrawingType.Full);
                }
                else
                {
                    threeDee.Render(displayPlayer, frameFrac);
                    switch (threeDee.WindowSize)
                    {
                        case < 8:
                            statusBar.Render(consolePlayer, BackgroundDrawingType.Full);
                            break;
                        case ThreeDeeRenderer.MaxScreenSize:
                            statusBar.Render(consolePlayer, BackgroundDrawingType.None);
                            break;
                    }
                }

                if (config.VideoDisplayMessage || ReferenceEquals(consolePlayer.Message, (string)DoomInfo.Strings.MSGOFF))
                {
                    if (consolePlayer.MessageTime > 0)
                    {
                        var scale = screen.Width / 320;
                        screen.DrawText(consolePlayer.Message, 0, 7 * scale, scale);
                    }
                }

                break;
            }
            case GameState.Intermission:
                intermission.Render(game.Intermission);
                break;
            case GameState.Finale:
                finale.Render(game.Finale);
                break;
        }
    }

    public void Render(Doom.Doom doom, Span<byte> destination, Fixed frameFrac)
    {
        if (doom.Wiping)
        {
            RenderWipe(doom, destination);
            return;
        }

        RenderDoom(doom, frameFrac);
        RenderMenu(doom);

        var colors = palette[0];
        if (doom.State == DoomState.Game &&
            doom.Game.State == GameState.Level)
        {
            colors = palette[GetPaletteNumber(doom.Game.World.ConsolePlayer)];
        }
        else if (doom.State == DoomState.Opening &&
                 doom.Opening.State == OpeningSequenceState.Demo &&
                 doom.Opening.DemoGame.State == GameState.Level)
        {
            colors = palette[GetPaletteNumber(doom.Opening.DemoGame.World.ConsolePlayer)];
        }
        else if (doom.State == DoomState.DemoPlayback &&
                 doom.DemoPlayback.Game.State == GameState.Level)
        {
            colors = palette[GetPaletteNumber(doom.DemoPlayback.Game.World.ConsolePlayer)];
        }

        WriteData(colors, destination);
    }

    private void RenderWipe(Doom.Doom doom, Span<byte> destination)
    {
        RenderDoom(doom, Fixed.One);

        var wipe = doom.WipeEffect;
        var scale = screen.Width / 320;
        for (var i = 0; i < WipeBandCount - 1; i++)
        {
            var x1 = wipeBandWidth * i;
            var x2 = x1 + wipeBandWidth;
            var y1 = Math.Max(scale * wipe.Y[i], 0);
            var y2 = Math.Max(scale * wipe.Y[i + 1], 0);
            var dy = (float)(y2 - y1) / wipeBandWidth;
            for (var x = x1; x < x2; x++)
            {
                var y = (int)MathF.Round(y1 + dy * ((x - x1) / 2 * 2));
                var copyLength = screen.Height - y;
                if (copyLength > 0)
                {
                    var srcPos = screen.Height * x;
                    var dstPos = screen.Height * x + y;
                    Array.Copy(wipeBuffer, srcPos, screen.Data, dstPos, copyLength);
                }
            }
        }

        RenderMenu(doom);

        WriteData(palette[0], destination);
    }

    public void InitializeWipe()
    {
        Array.Copy(screen.Data, wipeBuffer, screen.Data.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private void WriteData(ReadOnlySpan<uint> colors, Span<byte> destination)
    {
        var screenData = screen.Data.AsSpan();
        var p = MemoryMarshal.Cast<byte, uint>(destination);
        for (var i = 0; i < p.Length; i++)
            p[i] = colors[screenData[i]];
    }

    private static int GetPaletteNumber(Player player)
    {
        var count = player.DamageCount;

        if (player.Powers[(int)PowerType.Strength] != 0)
        {
            // Slowly fade the berzerk out.
            var bzc = 12 - (player.Powers[(int)PowerType.Strength] >> 6);
            if (bzc > count)
            {
                count = bzc;
            }
        }

        int palette;

        if (count != 0)
        {
            palette = (count + 7) >> 3;

            if (palette >= Palette.DamageCount)
            {
                palette = Palette.DamageCount - 1;
            }

            palette += Palette.DamageStart;
        }
        else if (player.BonusCount != 0)
        {
            palette = (player.BonusCount + 7) >> 3;

            if (palette >= Palette.BonusCount)
            {
                palette = Palette.BonusCount - 1;
            }

            palette += Palette.BonusStart;
        }
        else if (player.Powers[(int)PowerType.IronFeet] > 4 * 32 ||
                 (player.Powers[(int)PowerType.IronFeet] & 8) != 0)
        {
            palette = Palette.IronFeet;
        }
        else
        {
            palette = 0;
        }

        return palette;
    }
}