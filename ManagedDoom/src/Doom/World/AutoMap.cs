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


using System.Collections.Generic;

namespace ManagedDoom
{
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

        public AutoMap(World world)
        {
            this.world = world;

            MinX = Fixed.MaxValue;
            MaxX = Fixed.MinValue;
            MinY = Fixed.MaxValue;
            MaxY = Fixed.MinValue;
            foreach (var vertex in world.Map.Vertices)
            {
                if (vertex.X < MinX)
                {
                    MinX = vertex.X;
                }

                if (vertex.X > MaxX)
                {
                    MaxX = vertex.X;
                }

                if (vertex.Y < MinY)
                {
                    MinY = vertex.Y;
                }

                if (vertex.Y > MaxY)
                {
                    MaxY = vertex.Y;
                }
            }

            ViewX = MinX + (MaxX - MinX) / 2;
            ViewY = MinY + (MaxY - MinY) / 2;

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

            marks = new List<Vertex>();
            nextMarkNumber = 0;
        }

        public void Update()
        {
            if (zoomIn)
            {
                Zoom += Zoom / 16;
            }

            if (zoomOut)
            {
                Zoom -= Zoom / 16;
            }

            if (Zoom < Fixed.One / 2)
            {
                Zoom = Fixed.One / 2;
            }
            else if (Zoom > Fixed.One * 32)
            {
                Zoom = Fixed.One * 32;
            }

            if (left)
            {
                ViewX -= 64 / Zoom;
            }

            if (right)
            {
                ViewX += 64 / Zoom;
            }

            if (up)
            {
                ViewY += 64 / Zoom;
            }

            if (down)
            {
                ViewY -= 64 / Zoom;
            }

            if (ViewX < MinX)
            {
                ViewX = MinX;
            }
            else if (ViewX > MaxX)
            {
                ViewX = MaxX;
            }

            if (ViewY < MinY)
            {
                ViewY = MinY;
            }
            else if (ViewY > MaxY)
            {
                ViewY = MaxY;
            }

            if (Follow)
            {
                var player = world.ConsolePlayer.Mobj;
                ViewX = player.X;
                ViewY = player.Y;
            }
        }

        public bool DoEvent(DoomEvent e)
        {
            if (e.Key is DoomKey.Add or DoomKey.Quote or DoomKey.Equal)
            {
                if (e.Type == EventType.KeyDown)
                {
                    zoomIn = true;
                }
                else if (e.Type == EventType.KeyUp)
                {
                    zoomIn = false;
                }

                return true;
            }

            if (e.Key is DoomKey.Subtract or DoomKey.Hyphen or DoomKey.Semicolon)
            {
                if (e.Type == EventType.KeyDown)
                {
                    zoomOut = true;
                }
                else if (e.Type == EventType.KeyUp)
                {
                    zoomOut = false;
                }

                return true;
            }
            if (e.Key == DoomKey.Left)
            {
                if (e.Type == EventType.KeyDown)
                {
                    left = true;
                }
                else if (e.Type == EventType.KeyUp)
                {
                    left = false;
                }

                return true;
            }
            if (e.Key == DoomKey.Right)
            {
                if (e.Type == EventType.KeyDown)
                {
                    right = true;
                }
                else if (e.Type == EventType.KeyUp)
                {
                    right = false;
                }

                return true;
            }
            if (e.Key == DoomKey.Up)
            {
                if (e.Type == EventType.KeyDown)
                {
                    up = true;
                }
                else if (e.Type == EventType.KeyUp)
                {
                    up = false;
                }

                return true;
            }
            if (e.Key == DoomKey.Down)
            {
                if (e.Type == EventType.KeyDown)
                {
                    down = true;
                }
                else if (e.Type == EventType.KeyUp)
                {
                    down = false;
                }

                return true;
            }
            if (e.Key == DoomKey.F)
            {
                if (e.Type == EventType.KeyDown)
                {
                    Follow = !Follow;
                    if (Follow)
                    {
                        world.ConsolePlayer.SendMessage(DoomInfo.Strings.AMSTR_FOLLOWON);
                    }
                    else
                    {
                        world.ConsolePlayer.SendMessage(DoomInfo.Strings.AMSTR_FOLLOWOFF);
                    }
                    return true;
                }
            }
            else if (e.Key == DoomKey.M)
            {
                if (e.Type == EventType.KeyDown)
                {
                    if (marks.Count < 10)
                    {
                        marks.Add(new Vertex(ViewX, ViewY));
                    }
                    else
                    {
                        marks[nextMarkNumber] = new Vertex(ViewX, ViewY);
                    }
                    nextMarkNumber++;
                    if (nextMarkNumber == 10)
                    {
                        nextMarkNumber = 0;
                    }
                    world.ConsolePlayer.SendMessage(DoomInfo.Strings.AMSTR_MARKEDSPOT);
                    return true;
                }
            }
            else if (e.Key == DoomKey.C)
            {
                if (e.Type == EventType.KeyDown)
                {
                    marks.Clear();
                    nextMarkNumber = 0;
                    world.ConsolePlayer.SendMessage(DoomInfo.Strings.AMSTR_MARKSCLEARED);
                    return true;
                }
            }

            return false;
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
            State++;
            if ((int)State == 3)
            {
                State = AutoMapState.None;
            }
        }

        public Fixed MinX { get; }

        public Fixed MaxX { get; }

        public Fixed MinY { get; }

        public Fixed MaxY { get; }

        public Fixed ViewX { get; private set; }

        public Fixed ViewY { get; private set; }

        public Fixed Zoom { get; private set; }

        public bool Follow { get; private set; }

        public bool Visible { get; private set; }

        public AutoMapState State { get; private set; }

        public IReadOnlyList<Vertex> Marks => marks;
    }
}
