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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ManagedDoom.Doom.Game;
using ManagedDoom.Doom.Graphics;
using ManagedDoom.Doom.Map;
using ManagedDoom.Doom.Math;
using ManagedDoom.Doom.World;
using ManagedDoom.Silk;

namespace ManagedDoom.Video.Renders.ThreeDee;

public sealed class ThreeDeeRenderer : IThreeDeeRenderer
{
    public const int MaxScreenSize = 9;

    private readonly ColorMap colorMap;
    private readonly ITextureLookup textures;
    private readonly IFlatLookup flats;
    private readonly ISpriteLookup sprites;

    private readonly DrawScreen screen;
    private readonly int screenWidth;
    private readonly int screenHeight;
    private readonly byte[] screenData;
    private readonly int drawScale;

    private int windowSize;

    private Fixed frameFrac;

    private readonly WindowSettings windowSettings;
    private readonly WallRender wallRender;
    private readonly PlaneRender planeRender;
    private readonly SkyRender skyRender;
    private readonly LightningRender lightningRender;
    private readonly RenderingHistory renderingHistory;
    private readonly SpriteRender spriteRender;
    private readonly WeaponRender weaponRender;

    private int fuzzEffectPos;
    private readonly ColorTranslation colorTranslation;
    private readonly WindowBorder windowBorder;

    public ThreeDeeRenderer(GameContent content, DrawScreen screen, SilkConfig silkConfig)
    {
        colorMap = content.ColorMap;
        textures = content.Textures;
        flats = content.Flats;
        sprites = content.Sprites;

        this.screen = screen;
        screenWidth = screen.Width;
        screenHeight = screen.Height;
        screenData = screen.Data;
        drawScale = screenWidth / 320;

        this.windowSize = silkConfig.DoomConfig.Values.VideoGameScreenSize;

        this.wallRender = new WallRender(screenWidth);
        this.planeRender = new PlaneRender(screenWidth, screenHeight);
        this.skyRender = new SkyRender();
        this.lightningRender = new LightningRender(screenWidth, colorMap);
        this.renderingHistory = new RenderingHistory(screenWidth);
        this.spriteRender = new SpriteRender();
        this.weaponRender = new WeaponRender();

        this.colorTranslation = new ColorTranslation();
        this.colorTranslation.InitTranslations();

        this.windowBorder = new WindowBorder(content.Wad, flats);

        this.windowSettings = new WindowSettings();
        SetWindowSize(windowSize);
    }

    private void SetWindowSize(int size)
    {
        var scale = screenWidth / 320;
        switch (size)
        {
            case < 7:
            {
                var width = scale * (96 + 32 * size);
                var height = scale * (48 + 16 * size);
                var x = (screenWidth - width) / 2;
                var y = (screenHeight - StatusBarRenderer.Height * scale - height) / 2;
                windowSettings.Reset(x, y, width, height);
                break;
            }
            case 7:
            {
                var height = screenHeight - StatusBarRenderer.Height * scale;
                windowSettings.Reset(0, 0, screenWidth, height);
                break;
            }
            default:
            {
                windowSettings.Reset(0, 0, screenWidth, screenHeight);
                break;
            }
        }

        wallRender.Reset(windowSettings.CenterXFrac, windowSettings.WindowWidth);
        planeRender.Reset(windowSettings.WindowWidth, windowSettings.WindowHeight, wallRender);
        skyRender.Reset(windowSettings.WindowWidth, screenWidth, screenHeight);
        lightningRender.Reset(windowSettings.WindowWidth, colorMap);
        renderingHistory.Reset(windowSettings);
        weaponRender.Reset(windowSettings);
    }

    ////////////////////////////////////////////////////////////
    // Camera view
    ////////////////////////////////////////////////////////////

    private World? world;

    private Fixed viewX;
    private Fixed viewY;
    private Fixed viewZ;
    private Angle viewAngle;

    private Fixed viewSin;
    private Fixed viewCos;

    private int validCount;

    public void Render(Player player, Fixed frameFrac)
    {
        this.frameFrac = frameFrac;

        world = player.Mobj!.World;

        viewX = player.Mobj.GetInterpolatedX(frameFrac);
        viewY = player.Mobj.GetInterpolatedY(frameFrac);
        viewZ = player.GetInterpolatedViewZ(frameFrac);
        viewAngle = player.GetInterpolatedAngle(frameFrac);

        viewSin = Trig.Sin(viewAngle);
        viewCos = Trig.Cos(viewAngle);

        validCount = world.GetNewValidCount();

        lightningRender.ExtraLight = player.ExtraLight;
        lightningRender.FixedColorMap = player.FixedColorMap;

        planeRender.Clear(viewAngle, windowSettings.CenterXFrac);
        lightningRender.Clear(colorMap);
        renderingHistory.Clear(windowSettings);
        spriteRender.Clear();

        RenderBspNode(world.Map.Nodes.Length - 1);
        RenderSprites();
        RenderMaskedTextures();
        DrawPlayerSprites(player);

        if (windowSize < 7)
            windowBorder.FillBackScreen(screenData.AsSpan(), screen, windowSettings, drawScale);
    }

    private void RenderBspNode(int node)
    {
        while (true)
        {
            if (Node.IsSubsector(node))
            {
                var subSectorIndex = node == -1 ? 0 : Node.GetSubsector(node);
                DrawSubsector(subSectorIndex);
                return;
            }

            var bsp = world!.Map.Nodes[node];

            // Decide which side the view point is on.
            var side = Geometry.PointOnSide(viewX, viewY, bsp);

            // Recursively divide front space.
            RenderBspNode(bsp.Children[side]);

            // Possibly divide backspace.
            if (IsPotentiallyVisible(bsp.BoundingBox[side ^ 1]))
            {
                node = bsp.Children[side ^ 1];
                continue;
            }

            break;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DrawSubsector(int subSector)
    {
        var target = world!.Map.Subsectors[subSector];

        AddSprites(target.Sector, validCount);

        for (var i = 0; i < target.SegCount; i++)
            DrawSeg(world.Map.Segs[target.FirstSeg + i]);
    }

    private static readonly int[][] viewPosToFrustumTangent =
    [
        [3, 0, 2, 1],
        [3, 0, 2, 0],
        [3, 1, 2, 0],
        [0],
        [2, 0, 2, 1],
        [0, 0, 0, 0],
        [3, 1, 3, 0],
        [0],
        [2, 0, 3, 1],
        [2, 1, 3, 1],
        [2, 1, 3, 0]
    ];

    private bool IsPotentiallyVisible(Fixed[] bbox)
    {

        // Find the corners of the box that define the edges from
        // current viewpoint.
        int bx;
        if (viewX <= bbox[Box.Left])
            bx = 0;
        else if (viewX < bbox[Box.Right])
            bx = 1;
        else
            bx = 2;

        int by;
        if (viewY >= bbox[Box.Top])
            by = 0;
        else if (viewY > bbox[Box.Bottom])
            by = 1;
        else
            by = 2;

        var viewPos = (by << 2) + bx;
        if (viewPos == 5)
            return true;

        var frustumTangent = viewPosToFrustumTangent[viewPos];
        var x1 = bbox[frustumTangent[0]];
        var y1 = bbox[frustumTangent[1]];
        var x2 = bbox[frustumTangent[2]];
        var y2 = bbox[frustumTangent[3]];

        // Check clip list for an open space.
        var angle1 = Geometry.PointToAngle(viewX, viewY, x1, y1) - viewAngle;
        var angle2 = Geometry.PointToAngle(viewX, viewY, x2, y2) - viewAngle;

        var span = angle1 - angle2;

        // Sitting on a line?
        if (span >= Angle.Ang180)
            return true;

        var tSpan1 = angle1 + wallRender.ClipAngle;

        if (tSpan1 > wallRender.ClipAngle2)
        {
            tSpan1 -= wallRender.ClipAngle2;

            // Totally off the left edge?
            if (tSpan1 >= span)
                return false;

            angle1 = wallRender.ClipAngle;
        }

        var tSpan2 = wallRender.ClipAngle - angle2;
        if (tSpan2 > wallRender.ClipAngle2)
        {
            tSpan2 -= wallRender.ClipAngle2;

            // Totally off the left edge?
            if (tSpan2 >= span)
                return false;

            angle2 = -wallRender.ClipAngle;
        }

        // Find the first clippost that touches the source post
        // (adjacent pixels are touching).
        var sx1 = wallRender.AngleToX[(angle1 + Angle.Ang90).Data >> Trig.AngleToFineShift];
        var sx2 = wallRender.AngleToX[(angle2 + Angle.Ang90).Data >> Trig.AngleToFineShift];

        // Does not cross a pixel.
        if (sx1 == sx2)
            return false;

        sx2--;

        var start = 0;
        while (renderingHistory.ClipRanges[start].Last < sx2)
            start++;

        // The clippost contains the new span.
        if (sx1 >= renderingHistory.ClipRanges[start].First && sx2 <= renderingHistory.ClipRanges[start].Last)
            return false;

        return true;
    }


    private void DrawSeg(Seg seg)
    {
        // OPTIMIZE: quickly reject orthogonal back sides.
        var angle1 = Geometry.PointToAngle(viewX, viewY, seg.Vertex1.X, seg.Vertex1.Y);
        var angle2 = Geometry.PointToAngle(viewX, viewY, seg.Vertex2.X, seg.Vertex2.Y);

        // Clip to view edges.
        // OPTIMIZE: make constant out of 2 * wallRender.ClipAngle (FIELDOFVIEW).
        var span = angle1 - angle2;

        // Back side? I.e. backface culling?
        if (span >= Angle.Ang180)
            return;

        // Global angle needed by segcalc.
        var rwAngle1 = angle1;

        angle1 -= viewAngle;
        angle2 -= viewAngle;

        var tSpan1 = angle1 + wallRender.ClipAngle;
        if (tSpan1 > wallRender.ClipAngle2)
        {
            tSpan1 -= wallRender.ClipAngle2;

            // Totally off the left edge?
            if (tSpan1 >= span)
                return;

            angle1 = wallRender.ClipAngle;
        }

        var tSpan2 = wallRender.ClipAngle - angle2;
        if (tSpan2 > wallRender.ClipAngle2)
        {
            tSpan2 -= wallRender.ClipAngle2;

            // Totally off the left edge?
            if (tSpan2 >= span)
                return;

            angle2 = -wallRender.ClipAngle;
        }

        // The seg is in the view range, but not necessarily visible.
        var x1 = wallRender.AngleToX[(angle1 + Angle.Ang90).Data >> Trig.AngleToFineShift];
        var x2 = wallRender.AngleToX[(angle2 + Angle.Ang90).Data >> Trig.AngleToFineShift];

        // Does not cross a pixel?
        if (x1 == x2)
            return;

        var frontSector = seg.FrontSector;
        var backSector = seg.BackSector;

        var frontSectorFloorHeight = frontSector!.GetInterpolatedFloorHeight(frameFrac);
        var frontSectorCeilingHeight = frontSector.GetInterpolatedCeilingHeight(frameFrac);

        // Single sided line?
        if (backSector == null)
        {
            DrawSolidWall(seg, rwAngle1, x1, x2 - 1);
            return;
        }

        var backSectorFloorHeight = backSector.GetInterpolatedFloorHeight(frameFrac);
        var backSectorCeilingHeight = backSector.GetInterpolatedCeilingHeight(frameFrac);

        // Closed door.
        if (backSectorCeilingHeight <= frontSectorFloorHeight || backSectorFloorHeight >= frontSectorCeilingHeight)
        {
            DrawSolidWall(seg, rwAngle1, x1, x2 - 1);
            return;
        }

        // Window.
        if (backSectorCeilingHeight != frontSectorCeilingHeight || backSectorFloorHeight != frontSectorFloorHeight)
        {
            DrawPassWall(seg, rwAngle1, x1, x2 - 1);
            return;
        }

        // Reject empty lines used for triggers and special events.
        // Identical floor and ceiling on both sides, identical
        // light levels on both sides, and no middle texture.
        if (backSector.CeilingFlat == frontSector.CeilingFlat &&
            backSector.FloorFlat == frontSector.FloorFlat &&
            backSector.LightLevel == frontSector.LightLevel &&
            seg.SideDef.MiddleTexture == 0)
        {
            return;
        }

        DrawPassWall(seg, rwAngle1, x1, x2 - 1);
    }

    private void DrawSolidWall(Seg seg, Angle rwAngle1, int x1, int x2)
    {
        int next;
        var start = 0;
        var clipRanges = renderingHistory.ClipRanges.AsSpan();

        // Find the first range that touches the range
        // (adjacent pixels are touching).
        while (clipRanges[start].Last < x1 - 1)
            start++;

        if (x1 < clipRanges[start].First)
        {
            if (x2 < renderingHistory.ClipRanges[start].First - 1)
            {
                // Post is entirely visible (above start),
                // so insert a new clippost.
                DrawSolidWallRange(seg, rwAngle1, x1, x2);
                next = renderingHistory.ClipRangeCount++;

                while (next != start)
                {
                    clipRanges[next].CopyFrom(clipRanges[next - 1]);
                    next--;
                }

                clipRanges[next].First = x1;
                clipRanges[next].Last = x2;
                return;
            }

            // There is a fragment above *start.
            DrawSolidWallRange(seg, rwAngle1, x1, clipRanges[start].First - 1);

            // Now adjust the clip size.
            clipRanges[start].First = x1;
        }

        // Bottom contained in start?
        if (x2 <= clipRanges[start].Last)
            return;

        next = start;
        while (x2 >= clipRanges[next + 1].First - 1)
        {
            // There is a fragment between two posts.
            DrawSolidWallRange(seg, rwAngle1, clipRanges[next].Last + 1, clipRanges[next + 1].First - 1);
            next++;

            if (x2 <= clipRanges[next].Last)
            {
                // Bottom is contained in next.
                // Adjust the clip size.
                clipRanges[start].Last = clipRanges[next].Last;
                goto crunch;
            }
        }

        // There is a fragment after *next.
        DrawSolidWallRange(seg, rwAngle1, clipRanges[next].Last + 1, x2);

        // Adjust the clip size.
        clipRanges[start].Last = x2;

        // Remove start + 1 to next from the clip list,
        // because start now covers their area.
    crunch:
        // Post just extended past the bottom of one post.
        if (next == start)
            return;

        // Remove a post.
        while (next++ != renderingHistory.ClipRangeCount)
            clipRanges[++start].CopyFrom(clipRanges[next]);

        renderingHistory.ClipRangeCount = start + 1;
    }

    private void DrawPassWall(Seg seg, Angle rwAngle1, int x1, int x2)
    {
        // Find the first range that touches the range
        // (adjacent pixels are touching).
        var start = 0;
        var clipRanges = renderingHistory.ClipRanges.AsSpan();

        while (clipRanges[start].Last < x1 - 1)
            start++;

        if (x1 < clipRanges[start].First)
        {
            if (x2 < clipRanges[start].First - 1)
            {
                // Post is entirely visible (above start).
                DrawPassWallRange(seg, rwAngle1, x1, x2, false);
                return;
            }

            // There is a fragment above *start.
            DrawPassWallRange(seg, rwAngle1, x1, clipRanges[start].First - 1, false);
        }

        // Bottom contained in start?
        if (x2 <= clipRanges[start].Last)
            return;

        while (x2 >= clipRanges[start + 1].First - 1)
        {
            // There is a fragment between two posts.
            DrawPassWallRange(seg, rwAngle1, clipRanges[start].Last + 1, clipRanges[start + 1].First - 1, false);
            start++;

            if (x2 <= clipRanges[start].Last)
                return;
        }

        // There is a fragment after *next.
        DrawPassWallRange(seg, rwAngle1, clipRanges[start].Last + 1, x2, false);
    }

    private Fixed ScaleFromGlobalAngle(Angle visAngle, Angle viewAngle, Angle rwNormal, Fixed rwDistance)
    {
        var num = windowSettings.Projection * Trig.Sin(Angle.Ang90 + (visAngle - rwNormal));
        var den = rwDistance * Trig.Sin(Angle.Ang90 + (visAngle - viewAngle));

        if (den.Data <= num.Data >> 16)
            return Fixed.FromInt(64);

        var scale = num / den;

        if (scale > Fixed.FromInt(64))
            return Fixed.FromInt(64);

        return scale.Data < 256 ? new Fixed(256) : scale;
    }

    private const int heightBits = 12;
    private const int heightUnit = 1 << heightBits;

    private void DrawSolidWallRange(Seg seg, Angle rwAngle1, int x1, int x2)
    {
        if (seg.BackSector is not null)
        {
            DrawPassWallRange(seg, rwAngle1, x1, x2, true);
            return;
        }

        // Too many visible walls.
        if (renderingHistory.VisWallRangeCount == renderingHistory.VisWallRanges.Length)
            return;

        // Make some aliases to shorten the following code.
        var line = seg.LineDef;
        var side = seg.SideDef;
        var frontSector = seg.FrontSector;

        var frontSectorFloorHeight = frontSector!.GetInterpolatedFloorHeight(frameFrac);
        var frontSectorCeilingHeight = frontSector.GetInterpolatedCeilingHeight(frameFrac);

        // Mark the segment as visible for auto map.
        line!.Flags |= LineFlags.Mapped;

        // Calculate the relative plane heights of front and back sector.
        var worldFrontZ1 = frontSectorCeilingHeight - viewZ;
        var worldFrontZ2 = frontSectorFloorHeight - viewZ;

        // Check which parts must be rendered.
        var drawWall = side!.MiddleTexture != 0;
        var drawCeiling = worldFrontZ1 > Fixed.Zero || frontSector.CeilingFlat == flats.SkyFlatNumber;
        var drawFloor = worldFrontZ2 < Fixed.Zero;

        //
        // Determine how the wall textures are vertically aligned.
        //

        var wallTexture = textures[world.Specials.TextureTranslation[side.MiddleTexture]];
        var wallWidthMask = wallTexture.Width - 1;

        Fixed middleTextureAlt;
        if ((line.Flags & LineFlags.DontPegBottom) != 0)
        {
            var vTop = frontSectorFloorHeight + Fixed.FromInt(wallTexture.Height);
            middleTextureAlt = vTop - viewZ;
        }
        else
            middleTextureAlt = worldFrontZ1;

        middleTextureAlt += side.RowOffset;

        //
        // Calculate the scaling factors of the left and right edges of the wall range.
        //

        var rwNormalAngle = seg.Angle + Angle.Ang90;

        var offsetAngle = Angle.Abs(rwNormalAngle - rwAngle1);
        if (offsetAngle > Angle.Ang90)
            offsetAngle = Angle.Ang90;

        var distAngle = Angle.Ang90 - offsetAngle;

        var hypotenuse = Geometry.PointToDist(viewX, viewY, seg.Vertex1.X, seg.Vertex1.Y);

        var rwDistance = hypotenuse * Trig.Sin(distAngle);

        var rwScale = ScaleFromGlobalAngle(viewAngle + wallRender.XToAngle[x1], viewAngle, rwNormalAngle, rwDistance);

        var scale1 = rwScale;
        Fixed scale2;
        Fixed rwScaleStep;
        if (x2 > x1)
        {
            scale2 = ScaleFromGlobalAngle(viewAngle + wallRender.XToAngle[x2], viewAngle, rwNormalAngle, rwDistance);
            rwScaleStep = (scale2 - rwScale) / (x2 - x1);
        }
        else
        {
            scale2 = scale1;
            rwScaleStep = Fixed.Zero;
        }

        //
        // Determine how the wall textures are horizontally aligned
        // and which color map is used according to the light level (if necessary).
        //

        var textureOffsetAngle = rwNormalAngle - rwAngle1;
        if (textureOffsetAngle > Angle.Ang180)
            textureOffsetAngle = -textureOffsetAngle;

        if (textureOffsetAngle > Angle.Ang90)
            textureOffsetAngle = Angle.Ang90;

        var rwOffset = hypotenuse * Trig.Sin(textureOffsetAngle);
        if (rwNormalAngle - rwAngle1 < Angle.Ang180)
            rwOffset = -rwOffset;

        rwOffset += seg.Offset + side.TextureOffset;

        var rwCenterAngle = Angle.Ang90 + viewAngle - rwNormalAngle;

        var wallLightLevel = (frontSector.LightLevel >> LightningRender.lightSegShift) + lightningRender.ExtraLight;
        if (seg.Vertex1.Y == seg.Vertex2.Y)
            wallLightLevel--;
        else if (seg.Vertex1.X == seg.Vertex2.X)
            wallLightLevel++;

        var wallLights = lightningRender.scaleLight[Math.Clamp(wallLightLevel, 0, LightningRender.lightLevelCount - 1)];

        //
        // Determine where on the screen the wall is drawn.
        //

        // These values are right shifted to avoid overflow in the following process (maybe).
        worldFrontZ1 >>= 4;
        worldFrontZ2 >>= 4;

        // The Y positions of the top / bottom edges of the wall on the screen.
        var wallY1Frac = (windowSettings.CenterYFrac >> 4) - worldFrontZ1 * rwScale;
        var wallY1Step = -(rwScaleStep * worldFrontZ1);
        var wallY2Frac = (windowSettings.CenterYFrac >> 4) - worldFrontZ2 * rwScale;
        var wallY2Step = -(rwScaleStep * worldFrontZ2);

        //
        // Determine which color map is used for the plane according to the light level.
        //

        var planeLightLevel = (frontSector.LightLevel >> LightningRender.lightSegShift) + lightningRender.ExtraLight;
        var planeLights = lightningRender.zLight[Math.Clamp(planeLightLevel, 0, LightningRender.lightLevelCount - 1)];

        //
        // Prepare to record the rendering history.
        //

        ref var visWallRange = ref renderingHistory.GetAndIncrementVisWallRange();

        visWallRange.Seg = seg;
        visWallRange.X1 = x1;
        visWallRange.X2 = x2;
        visWallRange.Scale1 = scale1;
        visWallRange.Scale2 = scale2;
        visWallRange.ScaleStep = rwScaleStep;
        visWallRange.Silhouette = Silhouette.Both;
        visWallRange.LowerSilHeight = Fixed.MaxValue;
        visWallRange.UpperSilHeight = Fixed.MinValue;
        visWallRange.MaskedTextureColumn = -1;
        visWallRange.UpperClip = renderingHistory.WindowHeightArray;
        visWallRange.LowerClip = renderingHistory.NegOneArray;
        visWallRange.FrontSectorFloorHeight = frontSectorFloorHeight;
        visWallRange.FrontSectorCeilingHeight = frontSectorCeilingHeight;

        //
        // Floor and ceiling.
        //

        var ceilingFlat = flats[world.Specials.FlatTranslation[frontSector.CeilingFlat]];
        var floorFlat = flats[world.Specials.FlatTranslation[frontSector.FloorFlat]];

        //
        // Now the rendering is carried out.
        //

        for (var x = x1; x <= x2; x++)
        {
            var drawWallY1 = (wallY1Frac.Data + heightUnit - 1) >> heightBits;
            var drawWallY2 = wallY2Frac.Data >> heightBits;

            if (drawCeiling)
            {
                var cy1 = renderingHistory.UpperClip[x] + 1;
                var cy2 = Math.Min(drawWallY1 - 1, renderingHistory.LowerClip[x] - 1);
                DrawCeilingColumn(frontSector, ceilingFlat, planeLights, x, cy1, cy2, frontSectorCeilingHeight);
            }

            if (drawWall)
            {
                var wy1 = Math.Max(drawWallY1, renderingHistory.UpperClip[x] + 1);
                var wy2 = Math.Min(drawWallY2, renderingHistory.LowerClip[x] - 1);

                var angle = rwCenterAngle + wallRender.XToAngle[x];
                angle = new Angle(angle.Data & 0x7FFFFFFF);

                var textureColumn = (rwOffset - Trig.Tan(angle) * rwDistance).ToIntFloor();
                var source = wallTexture.Composite.Columns[textureColumn & wallWidthMask];

                if (source.Length > 0)
                {
                    var lightIndex = rwScale.Data >> LightningRender.scaleLightShift;
                    if (lightIndex >= lightningRender.MaxScaleLight)
                        lightIndex = lightningRender.MaxScaleLight - 1;

                    var invScale = new Fixed((int)(0xffffffffu / (uint)rwScale.Data));
                    DrawColumn(source[0], wallLights[lightIndex], x, wy1, wy2, invScale, middleTextureAlt);
                }
            }

            if (drawFloor)
            {
                var fy1 = Math.Max(drawWallY2 + 1, renderingHistory.UpperClip[x] + 1);
                var fy2 = renderingHistory.LowerClip[x] - 1;
                DrawFloorColumn(frontSector, floorFlat, planeLights, x, fy1, fy2, frontSectorFloorHeight);
            }

            rwScale += rwScaleStep;
            wallY1Frac += wallY1Step;
            wallY2Frac += wallY2Step;
        }
    }


    private void DrawPassWallRange(Seg seg, Angle rwAngle1, int x1, int x2, bool drawAsSolidWall)
    {
        // Too many visible walls.
        if (renderingHistory.VisualRangeExceeded())
            return;

        var range = x2 - x1 + 1;

        // Clip info buffer is not sufficient.
        if (renderingHistory.IsClipIntoBufferSufficient(range))
            return;

        // Make some aliases to shorten the following code.
        var line = seg.LineDef;

        if (line is null)
            return;

        var side = seg.SideDef;

        if (side is null)
            return;

        var frontSector = seg.FrontSector;

        if (frontSector is null)
            return;

        var backSector = seg.BackSector;

        if (backSector is null)
            return;

        var frontSectorFloorHeight = frontSector.GetInterpolatedFloorHeight(frameFrac);
        var frontSectorCeilingHeight = frontSector.GetInterpolatedCeilingHeight(frameFrac);
        var backSectorFloorHeight = backSector.GetInterpolatedFloorHeight(frameFrac);
        var backSectorCeilingHeight = backSector.GetInterpolatedCeilingHeight(frameFrac);

        // Mark the segment as visible for auto map.
        line.Flags |= LineFlags.Mapped;

        // Calculate the relative plane heights of front and back sector.
        // These values are later 4 bits right shifted to calculate the rendering area.
        var worldFrontZ1 = frontSectorCeilingHeight - viewZ;
        var worldFrontZ2 = frontSectorFloorHeight - viewZ;
        var worldBackZ1 = backSectorCeilingHeight - viewZ;
        var worldBackZ2 = backSectorFloorHeight - viewZ;

        // The hack below enables ceiling height change in outdoor area without showing the upper wall.
        if (frontSector.CeilingFlat == flats.SkyFlatNumber && backSector.CeilingFlat == flats.SkyFlatNumber)
            worldFrontZ1 = worldBackZ1;

        //
        // Check which parts must be rendered.
        //

        bool drawUpperWall;
        bool drawCeiling;
        if (drawAsSolidWall ||
            worldFrontZ1 != worldBackZ1 ||
            frontSector.CeilingFlat != backSector.CeilingFlat ||
            frontSector.LightLevel != backSector.LightLevel)
        {
            drawUpperWall = side.TopTexture != 0 && worldBackZ1 < worldFrontZ1;
            drawCeiling = worldFrontZ1 >= Fixed.Zero || frontSector.CeilingFlat == flats.SkyFlatNumber;
        }
        else
        {
            drawUpperWall = false;
            drawCeiling = false;
        }

        bool drawLowerWall;
        bool drawFloor;
        if (drawAsSolidWall ||
            worldFrontZ2 != worldBackZ2 ||
            frontSector.FloorFlat != backSector.FloorFlat ||
            frontSector.LightLevel != backSector.LightLevel)
        {
            drawLowerWall = side.BottomTexture != 0 && worldBackZ2 > worldFrontZ2;
            drawFloor = worldFrontZ2 <= Fixed.Zero;
        }
        else
        {
            drawLowerWall = false;
            drawFloor = false;
        }

        var drawMaskedTexture = side.MiddleTexture != 0;

        // If nothing must be rendered, we can skip this seg.
        if (!drawUpperWall && !drawCeiling && !drawLowerWall && !drawFloor && !drawMaskedTexture)
            return;

        var segTextured = drawUpperWall || drawLowerWall || drawMaskedTexture;

        //
        // Determine how the wall textures are vertically aligned (if necessary).
        //

        var upperWallTexture = default(Texture);
        var upperWallWidthMask = 0;
        var upperTextureAlt = default(Fixed);
        if (drawUpperWall)
        {
            upperWallTexture = textures[world!.Specials.TextureTranslation[side.TopTexture]];
            upperWallWidthMask = upperWallTexture.Width - 1;

            if ((line.Flags & LineFlags.DontPegTop) != 0)
                upperTextureAlt = worldFrontZ1;
            else
            {
                var vTop = backSectorCeilingHeight + Fixed.FromInt(upperWallTexture.Height);
                upperTextureAlt = vTop - viewZ;
            }

            upperTextureAlt += side.RowOffset;
        }

        var lowerWallTexture = default(Texture);
        var lowerWallWidthMask = 0;
        var lowerTextureAlt = default(Fixed);
        if (drawLowerWall)
        {
            lowerWallTexture = textures[world!.Specials.TextureTranslation[side.BottomTexture]];
            lowerWallWidthMask = lowerWallTexture.Width - 1;
            lowerTextureAlt = (line.Flags & LineFlags.DontPegBottom) != 0
                ? worldFrontZ1 + side.RowOffset
                : worldBackZ2 + side.RowOffset;
        }

        //
        // Calculate the scaling factors of the left and right edges of the wall range.
        //

        var rwNormalAngle = seg.Angle + Angle.Ang90;

        var offsetAngle = Angle.Abs(rwNormalAngle - rwAngle1);
        if (offsetAngle > Angle.Ang90)
            offsetAngle = Angle.Ang90;

        var distAngle = Angle.Ang90 - offsetAngle;

        var hypotenuse = Geometry.PointToDist(viewX, viewY, seg.Vertex1.X, seg.Vertex1.Y);

        var rwDistance = hypotenuse * Trig.Sin(distAngle);

        var rwScale = ScaleFromGlobalAngle(viewAngle + wallRender.XToAngle[x1], viewAngle, rwNormalAngle, rwDistance);

        var scale1 = rwScale;
        Fixed scale2;
        Fixed rwScaleStep;
        if (x2 > x1)
        {
            scale2 = ScaleFromGlobalAngle(viewAngle + wallRender.XToAngle[x2], viewAngle, rwNormalAngle, rwDistance);
            rwScaleStep = (scale2 - rwScale) / (x2 - x1);
        }
        else
        {
            scale2 = scale1;
            rwScaleStep = Fixed.Zero;
        }

        //
        // Determine how the wall textures are horizontally aligned
        // and which color map is used according to the light level (if necessary).
        //

        var rwOffset = default(Fixed);
        var rwCenterAngle = default(Angle);
        var wallLights = default(byte[][]);
        if (segTextured)
        {
            var textureOffsetAngle = rwNormalAngle - rwAngle1;
            if (textureOffsetAngle > Angle.Ang180)
                textureOffsetAngle = -textureOffsetAngle;

            if (textureOffsetAngle > Angle.Ang90)
                textureOffsetAngle = Angle.Ang90;

            rwOffset = hypotenuse * Trig.Sin(textureOffsetAngle);
            if (rwNormalAngle - rwAngle1 < Angle.Ang180)
                rwOffset = -rwOffset;

            rwOffset += seg.Offset + side.TextureOffset;

            rwCenterAngle = Angle.Ang90 + viewAngle - rwNormalAngle;

            var wallLightLevel = (frontSector.LightLevel >> LightningRender.lightSegShift) + lightningRender.ExtraLight;
            if (seg.Vertex1.Y == seg.Vertex2.Y)
                wallLightLevel--;
            else if (seg.Vertex1.X == seg.Vertex2.X)
                wallLightLevel++;

            var scaleLightIndex = Math.Clamp(wallLightLevel, 0, LightningRender.lightLevelCount - 1);
            wallLights = lightningRender.scaleLight[scaleLightIndex];
        }

        //
        // Determine where on the screen the wall is drawn.
        //

        // These values are right shifted to avoid overflow in the following process.
        worldFrontZ1 >>= 4;
        worldFrontZ2 >>= 4;
        worldBackZ1 >>= 4;
        worldBackZ2 >>= 4;

        // The Y positions of the top / bottom edges of the wall on the screen..
        var wallY1Frac = (windowSettings.CenterYFrac >> 4) - worldFrontZ1 * rwScale;
        var wallY1Step = -(rwScaleStep * worldFrontZ1);
        var wallY2Frac = (windowSettings.CenterYFrac >> 4) - worldFrontZ2 * rwScale;
        var wallY2Step = -(rwScaleStep * worldFrontZ2);

        // The Y position of the top edge of the portal (if visible).
        var portalY1Frac = default(Fixed);
        var portalY1Step = default(Fixed);
        if (drawUpperWall)
        {
            if (worldBackZ1 > worldFrontZ2)
            {
                portalY1Frac = (windowSettings.CenterYFrac >> 4) - worldBackZ1 * rwScale;
                portalY1Step = -(rwScaleStep * worldBackZ1);
            }
            else
            {
                portalY1Frac = (windowSettings.CenterYFrac >> 4) - worldFrontZ2 * rwScale;
                portalY1Step = -(rwScaleStep * worldFrontZ2);
            }
        }

        // The Y position of the bottom edge of the portal (if visible).
        var portalY2Frac = default(Fixed);
        var portalY2Step = default(Fixed);
        if (drawLowerWall)
        {
            if (worldBackZ2 < worldFrontZ1)
            {
                portalY2Frac = (windowSettings.CenterYFrac >> 4) - worldBackZ2 * rwScale;
                portalY2Step = -(rwScaleStep * worldBackZ2);
            }
            else
            {
                portalY2Frac = (windowSettings.CenterYFrac >> 4) - worldFrontZ1 * rwScale;
                portalY2Step = -(rwScaleStep * worldFrontZ1);
            }
        }

        //
        // Determine which color map is used for the plane according to the light level.
        //

        var planeLightLevel = (frontSector.LightLevel >> LightningRender.lightSegShift) + lightningRender.ExtraLight;
        var planeLights = lightningRender.zLight[Math.Clamp(planeLightLevel, 0, LightningRender.lightLevelCount - 1)];

        //
        // Prepare to record the rendering history.
        //

        ref var visWallRange = ref renderingHistory.GetAndIncrementVisWallRange();

        visWallRange.Seg = seg;
        visWallRange.X1 = x1;
        visWallRange.X2 = x2;
        visWallRange.Scale1 = scale1;
        visWallRange.Scale2 = scale2;
        visWallRange.ScaleStep = rwScaleStep;

        visWallRange.UpperClip = -1;
        visWallRange.LowerClip = -1;

        if (frontSectorFloorHeight > backSectorFloorHeight)
        {
            visWallRange.Silhouette = Silhouette.Lower;
            visWallRange.LowerSilHeight = frontSectorFloorHeight;
        }
        else if (backSectorFloorHeight > viewZ)
        {
            visWallRange.Silhouette = Silhouette.Lower;
            visWallRange.LowerSilHeight = Fixed.MaxValue;
        }
        else
            visWallRange.Silhouette = Silhouette.None;

        if (frontSectorCeilingHeight < backSectorCeilingHeight)
        {
            visWallRange.Silhouette |= Silhouette.Upper;
            visWallRange.UpperSilHeight = frontSectorCeilingHeight;
        }
        else if (backSectorCeilingHeight < viewZ)
        {
            visWallRange.Silhouette |= Silhouette.Upper;
            visWallRange.UpperSilHeight = Fixed.MinValue;
        }

        if (backSectorCeilingHeight <= frontSectorFloorHeight)
        {
            visWallRange.LowerClip = renderingHistory.NegOneArray;
            visWallRange.LowerSilHeight = Fixed.MaxValue;
            visWallRange.Silhouette |= Silhouette.Lower;
        }

        if (backSectorFloorHeight >= frontSectorCeilingHeight)
        {
            visWallRange.UpperClip = renderingHistory.WindowHeightArray;
            visWallRange.UpperSilHeight = Fixed.MinValue;
            visWallRange.Silhouette |= Silhouette.Upper;
        }

        var maskedTextureColumn = 0;
        if (drawMaskedTexture)
        {
            maskedTextureColumn = renderingHistory.ClipDataLength - x1;
            visWallRange.MaskedTextureColumn = maskedTextureColumn;
            renderingHistory.ClipDataLength += range;
        }
        else
            visWallRange.MaskedTextureColumn = -1;

        visWallRange.FrontSectorFloorHeight = frontSectorFloorHeight;
        visWallRange.FrontSectorCeilingHeight = frontSectorCeilingHeight;
        visWallRange.BackSectorFloorHeight = backSectorFloorHeight;
        visWallRange.BackSectorCeilingHeight = backSectorCeilingHeight;

        //
        // Floor and ceiling.
        //

        var ceilingFlat = flats[world.Specials.FlatTranslation[frontSector.CeilingFlat]];
        var floorFlat = flats[world.Specials.FlatTranslation[frontSector.FloorFlat]];

        //
        // Now the rendering is carried out.
        //

        for (var x = x1; x <= x2; x++)
        {
            var drawWallY1 = (wallY1Frac.Data + heightUnit - 1) >> heightBits;
            var drawWallY2 = wallY2Frac.Data >> heightBits;

            var textureColumn = 0;
            var lightIndex = 0;
            var invScale = default(Fixed);
            if (segTextured)
            {
                var angle = rwCenterAngle + wallRender.XToAngle[x];
                angle = new Angle(angle.Data & 0x7FFFFFFF);
                textureColumn = (rwOffset - Trig.Tan(angle) * rwDistance).ToIntFloor();

                lightIndex = rwScale.Data >> LightningRender.scaleLightShift;
                if (lightIndex >= lightningRender.MaxScaleLight)
                    lightIndex = lightningRender.MaxScaleLight - 1;

                invScale = new Fixed((int)(0xffffffffu / (uint)rwScale.Data));
            }

            if (drawUpperWall)
            {
                var drawUpperWallY1 = (wallY1Frac.Data + heightUnit - 1) >> heightBits;
                var drawUpperWallY2 = portalY1Frac.Data >> heightBits;

                if (drawCeiling)
                {
                    var cy1 = renderingHistory.UpperClip[x] + 1;
                    var cy2 = Math.Min(drawWallY1 - 1, renderingHistory.LowerClip[x] - 1);
                    DrawCeilingColumn(frontSector, ceilingFlat, planeLights, x, cy1, cy2, frontSectorCeilingHeight);
                }

                var wy1 = Math.Max(drawUpperWallY1, renderingHistory.UpperClip[x] + 1);
                var wy2 = Math.Min(drawUpperWallY2, renderingHistory.LowerClip[x] - 1);
                var source = upperWallTexture!.Composite.Columns[textureColumn & upperWallWidthMask];
                if (source.Length > 0)
                    DrawColumn(source[0], wallLights![lightIndex], x, wy1, wy2, invScale, upperTextureAlt);

                if (renderingHistory.UpperClip[x] < wy2)
                    renderingHistory.UpperClip[x] = (short)wy2;

                portalY1Frac += portalY1Step;
            }
            else if (drawCeiling)
            {
                var cy1 = renderingHistory.UpperClip[x] + 1;
                var cy2 = Math.Min(drawWallY1 - 1, renderingHistory.LowerClip[x] - 1);
                DrawCeilingColumn(frontSector, ceilingFlat, planeLights, x, cy1, cy2, frontSectorCeilingHeight);

                if (renderingHistory.UpperClip[x] < cy2)
                    renderingHistory.UpperClip[x] = (short)cy2;
            }

            if (drawLowerWall)
            {
                var drawLowerWallY1 = (portalY2Frac.Data + heightUnit - 1) >> heightBits;
                var drawLowerWallY2 = wallY2Frac.Data >> heightBits;

                var wy1 = Math.Max(drawLowerWallY1, renderingHistory.UpperClip[x] + 1);
                var wy2 = Math.Min(drawLowerWallY2, renderingHistory.LowerClip[x] - 1);
                var source = lowerWallTexture!.Composite.Columns[textureColumn & lowerWallWidthMask];
                if (source.Length > 0)
                    DrawColumn(source[0], wallLights![lightIndex], x, wy1, wy2, invScale, lowerTextureAlt);

                if (drawFloor)
                {
                    var fy1 = Math.Max(drawWallY2 + 1, renderingHistory.UpperClip[x] + 1);
                    var fy2 = renderingHistory.LowerClip[x] - 1;
                    DrawFloorColumn(frontSector, floorFlat, planeLights, x, fy1, fy2, frontSectorFloorHeight);
                }

                if (renderingHistory.LowerClip[x] > wy1)
                    renderingHistory.LowerClip[x] = (short)wy1;

                portalY2Frac += portalY2Step;
            }
            else if (drawFloor)
            {
                var fy1 = Math.Max(drawWallY2 + 1, renderingHistory.UpperClip[x] + 1);
                var fy2 = renderingHistory.LowerClip[x] - 1;
                DrawFloorColumn(frontSector, floorFlat, planeLights, x, fy1, fy2, frontSectorFloorHeight);

                if (renderingHistory.LowerClip[x] > drawWallY2 + 1)
                    renderingHistory.LowerClip[x] = (short)fy1;
            }

            if (drawMaskedTexture)
                renderingHistory.ClipData[maskedTextureColumn + x] = (short)textureColumn;

            rwScale += rwScaleStep;
            wallY1Frac += wallY1Step;
            wallY2Frac += wallY2Step;
        }

        //
        // Save sprite clipping info.
        //

        if (((visWallRange.Silhouette & Silhouette.Upper) != 0 ||
             drawMaskedTexture) && visWallRange.UpperClip == -1)
        {
            // copy upper clip
            var source = renderingHistory.UpperClip.AsSpan(x1, range);
            var dest = renderingHistory.ClipData.AsSpan(renderingHistory.ClipDataLength, range);
            source.CopyTo(dest);

            visWallRange.UpperClip = renderingHistory.ClipDataLength - x1;
            renderingHistory.ClipDataLength += range;
        }

        if (((visWallRange.Silhouette & Silhouette.Lower) != 0 ||
             drawMaskedTexture) && visWallRange.LowerClip == -1)
        {
            // copy lower clip
            var source = renderingHistory.LowerClip.AsSpan(x1, range);
            var dest = renderingHistory.ClipData.AsSpan(renderingHistory.ClipDataLength, range);
            source.CopyTo(dest);

            visWallRange.LowerClip = renderingHistory.ClipDataLength - x1;
            renderingHistory.ClipDataLength += range;
        }

        if (drawMaskedTexture)
        {
            if ((visWallRange.Silhouette & Silhouette.Upper) == 0)
            {
                visWallRange.Silhouette |= Silhouette.Upper;
                visWallRange.UpperSilHeight = Fixed.MinValue;
            }

            if ((visWallRange.Silhouette & Silhouette.Lower) == 0)
            {
                visWallRange.Silhouette |= Silhouette.Lower;
                visWallRange.LowerSilHeight = Fixed.MaxValue;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void RenderMaskedTextures()
    {
        var currentVisWallRanges = renderingHistory.GetCurrentVisWallRanges();
        ref var currentVisWallRangesRef = ref MemoryMarshal.GetReference(currentVisWallRanges);

        for (var i = currentVisWallRanges.Length - 1 - 1; i >= 0; i--)
        {
            ref var drawSeg = ref Unsafe.Add(ref currentVisWallRangesRef, i);
            if (drawSeg.MaskedTextureColumn != -1)
                DrawMaskedRange(drawSeg, drawSeg.X1, drawSeg.X2);
        }
    }

    private void DrawMaskedRange(VisWallRange drawSeg, int x1, int x2)
    {
        var seg = drawSeg.Seg;

        var wallLightLevel = (seg.FrontSector!.LightLevel >> LightningRender.lightSegShift) + lightningRender.ExtraLight;
        if (seg.Vertex1.Y == seg.Vertex2.Y)
            wallLightLevel--;
        else if (seg.Vertex1.X == seg.Vertex2.X)
            wallLightLevel++;

        var wallLights = lightningRender.scaleLight[Math.Clamp(wallLightLevel, 0, LightningRender.lightLevelCount - 1)];

        var wallTexture = textures[world!.Specials.TextureTranslation[seg.SideDef!.MiddleTexture]];
        var mask = wallTexture.Width - 1;

        var midTextureAlt = GetMidTextureAlt(seg, drawSeg, wallTexture, viewZ)
                            + seg.SideDef.RowOffset;

        var scaleStep = drawSeg.ScaleStep;
        var scale = drawSeg.Scale1 + (x1 - drawSeg.X1) * scaleStep;

        for (var x = x1; x <= x2; x++)
        {
            var index = Math.Min(scale.Data >> LightningRender.scaleLightShift, lightningRender.MaxScaleLight - 1);

            var col = renderingHistory.ClipData[drawSeg.MaskedTextureColumn + x];

            if (col != short.MaxValue)
            {
                var topY = windowSettings.CenterYFrac - midTextureAlt * scale;
                var invScale = new Fixed((int)(0xffffffffu / (uint)scale.Data));
                var ceilClip = renderingHistory.ClipData[drawSeg.UpperClip + x];
                var floorClip = renderingHistory.ClipData[drawSeg.LowerClip + x];
                DrawMaskedColumn(
                    wallTexture.Composite.Columns[col & mask],
                    wallLights[index],
                    x,
                    topY,
                    scale,
                    invScale,
                    midTextureAlt,
                    ceilClip,
                    floorClip);

                renderingHistory.ClipData[drawSeg.MaskedTextureColumn + x] = short.MaxValue;
            }

            scale += scaleStep;
        }

        return;

        static Fixed GetMidTextureAlt(Seg seg, VisWallRange drawSeg, Texture wallTexture, Fixed viewZ)
        {
            if ((seg.LineDef!.Flags & LineFlags.DontPegBottom) != 0)
            {
                var midTextureAlt = drawSeg.FrontSectorFloorHeight > drawSeg.BackSectorFloorHeight
                    ? drawSeg.FrontSectorFloorHeight
                    : drawSeg.BackSectorFloorHeight;
                return midTextureAlt + Fixed.FromInt(wallTexture.Height) - viewZ;
            }
            else
            {
                var midTextureAlt = drawSeg.FrontSectorCeilingHeight < drawSeg.BackSectorCeilingHeight
                    ? drawSeg.FrontSectorCeilingHeight
                    : drawSeg.BackSectorCeilingHeight;
                return midTextureAlt - viewZ;
            }
        }
    }

    private void DrawCeilingColumn(
        Sector sector,
        Flat flat,
        byte[][] planeLights,
        int x,
        int y1,
        int y2,
        Fixed ceilingHeight)
    {
        if (flat == flats.SkyFlat)
        {
            DrawSkyColumn(x, y1, y2);
            return;
        }

        if (y2 - y1 < 0)
            return;

        var height = Fixed.Abs(ceilingHeight - viewZ);

        var flatData = flat.Data.AsSpan();

        if (sector == planeRender.CeilingPrevSector && planeRender.CeilingPrevX == x - 1)
        {
            var p1 = Math.Max(y1, planeRender.CeilingPrevY1);
            var p2 = Math.Min(y2, planeRender.CeilingPrevY2);

            var pos = screenHeight * (windowSettings.WindowX + x) + windowSettings.WindowY + y1;

            for (var y = y1; y < p1; y++)
            {
                var distance = height * planeRender.PlaneYSlope[y];
                planeRender.CeilingXStep[y] = distance * planeRender.PlaneBaseXScale;
                planeRender.CeilingYStep[y] = distance * planeRender.PlaneBaseYScale;

                var length = distance * planeRender.PlaneDistScale[x];
                var angle = viewAngle + wallRender.XToAngle[x];
                var xFrac = viewX + Trig.Cos(angle) * length;
                var yFrac = -viewY - Trig.Sin(angle) * length;
                planeRender.CeilingXFrac[y] = xFrac;
                planeRender.CeilingYFrac[y] = yFrac;

                var planeColorMap = planeLights[Math.Min((uint)(distance.Data >> LightningRender.zLightShift), LightningRender.maxZLight - 1)];
                planeRender.CeilingLights[y] = planeColorMap;

                var spot = ((yFrac.Data >> (16 - 6)) & (63 * 64)) + ((xFrac.Data >> 16) & 63);
                screenData[pos] = planeColorMap[flatData[spot]];
                pos++;
            }

            for (var y = p1; y <= p2; y++)
            {
                var xFrac = planeRender.CeilingXFrac[y] + planeRender.CeilingXStep[y];
                var yFrac = planeRender.CeilingYFrac[y] + planeRender.CeilingYStep[y];

                var spot = ((yFrac.Data >> (16 - 6)) & (63 * 64)) + ((xFrac.Data >> 16) & 63);
                screenData[pos] = planeRender.CeilingLights[y][flatData[spot]];
                pos++;

                planeRender.CeilingXFrac[y] = xFrac;
                planeRender.CeilingYFrac[y] = yFrac;
            }

            for (var y = p2 + 1; y <= y2; y++)
            {
                var distance = height * planeRender.PlaneYSlope[y];
                planeRender.CeilingXStep[y] = distance * planeRender.PlaneBaseXScale;
                planeRender.CeilingYStep[y] = distance * planeRender.PlaneBaseYScale;

                var length = distance * planeRender.PlaneDistScale[x];
                var angle = viewAngle + wallRender.XToAngle[x];
                var xFrac = viewX + Trig.Cos(angle) * length;
                var yFrac = -viewY - Trig.Sin(angle) * length;
                planeRender.CeilingXFrac[y] = xFrac;
                planeRender.CeilingYFrac[y] = yFrac;

                var planeColorMap = planeLights[Math.Min((uint)(distance.Data >> LightningRender.zLightShift), LightningRender.maxZLight - 1)];
                planeRender.CeilingLights[y] = planeColorMap;

                var spot = ((yFrac.Data >> (16 - 6)) & (63 * 64)) + ((xFrac.Data >> 16) & 63);
                screenData[pos] = planeColorMap[flatData[spot]];
                pos++;
            }
        }
        else
        {
            var pos = screenHeight * (windowSettings.WindowX + x) + windowSettings.WindowY + y1;

            for (var y = y1; y <= y2; y++)
            {
                var distance = height * planeRender.PlaneYSlope[y];
                planeRender.CeilingXStep[y] = distance * planeRender.PlaneBaseXScale;
                planeRender.CeilingYStep[y] = distance * planeRender.PlaneBaseYScale;

                var length = distance * planeRender.PlaneDistScale[x];
                var angle = viewAngle + wallRender.XToAngle[x];
                var xFrac = viewX + Trig.Cos(angle) * length;
                var yFrac = -viewY - Trig.Sin(angle) * length;
                planeRender.CeilingXFrac[y] = xFrac;
                planeRender.CeilingYFrac[y] = yFrac;

                var planeColorMap = planeLights[Math.Min((uint)(distance.Data >> LightningRender.zLightShift), LightningRender.maxZLight - 1)];
                planeRender.CeilingLights[y] = planeColorMap;

                var spot = ((yFrac.Data >> (16 - 6)) & (63 * 64)) + ((xFrac.Data >> 16) & 63);
                screenData[pos] = planeColorMap[flatData[spot]];
                pos++;
            }
        }

        planeRender.CeilingPrevSector = sector;
        planeRender.CeilingPrevX = x;
        planeRender.CeilingPrevY1 = y1;
        planeRender.CeilingPrevY2 = y2;
    }

    private void DrawFloorColumn(
        Sector sector,
        Flat flat,
        byte[][] planeLights,
        int x,
        int y1,
        int y2,
        Fixed floorHeight)
    {
        if (flat == flats.SkyFlat)
        {
            DrawSkyColumn(x, y1, y2);
            return;
        }

        if (y2 - y1 < 0)
            return;

        var height = Fixed.Abs(floorHeight - viewZ);

        var flatData = flat.Data.AsSpan();

        if (sector == planeRender.FloorPrevSector && planeRender.FloorPrevX == x - 1)
        {
            var p1 = Math.Max(y1, planeRender.FloorPrevY1);
            var p2 = Math.Min(y2, planeRender.FloorPrevY2);

            var pos = screenHeight * (windowSettings.WindowX + x) + windowSettings.WindowY + y1;

            for (var y = y1; y < p1; y++)
            {
                var distance = height * planeRender.PlaneYSlope[y];
                planeRender.FloorXStep[y] = distance * planeRender.PlaneBaseXScale;
                planeRender.FloorYStep[y] = distance * planeRender.PlaneBaseYScale;

                var length = distance * planeRender.PlaneDistScale[x];
                var angle = viewAngle + wallRender.XToAngle[x];
                var xFrac = viewX + Trig.Cos(angle) * length;
                var yFrac = -viewY - Trig.Sin(angle) * length;
                planeRender.FloorXFrac[y] = xFrac;
                planeRender.FloorYFrac[y] = yFrac;

                var planeColorMap = planeLights[Math.Min((uint)(distance.Data >> LightningRender.zLightShift), LightningRender.maxZLight - 1)];
                planeRender.FloorLights[y] = planeColorMap;

                var spot = ((yFrac.Data >> (16 - 6)) & (63 * 64)) + ((xFrac.Data >> 16) & 63);
                screenData[pos] = planeColorMap[flatData[spot]];
                pos++;
            }

            for (var y = p1; y <= p2; y++)
            {
                var xFrac = planeRender.FloorXFrac[y] + planeRender.FloorXStep[y];
                var yFrac = planeRender.FloorYFrac[y] + planeRender.FloorYStep[y];

                var spot = ((yFrac.Data >> (16 - 6)) & (63 * 64)) + ((xFrac.Data >> 16) & 63);
                screenData[pos] = planeRender.FloorLights[y][flatData[spot]];
                pos++;

                planeRender.FloorXFrac[y] = xFrac;
                planeRender.FloorYFrac[y] = yFrac;
            }

            for (var y = p2 + 1; y <= y2; y++)
            {
                var distance = height * planeRender.PlaneYSlope[y];
                planeRender.FloorXStep[y] = distance * planeRender.PlaneBaseXScale;
                planeRender.FloorYStep[y] = distance * planeRender.PlaneBaseYScale;

                var length = distance * planeRender.PlaneDistScale[x];
                var angle = viewAngle + wallRender.XToAngle[x];
                var xFrac = viewX + Trig.Cos(angle) * length;
                var yFrac = -viewY - Trig.Sin(angle) * length;
                planeRender.FloorXFrac[y] = xFrac;
                planeRender.FloorYFrac[y] = yFrac;

                var planeColorMap = planeLights[Math.Min((uint)(distance.Data >> LightningRender.zLightShift), LightningRender.maxZLight - 1)];
                planeRender.FloorLights[y] = planeColorMap;

                var spot = ((yFrac.Data >> (16 - 6)) & (63 * 64)) + ((xFrac.Data >> 16) & 63);
                screenData[pos] = planeColorMap[flatData[spot]];
                pos++;
            }
        }
        else
        {
            var pos = screenHeight * (windowSettings.WindowX + x) + windowSettings.WindowY + y1;

            for (var y = y1; y <= y2; y++)
            {
                var distance = height * planeRender.PlaneYSlope[y];
                planeRender.FloorXStep[y] = distance * planeRender.PlaneBaseXScale;
                planeRender.FloorYStep[y] = distance * planeRender.PlaneBaseYScale;

                var length = distance * planeRender.PlaneDistScale[x];
                var angle = viewAngle + wallRender.XToAngle[x];
                var xFrac = viewX + Trig.Cos(angle) * length;
                var yFrac = -viewY - Trig.Sin(angle) * length;
                planeRender.FloorXFrac[y] = xFrac;
                planeRender.FloorYFrac[y] = yFrac;

                var lightIndex = Math.Min((uint)(distance.Data >> LightningRender.zLightShift), LightningRender.maxZLight - 1);
                var planeColorMap = planeLights[lightIndex];
                planeRender.FloorLights[y] = planeColorMap;

                var spot = ((yFrac.Data >> (16 - 6)) & (63 * 64)) + ((xFrac.Data >> 16) & 63);
                screenData[pos] = planeColorMap[flatData[spot]];
                pos++;
            }
        }

        planeRender.FloorPrevSector = sector;
        planeRender.FloorPrevX = x;
        planeRender.FloorPrevY1 = y1;
        planeRender.FloorPrevY2 = y2;
    }

    private void DrawColumn(
        Column column,
        ReadOnlySpan<byte> map,
        int x,
        int y1,
        int y2,
        Fixed invScale,
        Fixed textureAlt)
    {
        if (y2 - y1 < 0)
            return;

        // Framebuffer destination address.
        // Use ylookup LUT to avoid multiply with ScreenWidth.
        // Use columnofs LUT for subwindows? 
        var pos1 = screenHeight * (windowSettings.WindowX + x) + windowSettings.WindowY + y1;
        var pos2 = pos1 + (y2 - y1);

        // Determine scaling, which is the only mapping to be done.
        var fracStep = invScale;
        var frac = textureAlt + (y1 - windowSettings.CenterY) * fracStep;

        // Inner loop that does the actual texture mapping,
        // e.g. a DDA-lile scaling.
        // This is as fast as it gets.
        var source = column.Data.AsSpan();
        var offset = column.Offset;
        for (var pos = pos1; pos <= pos2; pos++)
        {
            // Re-map color indices from wall texture column
            // using a lighting/special effects LUT.
            var sourceIndex = offset + ((frac.Data >> Fixed.FracBits) & 127);
            var mapIndex = source[sourceIndex];
            screenData[pos] = map[mapIndex];
            frac += fracStep;
        }
    }

    private void DrawColumnTranslation(
        Column column,
        ReadOnlySpan<byte> translation,
        ReadOnlySpan<byte> map,
        int x,
        int y1,
        int y2,
        Fixed invScale,
        Fixed textureAlt)
    {
        if (y2 - y1 < 0)
            return;

        // Framebuffer destination address.
        // Use ylookup LUT to avoid multiply with ScreenWidth.
        // Use columnofs LUT for subwindows? 
        var pos1 = screenHeight * (windowSettings.WindowX + x) + windowSettings.WindowY + y1;
        var pos2 = pos1 + (y2 - y1);

        // Determine scaling, which is the only mapping to be done.
        var fracStep = invScale;
        var frac = textureAlt + (y1 - windowSettings.CenterY) * fracStep;

        // Inner loop that does the actual texture mapping,
        // e.g. a DDA-lile scaling.
        // This is as fast as it gets.
        var source = column.Data.AsSpan();
        var offset = column.Offset;
        for (var pos = pos1; pos <= pos2; pos++)
        {
            // Re-map color indices from wall texture column
            // using a lighting/special effects LUT.
            screenData[pos] = map[translation[source[offset + ((frac.Data >> Fixed.FracBits) & 127)]]];
            frac += fracStep;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DrawFuzzColumn(
        int x,
        int y1,
        int y2)
    {
        if (y2 - y1 < 0)
            return;

        if (y1 == 0)
            y1 = 1;

        if (y2 == windowSettings.WindowHeight - 1)
            y2 = windowSettings.WindowHeight - 2;

        var pos1 = screenHeight * (windowSettings.WindowX + x) + windowSettings.WindowY + y1;
        var pos2 = pos1 + (y2 - y1);

        var mapSpan = colorMap[6].AsSpan();

        for (var pos = pos1; pos <= pos2; pos++)
        {
            var fuzz = FuzzEffectsExtensions.GetAndIncrementPosition(ref fuzzEffectPos);
            screenData[pos] = mapSpan[screenData[pos + fuzz]];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DrawSkyColumn(int x, int y1, int y2)
    {
        var angle = (viewAngle + wallRender.XToAngle[x]).Data >> SkyRender.AngleToSkyShift;
        var skyTexture = world!.Map.SkyTexture;
        var mask = skyTexture.Width - 1;
        var source = skyTexture.Composite.Columns[angle & mask];
        DrawColumn(source[0], colorMap[0], x, y1, y2, skyRender.SkyInvScale, skyRender.SkyTextureAlt);
    }

    private void DrawMaskedColumn(
        Column[] columns,
        ReadOnlySpan<byte> map,
        int x,
        Fixed topY,
        Fixed scale,
        Fixed invScale,
        Fixed textureAlt,
        int upperClip,
        int lowerClip)
    {
        foreach (var column in columns)
        {
            var y1Frac = topY + scale * column.TopDelta;
            var y2Frac = y1Frac + scale * column.Length;
            var y1 = (y1Frac.Data + Fixed.FracUnit - 1) >> Fixed.FracBits;
            var y2 = (y2Frac.Data - 1) >> Fixed.FracBits;

            y1 = Math.Max(y1, upperClip + 1);
            y2 = Math.Min(y2, lowerClip - 1);

            if (y1 <= y2)
            {
                var alt = new Fixed(textureAlt.Data - (column.TopDelta << Fixed.FracBits));
                DrawColumn(column, map, x, y1, y2, invScale, alt);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DrawMaskedColumnTranslation(
        ReadOnlySpan<Column> columns,
        ReadOnlySpan<byte> translation,
        ReadOnlySpan<byte> map,
        int x,
        Fixed topY,
        Fixed scale,
        Fixed invScale,
        Fixed textureAlt,
        int upperClip,
        int lowerClip)
    {
        foreach (var column in columns)
        {
            var y1Frac = topY + scale * column.TopDelta;
            var y2Frac = y1Frac + scale * column.Length;
            var y1 = (y1Frac.Data + Fixed.FracUnit - 1) >> Fixed.FracBits;
            var y2 = (y2Frac.Data - 1) >> Fixed.FracBits;

            y1 = Math.Max(y1, upperClip + 1);
            y2 = Math.Min(y2, lowerClip - 1);

            if (y1 <= y2)
            {
                var alt = new Fixed(textureAlt.Data - (column.TopDelta << Fixed.FracBits));
                DrawColumnTranslation(column, translation, map, x, y1, y2, invScale, alt);
            }
        }
    }

    private void DrawMaskedFuzzColumn(
        ReadOnlySpan<Column> columns,
        int x,
        Fixed topY,
        Fixed scale,
        int upperClip,
        int lowerClip)
    {
        foreach (var column in columns)
        {
            var y1Frac = topY + scale * column.TopDelta;
            var y2Frac = y1Frac + scale * column.Length;
            var y1 = (y1Frac.Data + Fixed.FracUnit - 1) >> Fixed.FracBits;
            var y2 = (y2Frac.Data - 1) >> Fixed.FracBits;

            y1 = Math.Max(y1, upperClip + 1);
            y2 = Math.Min(y2, lowerClip - 1);

            if (y1 <= y2)
                DrawFuzzColumn(x, y1, y2);
        }
    }

    private void AddSprites(Sector sector, int validCount)
    {
        // BSP is traversed by subsector.
        // A sector might have been split into several subsectors during BSP building.
        // Thus, we check whether it's already added.
        if (sector.ValidCount == validCount)
            return;

        // Well, now it will be done.
        sector.ValidCount = validCount;

        var spriteLightLevel = (sector.LightLevel >> LightningRender.lightSegShift) + lightningRender.ExtraLight;
        var spriteLights = lightningRender.scaleLight[Math.Clamp(spriteLightLevel, 0, LightningRender.lightLevelCount - 1)];

        // Handle all things in sector.
        foreach (var thing in sector)
            ProjectSprite(thing, spriteLights);
    }

    private void ProjectSprite(Mobj thing, byte[][] spriteLights)
    {
        if (spriteRender.HasTooManySprites())
            return;

        var thingX = thing.GetInterpolatedX(frameFrac);
        var thingY = thing.GetInterpolatedY(frameFrac);
        var thingZ = thing.GetInterpolatedZ(frameFrac);

        // Transform the origin point.
        var trX = thingX - viewX;
        var trY = thingY - viewY;

        var gxt = (trX * viewCos);
        var gyt = -(trY * viewSin);

        var tz = gxt - gyt;

        // Thing is behind view plane?
        if (tz < SpriteRender.minZ)
            return;

        var xScale = windowSettings.Projection / tz;

        gxt = -trX * viewSin;
        gyt = trY * viewCos;
        var tx = -(gyt + gxt);

        // Too far off the side?
        if (Fixed.Abs(tx) > (tz << 2))
            return;

        var spriteDef = sprites[thing.Sprite];
        var frameNumber = thing.Frame & 0x7F;
        var spriteFrame = spriteDef.Frames[frameNumber];

        Patch lump;
        bool flip;
        if (spriteFrame.Rotate)
        {
            // Choose a different rotation based on player view.
            var ang = Geometry.PointToAngle(viewX, viewY, thingX, thingY);
            var rot = (ang.Data - thing.Angle.Data + Angle.Ang45.Data / 2 * 9) >> 29;
            lump = spriteFrame.Patches[rot];
            flip = spriteFrame.Flip[rot];
        }
        else
        {
            // Use single rotation for all views.
            lump = spriteFrame.Patches[0];
            flip = spriteFrame.Flip[0];
        }

        // Calculate edges of the shape.
        tx -= Fixed.FromInt(lump.LeftOffset);
        var x1 = (windowSettings.CenterXFrac + (tx * xScale)).Data >> Fixed.FracBits;

        // Off the right side?
        if (x1 > windowSettings.WindowWidth)
            return;

        tx += Fixed.FromInt(lump.Width);
        var x2 = ((windowSettings.CenterXFrac + (tx * xScale)).Data >> Fixed.FracBits) - 1;

        // Off the left side?
        if (x2 < 0)
            return;

        // Store information in a vissprite.
        var vis = spriteRender.VisSprites[spriteRender.VisSpriteCount];
        spriteRender.VisSpriteCount++;

        vis.MobjFlags = thing.Flags;
        vis.Scale = xScale;
        vis.GlobalX = thingX;
        vis.GlobalY = thingY;
        vis.GlobalBottomZ = thingZ;
        vis.GlobalTopZ = thingZ + Fixed.FromInt(lump.TopOffset);
        vis.TextureAlt = vis.GlobalTopZ - viewZ;
        vis.X1 = x1 < 0 ? 0 : x1;
        vis.X2 = x2 >= windowSettings.WindowWidth ? windowSettings.WindowWidth - 1 : x2;

        var invScale = Fixed.One / xScale;

        if (flip)
        {
            vis.StartFrac = new Fixed(Fixed.FromInt(lump.Width).Data - 1);
            vis.InvScale = -invScale;
        }
        else
        {
            vis.StartFrac = Fixed.Zero;
            vis.InvScale = invScale;
        }

        if (vis.X1 > x1)
            vis.StartFrac += vis.InvScale * (vis.X1 - x1);

        vis.Patch = lump;

        if (lightningRender.FixedColorMap == 0)
        {
            vis.ColorMap = (thing.Frame & 0x8000) == 0
                ? spriteLights[Math.Min(xScale.Data >> LightningRender.scaleLightShift, lightningRender.MaxScaleLight - 1)]
                : colorMap.FullBright;
        }
        else
            vis.ColorMap = colorMap[lightningRender.FixedColorMap];
    }

    private void RenderSprites()
    {
        var visibleSprites = spriteRender.GetVisibleSprites();
        foreach (var visibleSprite in visibleSprites)
            DrawSprite(visibleSprite);
    }

    private void DrawSprite(VisSprite sprite)
    {
        var lowerClips = renderingHistory.LowerClip.AsSpan();
        var upperClips = renderingHistory.UpperClip.AsSpan();

        for (var x = sprite.X1; x <= sprite.X2; x++)
        {
            lowerClips[x] = -2;
            upperClips[x] = -2;
        }

        var clipDate = renderingHistory.ClipData.AsSpan();

        // Scan drawsegs from end to start for obscuring segs.
        // The first drawseg that has a greater scale is the clip seg.
        var currentVisualWallRanges = renderingHistory.GetCurrentVisWallRanges();
        for (var i = currentVisualWallRanges.Length - 1; i >= 0; i--)
        {
            var wall = currentVisualWallRanges[i];

            // Determine if the drawseg obscures the sprite.
            if (wall.X1 > sprite.X2 || wall.X2 < sprite.X1 || wall is { Silhouette: 0, MaskedTextureColumn: -1 })
                continue;

            var r1 = wall.X1 < sprite.X1 ? sprite.X1 : wall.X1;
            var r2 = wall.X2 > sprite.X2 ? sprite.X2 : wall.X2;

            var (lowScale, scale) = GetScales(wall);

            if (scale < sprite.Scale || (lowScale < sprite.Scale && Geometry.PointOnSegSide(sprite.GlobalX, sprite.GlobalY, wall.Seg) == 0))
            {
                // Masked mid texture?
                if (wall.MaskedTextureColumn != -1)
                    DrawMaskedRange(wall, r1, r2);

                // Seg is behind sprite.
                continue;
            }

            // Clip this piece of the sprite.
            var silhouette = wall.Silhouette;

            if (sprite.GlobalBottomZ >= wall.LowerSilHeight)
                silhouette &= ~Silhouette.Lower;

            if (sprite.GlobalTopZ <= wall.UpperSilHeight)
                silhouette &= ~Silhouette.Upper;

            if (silhouette == Silhouette.Lower)
            {
                // Bottom sil.
                for (var x = r1; x <= r2; x++)
                {
                    if (lowerClips[x] == -2)
                        lowerClips[x] = clipDate[wall.LowerClip + x];
                }
            }
            else if (silhouette == Silhouette.Upper)
            {
                // Top sil.
                for (var x = r1; x <= r2; x++)
                {
                    if (upperClips[x] == -2)
                        upperClips[x] = clipDate[wall.UpperClip + x];
                }
            }
            else if (silhouette == Silhouette.Both)
            {
                // Both.
                for (var x = r1; x <= r2; x++)
                {
                    if (lowerClips[x] == -2)
                        lowerClips[x] = clipDate[wall.LowerClip + x];

                    if (upperClips[x] == -2)
                        upperClips[x] = clipDate[wall.UpperClip + x];
                }
            }
        }

        // All clipping has been performed, so draw the sprite.

        // Check for unclipped columns.
        for (var x = sprite.X1; x <= sprite.X2; x++)
        {
            if (lowerClips[x] == -2)
                lowerClips[x] = (short)windowSettings.WindowHeight;

            if (upperClips[x] == -2)
                upperClips[x] = -1;
        }

        if ((sprite.MobjFlags & MobjFlags.Shadow) != 0)
        {
            var frac = sprite.StartFrac;
            for (var x = sprite.X1; x <= sprite.X2; x++)
            {
                var textureColumn = frac.ToIntFloor();
                DrawMaskedFuzzColumn(
                    sprite.Patch.Columns[textureColumn],
                    x,
                    windowSettings.CenterYFrac - (sprite.TextureAlt * sprite.Scale),
                    sprite.Scale,
                    upperClips[x],
                    lowerClips[x]);
                frac += sprite.InvScale;
            }
        }
        else if ((int)(sprite.MobjFlags & MobjFlags.Translation) >> (int)MobjFlags.TransShift != 0)
        {
            var translation = colorTranslation.GetTranslation(sprite.MobjFlags);
            var frac = sprite.StartFrac;
            for (var x = sprite.X1; x <= sprite.X2; x++)
            {
                var textureColumn = frac.ToIntFloor();
                DrawMaskedColumnTranslation(
                    sprite.Patch.Columns[textureColumn],
                    translation,
                    sprite.ColorMap,
                    x,
                    windowSettings.CenterYFrac - (sprite.TextureAlt * sprite.Scale),
                    sprite.Scale,
                    Fixed.Abs(sprite.InvScale),
                    sprite.TextureAlt,
                    upperClips[x],
                    lowerClips[x]);
                frac += sprite.InvScale;
            }
        }
        else
        {
            var frac = sprite.StartFrac;
            for (var x = sprite.X1; x <= sprite.X2; x++)
            {
                var textureColumn = frac.ToIntFloor();
                DrawMaskedColumn(
                    sprite.Patch.Columns[textureColumn],
                    sprite.ColorMap,
                    x,
                    windowSettings.CenterYFrac - (sprite.TextureAlt * sprite.Scale),
                    sprite.Scale,
                    Fixed.Abs(sprite.InvScale),
                    sprite.TextureAlt,
                    upperClips[x],
                    lowerClips[x]);
                frac += sprite.InvScale;
            }
        }

        return;

        static (Fixed, Fixed) GetScales(VisWallRange wall)
        {
            return wall.Scale1 > wall.Scale2 ? (wall.Scale2, wall.Scale1) : (wall.Scale1, wall.Scale2);
        }
    }

    private void DrawPlayerSprite(PlayerSpriteDef psp, ReadOnlySpan<byte[]> spriteLights, bool fuzz)
    {
        // Decide which patch to use.
        var spriteDef = sprites[psp.State!.Sprite];

        var spriteFrame = spriteDef.Frames[psp.State.Frame & 0x7fff];

        var lump = spriteFrame.Patches[0];
        var flip = spriteFrame.Flip[0];

        // Calculate edges of the shape.
        var tx = psp.Sx - Fixed.FromInt(160);
        tx -= Fixed.FromInt(lump.LeftOffset);
        var x1 = (windowSettings.CenterXFrac + tx * weaponRender.WeaponScale).Data >> Fixed.FracBits;

        // Off the right side?
        if (x1 > windowSettings.WindowWidth)
            return;

        tx += Fixed.FromInt(lump.Width);
        var x2 = ((windowSettings.CenterXFrac + tx * weaponRender.WeaponScale).Data >> Fixed.FracBits) - 1;

        // Off the left side?
        if (x2 < 0)
            return;

        // Store information in a vissprite.
        var vis = weaponRender.WeaponSprite;
        vis.MobjFlags = 0;
        // The code below is based on Crispy Doom's weapon rendering code.
        vis.TextureAlt = Fixed.FromInt(100) + Fixed.One / 4 - (psp.Sy - Fixed.FromInt(lump.TopOffset));
        vis.X1 = x1 < 0 ? 0 : x1;
        vis.X2 = x2 >= windowSettings.WindowWidth ? windowSettings.WindowWidth - 1 : x2;
        vis.Scale = weaponRender.WeaponScale;

        if (flip)
        {
            vis.InvScale = -weaponRender.WeaponInvScale;
            vis.StartFrac = Fixed.FromInt(lump.Width) - new Fixed(1);
        }
        else
        {
            vis.InvScale = weaponRender.WeaponInvScale;
            vis.StartFrac = Fixed.Zero;
        }

        if (vis.X1 > x1)
            vis.StartFrac += vis.InvScale * (vis.X1 - x1);

        vis.Patch = lump;

        if (lightningRender.FixedColorMap == 0)
            vis.ColorMap = (psp.State.Frame & 0x8000) == 0
                ? spriteLights[lightningRender.MaxScaleLight - 1]
                : colorMap.FullBright;
        else
            vis.ColorMap = colorMap[lightningRender.FixedColorMap];

        if (fuzz)
        {
            var frac = vis.StartFrac;
            for (var x = vis.X1; x <= vis.X2; x++)
            {
                var textureColumn = frac.Data >> Fixed.FracBits;
                DrawMaskedFuzzColumn(
                    vis.Patch.Columns[textureColumn],
                    x,
                    windowSettings.CenterYFrac - (vis.TextureAlt * vis.Scale),
                    vis.Scale,
                    -1,
                    windowSettings.WindowHeight);
                frac += vis.InvScale;
            }
        }
        else
        {
            var frac = vis.StartFrac;
            for (var x = vis.X1; x <= vis.X2; x++)
            {
                var textureColumn = frac.Data >> Fixed.FracBits;
                DrawMaskedColumn(
                    vis.Patch.Columns[textureColumn],
                    vis.ColorMap,
                    x,
                    windowSettings.CenterYFrac - (vis.TextureAlt * vis.Scale),
                    vis.Scale,
                    Fixed.Abs(vis.InvScale),
                    vis.TextureAlt,
                    -1,
                    windowSettings.WindowHeight);
                frac += vis.InvScale;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void DrawPlayerSprites(Player player)
    {
        // Get light level.
        var spriteLightLevel = (player.Mobj!.Subsector.Sector.LightLevel >> LightningRender.lightSegShift) + lightningRender.ExtraLight;

        var spriteLights = spriteLightLevel switch
        {
            < 0                                => lightningRender.scaleLight[0],
            >= LightningRender.lightLevelCount => lightningRender.scaleLight[LightningRender.lightLevelCount - 1],
            _                                  => lightningRender.scaleLight[spriteLightLevel]
        };

        // Shadow draw.
        var fuzz = player.Powers[PowerType.Invisibility] > 4 * 32 ||
                   (player.Powers[PowerType.Invisibility] & 8) != 0;

        // Add all active psprites.
        foreach (var psp in player.PlayerSprites.AsSpan())
        {
            if (psp.State != null)
                DrawPlayerSprite(psp, spriteLights, fuzz);
        }
    }

    public int WindowSize
    {
        get => windowSize;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            var changed = windowSize != value;
            if (changed)
            {
                windowSize = value;
                SetWindowSize(windowSize);
            }
        }
    }
}