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
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ManagedDoom.Doom.Event;
using ManagedDoom.Doom.Info;
using ManagedDoom.Doom.Map;
using ManagedDoom.Doom.Math;
using ManagedDoom.UserInput;

namespace ManagedDoom.Doom.World;

public sealed class AutoMap
{
    private readonly World world;

    private bool zoomIn;
    private bool zoomOut;

    private bool left;
    private bool right;
    private bool up;
    private bool down;

    private readonly List<Vertex> marks;
    private int nextMarkNumber;

    private readonly Fixed minX;
    private readonly Fixed maxX;
    private readonly Fixed minY;
    private readonly Fixed maxY;

    public AutoMap(World world)
    {
        this.world = world;

        minX = Fixed.MaxValue;
        maxX = Fixed.MinValue;
        minY = Fixed.MaxValue;
        maxY = Fixed.MinValue;
        foreach (var vertex in world.Map.Vertices.AsSpan())
        {
            if (vertex.X < minX)
                minX = vertex.X;

            if (vertex.X > maxX)
                maxX = vertex.X;

            if (vertex.Y < minY)
                minY = vertex.Y;

            if (vertex.Y > maxY)
                maxY = vertex.Y;
        }

        ViewX = minX + (maxX - minX) / 2;
        ViewY = minY + (maxY - minY) / 2;

        Visible = false;
        State = AutoMapState.None;

        Zoom = Fixed.One;
        Follow = true;

        zoomIn = false;
        zoomOut = false;
        left = false;
        right = false;
        up = false;
        down = false;

        marks = new List<Vertex>(10);
        nextMarkNumber = 0;
    }

    public Fixed ViewX { get; private set; }

    public Fixed ViewY { get; private set; }

    public Fixed Zoom { get; private set; }

    public bool Follow { get; private set; }

    public bool Visible { get; private set; }

    public AutoMapState State { get; private set; }

    public IReadOnlyList<Vertex> Marks => marks;

    public void Update()
    {
        UpdateZoom();
        UpdateViewX();
        UpdateViewY();

        if (Follow)
        {
            var player = world.ConsolePlayer.Mobj;
            ViewX = player!.X;
            ViewY = player.Y;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateZoom()
    {
        var zoom = Zoom;
        if (zoomIn)
            zoom += zoom / 16;

        if (zoomOut)
            zoom -= zoom / 16;

        if (zoom < Fixed.One / 2)
            zoom = Fixed.One / 2;
        else if (zoom > Fixed.One * 32)
            zoom = Fixed.One * 32;

        Zoom = zoom;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateViewX()
    {
        var viewX = ViewX;

        if (left)
            viewX -= 64 / Zoom;

        if (right)
            viewX += 64 / Zoom;

        if (viewX < minX)
            viewX = minX;
        else if (viewX > maxX)
            viewX = maxX;

        ViewX = viewX;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateViewY()
    {
        var viewY = ViewY;

        if (up)
            viewY += 64 / Zoom;

        if (down)
            viewY -= 64 / Zoom;

        if (viewY < minY)
            viewY = minY;
        else if (viewY > maxY)
            viewY = maxY;

        ViewY = viewY;
    }

    public bool DoEvent(DoomEvent e)
    {
        if (e.Key is DoomKey.Add or DoomKey.Quote or DoomKey.Equal)
        {
            zoomIn = e.Type switch
            {
                EventType.KeyDown => true,
                EventType.KeyUp   => false,
                _                 => zoomIn
            };

            return true;
        }

        if (e.Key is DoomKey.Subtract or DoomKey.Hyphen or DoomKey.Semicolon)
        {
            zoomOut = e.Type switch
            {
                EventType.KeyDown => true,
                EventType.KeyUp   => false,
                _                 => zoomOut
            };

            return true;
        }

        if (e.Key == DoomKey.Left)
        {
            left = e.Type switch
            {
                EventType.KeyDown => true,
                EventType.KeyUp   => false,
                _                 => left
            };

            return true;
        }

        if (e.Key == DoomKey.Right)
        {
            right = e.Type switch
            {
                EventType.KeyDown => true,
                EventType.KeyUp   => false,
                _                 => right
            };

            return true;
        }

        if (e.Key == DoomKey.Up)
        {
            up = e.Type switch
            {
                EventType.KeyDown => true,
                EventType.KeyUp   => false,
                _                 => up
            };

            return true;
        }

        if (e.Key == DoomKey.Down)
        {
            down = e.Type switch
            {
                EventType.KeyDown => true,
                EventType.KeyUp   => false,
                _                 => down
            };

            return true;
        }

        if (e.Key == DoomKey.F)
        {
            if (e.Type == EventType.KeyDown)
            {
                Follow ^= true;
                var msg = Follow ? DoomInfo.Strings.AMSTR_FOLLOWON : DoomInfo.Strings.AMSTR_FOLLOWOFF;
                world.ConsolePlayer.SendMessage(msg);
                return true;
            }
        }
        else if (e.Key == DoomKey.M)
        {
            if (e.Type == EventType.KeyDown)
            {
                AddMark();
                world.ConsolePlayer.SendMessage(DoomInfo.Strings.AMSTR_MARKEDSPOT);
                return true;
            }
        }
        else if (e is { Key: DoomKey.C, Type: EventType.KeyDown })
        {
            marks.Clear();
            nextMarkNumber = 0;
            world.ConsolePlayer.SendMessage(DoomInfo.Strings.AMSTR_MARKSCLEARED);
            return true;
        }

        return false;
    }

    private void AddMark()
    {
        if (marks.Count < 10)
            marks.Add(new Vertex(ViewX, ViewY));
        else
            marks[nextMarkNumber] = new Vertex(ViewX, ViewY);

        nextMarkNumber++;
        if (nextMarkNumber == 10)
            nextMarkNumber = 0;
    }

    public void Open()
    {
        Visible = true;
    }

    public void Close()
    {
        Visible = false;
        zoomIn = false;
        zoomOut = false;
        left = false;
        right = false;
        up = false;
        down = false;
    }

    public void ToggleCheat()
    {
        if (State == AutoMapState.AllThings)
            State = AutoMapState.None;
        else
            State++;
    }
}