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
	public sealed class CeilingMove : Thinker
	{
		private readonly World world;

		// 1 = up, 0 = waiting, -1 = down.

		// Corresponding sector tag.

		public CeilingMove(World world)
		{
			this.world = world;
		}

		public override void Run()
		{
			SectorActionResult result;

			var sa = world.SectorAction;

			switch (Direction)
			{
				case 0:
					// In statis.
					break;

				case 1:
					// Up.
					result = sa.MovePlane(
						Sector,
						Speed,
						TopHeight,
						false,
						1,
						Direction);

					if (((world.LevelTime + Sector.Number) & 7) == 0)
					{
						switch (Type)
						{
							case CeilingMoveType.SilentCrushAndRaise:
								break;

							default:
								world.StartSound(Sector.SoundOrigin, Sfx.STNMOV, SfxType.Misc);
								break;
						}
					}

					if (result == SectorActionResult.PastDestination)
					{
						switch (Type)
						{
							case CeilingMoveType.RaiseToHighest:
								sa.RemoveActiveCeiling(this);
								Sector.DisableFrameInterpolationForOneFrame();
								break;

							case CeilingMoveType.SilentCrushAndRaise:
							case CeilingMoveType.FastCrushAndRaise:
							case CeilingMoveType.CrushAndRaise:
								if (Type == CeilingMoveType.SilentCrushAndRaise)
								{
									world.StartSound(Sector.SoundOrigin, Sfx.PSTOP, SfxType.Misc);
								}
								Direction = -1;
								break;

							default:
								break;
						}

					}
					break;

				case -1:
					// Down.
					result = sa.MovePlane(
						Sector,
						Speed,
						BottomHeight,
						Crush,
						1,
						Direction);

					if (((world.LevelTime + Sector.Number) & 7) == 0)
					{
						switch (Type)
						{
							case CeilingMoveType.SilentCrushAndRaise:
								break;

							default:
								world.StartSound(Sector.SoundOrigin, Sfx.STNMOV, SfxType.Misc);
								break;
						}
					}

					if (result == SectorActionResult.PastDestination)
					{
						switch (Type)
						{
							case CeilingMoveType.SilentCrushAndRaise:
							case CeilingMoveType.CrushAndRaise:
							case CeilingMoveType.FastCrushAndRaise:
								if (Type == CeilingMoveType.SilentCrushAndRaise)
								{
									world.StartSound(Sector.SoundOrigin, Sfx.PSTOP, SfxType.Misc);
									Speed = SectorAction.CeilingSpeed;
								}
								if (Type == CeilingMoveType.CrushAndRaise)
								{
									Speed = SectorAction.CeilingSpeed;
								}
								Direction = 1;
								break;

							case CeilingMoveType.LowerAndCrush:
							case CeilingMoveType.LowerToFloor:
								sa.RemoveActiveCeiling(this);
								Sector.DisableFrameInterpolationForOneFrame();
								break;

							default:
								break;
						}
					}
					else
					{
						if (result == SectorActionResult.Crushed)
						{
							switch (Type)
							{
								case CeilingMoveType.SilentCrushAndRaise:
								case CeilingMoveType.CrushAndRaise:
								case CeilingMoveType.LowerAndCrush:
									Speed = SectorAction.CeilingSpeed / 8;
									break;

								default:
									break;
							}
						}
					}
					break;
			}
		}

		public CeilingMoveType Type { get; set; }

		public Sector Sector { get; set; }

		public Fixed BottomHeight { get; set; }

		public Fixed TopHeight { get; set; }

		public Fixed Speed { get; set; }

		public bool Crush { get; set; }

		public int Direction { get; set; }

		public int Tag { get; set; }

		public int OldDirection { get; set; }
	}
}
