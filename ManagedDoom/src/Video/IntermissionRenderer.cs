﻿//
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
using ManagedDoom.Doom.Game;
using ManagedDoom.Doom.Graphics;
using ManagedDoom.Doom.Intermission;
using ManagedDoom.Doom.Wad;

namespace ManagedDoom.Video;

public sealed class IntermissionRenderer
{
    // GLOBAL LOCATIONS
    private const int titleY = 2;
    private const int spacingY = 33;

    // SINGPLE-PLAYER STUFF
    private const int spStatsX = 50;
    private const int spStatsY = 50;
    private const int spTimeX = 16;
    private const int spTimeY = 200 - 32;

    // NET GAME STUFF
    private const int ngStatsY = 50;
    private const int ngSpacingX = 64;

    // DEATHMATCH STUFF
    private const int dmMatrixX = 42;
    private const int dmMatrixY = 68;
    private const int dmSpacingX = 40;
    private const int dmTotalsX = 269;
    private const int dmKillersX = 10;
    private const int dmKillersY = 100;
    private const int dmVictimsX = 5;
    private const int dmVictimsY = 50;

    private static readonly string[] mapPictures =
    [
        "WIMAP0",
        "WIMAP1",
        "WIMAP2"
    ];

    private static readonly string[] playerBoxes =
    [
        "STPB0",
        "STPB1",
        "STPB2",
        "STPB3"
    ];

    private static readonly string[] youAreHere =
    [
        "WIURH0",
        "WIURH1"
    ];

    private static readonly string[][] doomLevels;
    private static readonly string[] doom2Levels;

    private readonly PatchCache cache;
    private readonly Patch colon;

    private readonly Patch minus;
    private readonly Patch[] numbers;
    private readonly Patch percent;

    private readonly int scale;
    private readonly DrawScreen screen;

    static IntermissionRenderer()
    {
        doomLevels = new string[4][];
        for (var e = 0; e < 4; e++)
        {
            doomLevels[e] = new string[9];
            for (var m = 0; m < 9; m++)
                doomLevels[e][m] = $"WILV{e}{m}";
        }

        doom2Levels = new string[32];
        for (var m = 0; m < 32; m++)
            doom2Levels[m] = $"CWILV{m:00}";
    }

    public IntermissionRenderer(Wad wad, DrawScreen screen)
    {
        this.screen = screen;

        cache = new PatchCache(wad);

        minus = Patch.FromWad(wad, "WIMINUS");
        numbers = new Patch[10];
        for (var i = 0; i < 10; i++)
            numbers[i] = Patch.FromWad(wad, $"WINUM{i}");

        percent = Patch.FromWad(wad, "WIPCNT");
        colon = Patch.FromWad(wad, "WICOLON");

        scale = screen.Width / 320;
    }


    private void DrawPatch(Patch patch, int x, int y)
    {
        screen.DrawPatch(patch, scale * x, scale * y, scale);
    }

    private void DrawPatch(string name, int x, int y)
    {
        var patchScale = screen.Width / 320;
        screen.DrawPatch(cache[name], patchScale * x, patchScale * y, patchScale);
    }

    private void DrawPatch(string name, in WorldMap.Point point)
    {
        var patchScale = screen.Width / 320;
        screen.DrawPatch(cache[name], patchScale * point.X, patchScale * point.Y, patchScale);
    }


    private int GetWidth(string name)
    {
        return cache.GetWidth(name);
    }

    private int GetHeight(string name)
    {
        return cache.GetHeight(name);
    }


    public void Render(Intermission im)
    {
        switch (im.State)
        {
            case IntermissionState.StatCount:
                if (im.Options.Deathmatch != 0)
                    DrawDeathmatchStats(im);
                else if (im.Options.NetGame)
                    DrawNetGameStats(im);
                else
                    DrawSinglePlayerStats(im);

                break;

            case IntermissionState.ShowNextLoc:
                DrawShowNextLoc(im);
                break;

            case IntermissionState.NoState:
                DrawNoState(im);
                break;
        }
    }


    private void DrawBackground(Intermission im)
    {
        if (im.Options.GameMode == GameMode.Commercial)
            DrawPatch("INTERPIC", 0, 0);
        else
        {
            var e = im.Options.Episode - 1;
            var patchName = e < mapPictures.Length ? mapPictures[e] : "INTERPIC";
            DrawPatch(patchName, 0, 0);
        }
    }

    private void DrawSinglePlayerStats(Intermission im)
    {
        DrawBackground(im);

        // Draw animated background.
        DrawBackgroundAnimation(im);

        // Draw level name.
        DrawFinishedLevelName(im);

        // Line height.
        var lineHeight = (3 * numbers[0].Height) / 2;

        DrawPatch(
            "WIOSTK", // KILLS
            spStatsX,
            spStatsY);

        DrawPercent(
            320 - spStatsX,
            spStatsY,
            im.KillCount[0]);

        DrawPatch(
            "WIOSTI", // ITEMS
            spStatsX,
            spStatsY + lineHeight);

        DrawPercent(
            320 - spStatsX,
            spStatsY + lineHeight,
            im.ItemCount[0]);

        DrawPatch(
            "WISCRT2", // SECRET
            spStatsX,
            spStatsY + 2 * lineHeight);

        DrawPercent(
            320 - spStatsX,
            spStatsY + 2 * lineHeight,
            im.SecretCount[0]);

        DrawPatch(
            "WITIME", // TIME
            spTimeX,
            spTimeY);

        DrawTime(
            320 / 2 - spTimeX,
            spTimeY,
            im.TimeCount);

        if (im.Info.Episode < 3)
        {
            DrawPatch(
                "WIPAR", // PAR
                320 / 2 + spTimeX,
                spTimeY);

            DrawTime(
                320 - spTimeX,
                spTimeY,
                im.ParCount);
        }
    }

    private void DrawNetGameStats(Intermission im)
    {
        DrawBackground(im);

        // Draw animated background.
        DrawBackgroundAnimation(im);

        // Draw level name.
        DrawFinishedLevelName(im);

        var ngStatsX = 32 + GetWidth("STFST01") / 2;
        if (!im.DoFrags)
        {
            ngStatsX += 32;
        }

        // Draw stat titles (top line).
        DrawPatch(
            "WIOSTK", // KILLS
            ngStatsX + ngSpacingX - GetWidth("WIOSTK"),
            ngStatsY);

        DrawPatch(
            "WIOSTI", // ITEMS
            ngStatsX + 2 * ngSpacingX - GetWidth("WIOSTI"),
            ngStatsY);

        DrawPatch(
            "WIOSTS", // SCRT
            ngStatsX + 3 * ngSpacingX - GetWidth("WIOSTS"),
            ngStatsY);

        if (im.DoFrags)
        {
            DrawPatch(
                "WIFRGS", // FRAGS
                ngStatsX + 4 * ngSpacingX - GetWidth("WIFRGS"),
                ngStatsY);
        }

        // Draw stats.
        var y = ngStatsY + GetHeight("WIOSTK");

        for (var i = 0; i < Player.MaxPlayerCount; i++)
        {
            if (!im.Options.Players[i].InGame)
                continue;

            var x = ngStatsX;

            DrawPatch(
                playerBoxes[i],
                x - GetWidth(playerBoxes[i]),
                y);

            if (i == im.Options.ConsolePlayer)
            {
                DrawPatch(
                    "STFST01", // Player face
                    x - GetWidth(playerBoxes[i]),
                    y);
            }

            x += ngSpacingX;

            DrawPercent(x - percent.Width, y + 10, im.KillCount[i]);
            x += ngSpacingX;

            DrawPercent(x - percent.Width, y + 10, im.ItemCount[i]);
            x += ngSpacingX;

            DrawPercent(x - percent.Width, y + 10, im.SecretCount[i]);
            x += ngSpacingX;

            if (im.DoFrags)
                DrawNumber(x, y + 10, im.FragCount[i], -1);

            y += spacingY;
        }
    }

    private void DrawDeathmatchStats(Intermission im)
    {
        DrawBackground(im);

        // Draw animated background.
        DrawBackgroundAnimation(im);

        // Draw level name.
        DrawFinishedLevelName(im);

        // Draw stat titles (top line).
        DrawPatch(
            "WIMSTT", // TOTAL
            dmTotalsX - GetWidth("WIMSTT") / 2,
            dmMatrixY - spacingY + 10);

        DrawPatch(
            "WIKILRS", // KILLERS
            dmKillersX,
            dmKillersY);

        DrawPatch(
            "WIVCTMS", // VICTIMS
            dmVictimsX,
            dmVictimsY);

        // Draw player boxes.
        var x = dmMatrixX + dmSpacingX;
        var y = dmMatrixY;

        for (var i = 0; i < Player.MaxPlayerCount; i++)
        {
            if (im.Options.Players[i].InGame)
            {
                DrawPatch(
                    playerBoxes[i],
                    x - GetWidth(playerBoxes[i]) / 2,
                    dmMatrixY - spacingY);

                DrawPatch(
                    playerBoxes[i],
                    dmMatrixX - GetWidth(playerBoxes[i]) / 2,
                    y);

                if (i == im.Options.ConsolePlayer)
                {
                    DrawPatch(
                        "STFDEAD0", // Player face (dead)
                        x - GetWidth(playerBoxes[i]) / 2,
                        dmMatrixY - spacingY);

                    DrawPatch(
                        "STFST01", // Player face
                        dmMatrixX - GetWidth(playerBoxes[i]) / 2,
                        y);
                }
            }

            // V_DrawPatch(x-SHORT(bp[i]->width)/2,
            //   DM_MATRIXY - WI_SPACINGY, FB, bp[i]);
            // V_DrawPatch(DM_MATRIXX-SHORT(bp[i]->width)/2,
            //   y, FB, bp[i]);
            x += dmSpacingX;
            y += spacingY;
        }

        // Draw stats.
        y = dmMatrixY + 10;
        var w = numbers[0].Width;

        for (var i = 0; i < Player.MaxPlayerCount; i++)
        {
            x = dmMatrixX + dmSpacingX;

            if (im.Options.Players[i].InGame)
            {
                for (var j = 0; j < Player.MaxPlayerCount; j++)
                {
                    if (im.Options.Players[j].InGame)
                        DrawNumber(x + w, y, im.DeathmatchFrags[i][j], 2);

                    x += dmSpacingX;
                }

                DrawNumber(dmTotalsX + w, y, im.DeathmatchTotals[i], 2);
            }

            y += spacingY;
        }
    }

    private void DrawNoState(Intermission im)
    {
        DrawShowNextLoc(im);
    }

    private void DrawShowNextLoc(Intermission im)
    {
        DrawBackground(im);

        // Draw animated background.
        DrawBackgroundAnimation(im);

        if (im.Options.GameMode != GameMode.Commercial)
        {
            if (im.Info.Episode > 2)
            {
                DrawEnteringLevelName(im);
                return;
            }

            var last = (im.Info.LastLevel == 8) ? im.Info.NextLevel - 1 : im.Info.LastLevel;

            // Draw a splat on taken cities.
            for (var i = 0; i <= last; i++)
            {
                var p = WorldMap.Locations[im.Info.Episode][i];
                DrawPatch("WISPLAT", in p);
            }

            // Splat the secret level?
            if (im.Info.DidSecret)
            {
                var p = WorldMap.Locations[im.Info.Episode][8];
                DrawPatch("WISPLAT", in p);
            }

            // Draw "you are here".
            if (im.ShowYouAreHere)
            {
                var p = WorldMap.Locations[im.Info.Episode][im.Info.NextLevel];
                DrawSuitablePatch(youAreHere, p.X, p.Y);
            }
        }

        // Draw next level name.
        if ((im.Options.GameMode != GameMode.Commercial) || im.Info.NextLevel != 30)
        {
            DrawEnteringLevelName(im);
        }
    }

    private void DrawFinishedLevelName(Intermission intermission)
    {
        var wbs = intermission.Info;
        var y = titleY;

        string levelName;
        if (intermission.Options.GameMode != GameMode.Commercial)
        {
            var e = intermission.Options.Episode - 1;
            levelName = doomLevels[e][wbs.LastLevel];
        }
        else
            levelName = doom2Levels[wbs.LastLevel];

        // Draw level name. 
        DrawPatch(
            levelName,
            (320 - GetWidth(levelName)) / 2,
            y);

        // Draw "Finished!".
        y += (5 * GetHeight(levelName)) / 4;

        DrawPatch(
            "WIF",
            (320 - GetWidth("WIF")) / 2,
            y);
    }

    private void DrawEnteringLevelName(Intermission im)
    {
        var wbs = im.Info;
        var y = titleY;

        string levelName;
        if (im.Options.GameMode != GameMode.Commercial)
        {
            var e = im.Options.Episode - 1;
            levelName = doomLevels[e][wbs.NextLevel];
        }
        else
            levelName = doom2Levels[wbs.NextLevel];

        // Draw "Entering".
        DrawPatch(
            "WIENTER",
            (320 - GetWidth("WIENTER")) / 2,
            y);

        // Draw level name.
        y += (5 * GetHeight(levelName)) / 4;

        DrawPatch(
            levelName,
            (320 - GetWidth(levelName)) / 2,
            y);
    }


    private int DrawNumber(int x, int y, int n, int digits)
    {
        if (digits < 0)
        {
            if (n == 0)
            {
                // Make variable-length zeros 1 digit long.
                digits = 1;
            }
            else
            {
                // Figure out number of digits.
                digits = 0;
                var temp = n;
                while (temp != 0)
                {
                    temp /= 10;
                    digits++;
                }
            }
        }

        var neg = n < 0;
        if (neg)
            n = -n;

        // If non-number, do not draw it.
        if (n == 1994)
            return 0;

        var fontWidth = numbers[0].Width;

        // Draw the new number.
        while (digits-- != 0)
        {
            x -= fontWidth;
            DrawPatch(numbers[n % 10], x, y);
            n /= 10;
        }

        // Draw a minus sign if necessary.
        if (neg)
            DrawPatch(minus, x -= 8, y);

        return x;
    }

    private void DrawPercent(int x, int y, int p)
    {
        if (p < 0)
            return;

        DrawPatch(percent, x, y);
        DrawNumber(x, y, p, -1);
    }

    private void DrawTime(int x, int y, int t)
    {
        switch (t)
        {
            case < 0:
                return;
            case <= 61 * 59:
            {
                var div = 1;

                do
                {
                    var n = (t / div) % 60;
                    x = DrawNumber(x, y, n, 2) - colon.Width;
                    div *= 60;

                    // Draw.
                    if (div == 60 || t / div != 0)
                        DrawPatch(colon, x, y);
                } while (t / div != 0);

                break;
            }
            default:
                DrawPatch(
                    "WISUCKS", // SUCKS
                    x - GetWidth("WISUCKS"),
                    y);
                break;
        }
    }

    private void DrawBackgroundAnimation(Intermission im)
    {
        if (im.Options.GameMode == GameMode.Commercial)
            return;

        if (im.Info.Episode > 2)
            return;

        foreach (var a in im.Animations)
        {
            if (a.PatchNumber >= 0)
                DrawPatch(a.Patches[a.PatchNumber], a.LocationX, a.LocationY);
        }
    }

    private void DrawSuitablePatch(IReadOnlyList<string> candidates, int x, int y)
    {
        var fits = false;
        var i = 0;

        do
        {
            var patch = cache[candidates[i]];

            var left = x - patch.LeftOffset;
            var top = y - patch.TopOffset;
            var right = left + patch.Width;
            var bottom = top + patch.Height;

            if (left >= 0 && right < 320 && top >= 0 && bottom < 320)
                fits = true;
            else
                i++;
        } while (!fits && i != 2);

        if (fits && i < 2)
            DrawPatch(candidates[i], x, y);
        else
            throw new Exception("Could not place patch!");
    }
}