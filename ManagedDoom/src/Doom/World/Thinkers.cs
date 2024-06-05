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
using System.Collections;
using System.Collections.Generic;

namespace ManagedDoom.Doom.World
{
    public sealed class Thinkers
    {
        private World world;

        public Thinkers(World world)
        {
            this.world = world;

            InitThinkers();
        }


        private Thinker cap;

        private void InitThinkers()
        {
            cap = new Thinker();
            cap.Prev = cap.Next = cap;
        }

        public void Add(Thinker thinker)
        {
            cap.Prev.Next = thinker;
            thinker.Next = cap;
            thinker.Prev = cap.Prev;
            cap.Prev = thinker;
        }

        public void Remove(Thinker thinker)
        {
            thinker.ThinkerState = ThinkerState.Removed;
        }

        public void Run()
        {
            var current = cap.Next;
            while (current != cap)
            {
                if (current.ThinkerState == ThinkerState.Removed)
                {
                    // Time to remove it.
                    current.Next.Prev = current.Prev;
                    current.Prev.Next = current.Next;
                }
                else
                {
                    if (current.ThinkerState == ThinkerState.Active)
                    {
                        current.Run();
                    }
                }
                current = current.Next;
            }
        }

        public void UpdateFrameInterpolationInfo()
        {
            var current = cap.Next;
            while (current != cap)
            {
                current.UpdateFrameInterpolationInfo();
                current = current.Next;
            }
        }

        public void Reset()
        {
            cap.Prev = cap.Next = cap;
        }

        public ThinkerEnumerator GetEnumerator()
        {
            return new ThinkerEnumerator(this);
        }



        public struct ThinkerEnumerator : IEnumerator<Thinker>
        {
            private readonly Thinkers thinkers;

            public ThinkerEnumerator(Thinkers thinkers)
            {
                this.thinkers = thinkers;
                Current = thinkers.cap;
            }

            public bool MoveNext()
            {
                while (true)
                {
                    Current = Current.Next;
                    if (Current == thinkers.cap)
                    {
                        return false;
                    }

                    if (Current.ThinkerState != ThinkerState.Removed)
                    {
                        return true;
                    }
                }
            }

            public void Reset()
            {
                Current = thinkers.cap;
            }

            public void Dispose()
            {
            }

            public Thinker Current { get; private set; }

            object IEnumerator.Current => throw new NotImplementedException();
        }
    }
}
