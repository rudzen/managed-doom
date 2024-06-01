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

namespace ManagedDoom
{
	public sealed class FloorMove : Thinker
	{
		private readonly World world;

		public FloorMove(World world)
		{
			this.world = world;
		}

		public override void Run()
		{
			SectorActionResult result;

			var sa = world.SectorAction;

			result = sa.MovePlane(
				Sector,
				Speed,
				FloorDestHeight,
				Crush,
				0,
				Direction);

			if (((world.LevelTime + Sector.Number) & 7) == 0)
			{
				world.StartSound(Sector.SoundOrigin, Sfx.STNMOV, SfxType.Misc);
			}

			if (result == SectorActionResult.PastDestination)
			{
				Sector.SpecialData = null;

				if (Direction == 1)
				{
					switch (Type)
					{
						case FloorMoveType.DonutRaise:
							Sector.Special = NewSpecial;
							Sector.FloorFlat = Texture;
							break;
					}
				}
				else if (Direction == -1)
				{
					switch (Type)
					{
						case FloorMoveType.LowerAndChange:
							Sector.Special = NewSpecial;
							Sector.FloorFlat = Texture;
							break;
					}
				}

				world.Thinkers.Remove(this);
				Sector.DisableFrameInterpolationForOneFrame();

				world.StartSound(Sector.SoundOrigin, Sfx.PSTOP, SfxType.Misc);
			}
		}

		public FloorMoveType Type { get; set; }

		public bool Crush { get; set; }

		public Sector Sector { get; set; }

		public int Direction { get; set; }

		public SectorSpecial NewSpecial { get; set; }

		public int Texture { get; set; }

		public Fixed FloorDestHeight { get; set; }

		public Fixed Speed { get; set; }
	}
}
