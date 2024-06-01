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
	public sealed class Animation
	{
		private readonly Intermission im;
		private readonly int number;

		private readonly AnimationType type;
		private readonly int period;
		private readonly int frameCount;
		private readonly string[] patches;
		private int nextTic;

		public Animation(Intermission intermission, AnimationInfo info, int number)
		{
			im = intermission;
			this.number = number;

			type = info.Type;
			period = info.Period;
			frameCount = info.Count;
			LocationX = info.X;
			LocationY = info.Y;
			Data = info.Data;

			patches = new string[frameCount];
			for (var i = 0; i < frameCount; i++)
			{
				// MONDO HACK!
				if (im.Info.Episode != 1 || number != 8)
				{
					patches[i] = "WIA" + im.Info.Episode + number.ToString("00") + i.ToString("00");
				}
				else
				{
					// HACK ALERT!
					patches[i] = "WIA104" + i.ToString("00");
				}
			}
		}

		public void Reset(int bgCount)
		{
			PatchNumber = -1;

			// Specify the next time to draw it.
			if (type == AnimationType.Always)
			{
				nextTic = bgCount + 1 + (im.Random.Next() % period);
			}
			else if (type == AnimationType.Random)
			{
				nextTic = bgCount + 1 + (im.Random.Next() % Data);
			}
			else if (type == AnimationType.Level)
			{
				nextTic = bgCount + 1;
			}
		}

		public void Update(int bgCount)
		{
			if (bgCount == nextTic)
			{
				switch (type)
				{
					case AnimationType.Always:
						if (++PatchNumber >= frameCount)
						{
							PatchNumber = 0;
						}
						nextTic = bgCount + period;
						break;

					case AnimationType.Random:
						PatchNumber++;
						if (PatchNumber == frameCount)
						{
							PatchNumber = -1;
							nextTic = bgCount + (im.Random.Next() % Data);
						}
						else
						{
							nextTic = bgCount + period;
						}
						break;

					case AnimationType.Level:
						// Gawd-awful hack for level anims.
						if (!(im.State == IntermissionState.StatCount && number == 7) && im.Info.NextLevel == Data)
						{
							PatchNumber++;
							if (PatchNumber == frameCount)
							{
								PatchNumber--;
							}
							nextTic = bgCount + period;
						}
						break;
				}
			}
		}

		public int LocationX { get; }

		public int LocationY { get; }

		public int Data { get; }

		public IReadOnlyList<string> Patches => patches;
		public int PatchNumber { get; private set; }
	}
}
