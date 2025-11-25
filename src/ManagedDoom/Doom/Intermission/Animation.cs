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

namespace ManagedDoom.Doom.Intermission;

public sealed class Animation
{
    private readonly Intermission im;
    private readonly int number;

    private readonly AnimationType type;
    private readonly int period;
    private readonly int frameCount;
    private int nextTic;

    private readonly int data;

    public Animation(Intermission intermission, AnimationInfo info, int number)
    {
        im = intermission;
        this.number = number;

        type = info.Type;
        period = info.Period;
        frameCount = info.Count;
        LocationX = info.X;
        LocationY = info.Y;
        data = info.Data;

        Patches = new string[frameCount];
        for (var i = 0; i < frameCount; i++)
        {
            // MONDO HACK!
            if (im.Info.Episode != 1 || number != 8)
                Patches[i] = $"WIA{im.Info.Episode}{number:00}{i:00}";
            // HACK ALERT!
            else
                Patches[i] = $"WIA104{i:00}";
        }
    }

    public int LocationX { get; }
    public int LocationY { get; }
    public string[] Patches { get; }

    public int PatchNumber { get; private set; }

    public void Reset(int bgCount)
    {
        PatchNumber = -1;

        nextTic = type switch
        {
            // Specify the next time to draw it.
            AnimationType.Always => bgCount + 1 + (im.Random.Next() % period),
            AnimationType.Random => bgCount + 1 + (im.Random.Next() % data),
            AnimationType.Level  => bgCount + 1,
            _                    => nextTic
        };
    }

    public void Update(int bgCount)
    {
        if (bgCount != nextTic)
            return;

        if (type == AnimationType.Always)
        {
            if (++PatchNumber >= frameCount)
                PatchNumber = 0;

            nextTic = bgCount + period;
        }
        else if (type == AnimationType.Random)
        {
            PatchNumber++;
            if (PatchNumber == frameCount)
            {
                PatchNumber = -1;
                nextTic = bgCount + (im.Random.Next() % data);
            }
            else
                nextTic = bgCount + period;
        }
        else if (type == AnimationType.Level)
        {
            // Gawd-awful hack for level anims.
            if (!(im.State == IntermissionState.StatCount && number == 7) && im.Info.NextLevel == data)
            {
                PatchNumber++;
                if (PatchNumber == frameCount)
                    PatchNumber--;

                nextTic = bgCount + period;
            }
        }
    }
}