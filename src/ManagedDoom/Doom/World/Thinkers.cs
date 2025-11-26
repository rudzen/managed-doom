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

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ManagedDoom.Doom.World;

public sealed class Thinkers
{
    // Pre-allocate for typical usage
    private readonly List<Thinker> thinkers = new(1024);

    public void Add(Thinker thinker)
    {
        thinkers.Add(thinker);
    }

    public static void Remove(Thinker thinker)
    {
        thinker.ThinkerState = ThinkerState.Removed;
    }

    public void Run()
    {
        var thinkersSpan = CollectionsMarshal.AsSpan(thinkers);
        ref var thinkersRef = ref MemoryMarshal.GetReference(thinkersSpan);

        // Pass 1: Process thinkers in insertion order (FIFO) for correct RNG sequence
        for (var i = 0; i < thinkers.Count; i++)
        {
            ref var thinker = ref Unsafe.Add(ref thinkersRef, i);
            if (thinker.ThinkerState == ThinkerState.Active)
                thinker.Run();
        }

        // Pass 2: Remove dead thinkers after iteration completes
        thinkers.RemoveAll(t => t.ThinkerState == ThinkerState.Removed);
    }

    public void UpdateFrameInterpolationInfo()
    {
        var thinkersSpan = CollectionsMarshal.AsSpan(thinkers);
        ref var thinkersRef = ref MemoryMarshal.GetReference(thinkersSpan);

        // Can iterate forward for read-only operations
        for (var i = 0; i < thinkers.Count; i++)
        {
            ref var thinker = ref Unsafe.Add(ref thinkersRef, i);
            if (thinker is Mobj mobj)
                mobj.UpdateFrameInterpolationInfo();
        }
    }

    public void Reset()
    {
        thinkers.Clear();
    }

    public ThinkerEnumerator GetEnumerator()
    {
        return new ThinkerEnumerator(this);
    }

    public struct ThinkerEnumerator : IEnumerator<Thinker>
    {
        private readonly Thinkers thinkers;
        private int index;

        public ThinkerEnumerator(Thinkers thinkers)
        {
            this.thinkers = thinkers;
            index = -1;
            Current = null!;
        }

        public bool MoveNext()
        {
            while (true)
            {
                index++;

                if (index >= thinkers.thinkers.Count)
                    return false;

                Current = thinkers.thinkers[index];

                if (Current.ThinkerState != ThinkerState.Removed)
                    return true;
            }
        }

        public void Reset()
        {
            index = -1;
            Current = null!;
        }

        public readonly void Dispose()
        {
        }

        public Thinker Current { get; private set; }

        readonly object IEnumerator.Current => Current;
    }
}