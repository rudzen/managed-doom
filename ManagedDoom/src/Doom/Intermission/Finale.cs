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
	public sealed class Finale
	{
		public static readonly int TextSpeed = 3;
		public static readonly int TextWait = 250;

		// Stage of animation:
		// 0 = text, 1 = art screen, 2 = character cast.

		// For bunny scroll.

		private UpdateResult updateResult;

		public Finale(GameOptions options)
		{
			this.Options = options;

			string c1Text;
			string c2Text;
			string c3Text;
			string c4Text;
			string c5Text;
			string c6Text;
			switch (options.MissionPack)
			{
				case MissionPack.Plutonia:
					c1Text = DoomInfo.Strings.P1TEXT;
					c2Text = DoomInfo.Strings.P2TEXT;
					c3Text = DoomInfo.Strings.P3TEXT;
					c4Text = DoomInfo.Strings.P4TEXT;
					c5Text = DoomInfo.Strings.P5TEXT;
					c6Text = DoomInfo.Strings.P6TEXT;
					break;

				case MissionPack.Tnt:
					c1Text = DoomInfo.Strings.T1TEXT;
					c2Text = DoomInfo.Strings.T2TEXT;
					c3Text = DoomInfo.Strings.T3TEXT;
					c4Text = DoomInfo.Strings.T4TEXT;
					c5Text = DoomInfo.Strings.T5TEXT;
					c6Text = DoomInfo.Strings.T6TEXT;
					break;

				default:
					c1Text = DoomInfo.Strings.C1TEXT;
					c2Text = DoomInfo.Strings.C2TEXT;
					c3Text = DoomInfo.Strings.C3TEXT;
					c4Text = DoomInfo.Strings.C4TEXT;
					c5Text = DoomInfo.Strings.C5TEXT;
					c6Text = DoomInfo.Strings.C6TEXT;
					break;
			}

			switch (options.GameMode)
			{
				case GameMode.Shareware:
				case GameMode.Registered:
				case GameMode.Retail:
					options.Music.StartMusic(Bgm.VICTOR, true);
					switch (options.Episode)
					{
						case 1:
							Flat = "FLOOR4_8";
							Text = DoomInfo.Strings.E1TEXT;
							break;

						case 2:
							Flat = "SFLR6_1";
							Text = DoomInfo.Strings.E2TEXT;
							break;

						case 3:
							Flat = "MFLR8_4";
							Text = DoomInfo.Strings.E3TEXT;
							break;

						case 4:
							Flat = "MFLR8_3";
							Text = DoomInfo.Strings.E4TEXT;
							break;

						default:
							break;
					}
					break;

				case GameMode.Commercial:
					options.Music.StartMusic(Bgm.READ_M, true);
					switch (options.Map)
					{
						case 6:
							Flat = "SLIME16";
							Text = c1Text;
							break;

						case 11:
							Flat = "RROCK14";
							Text = c2Text;
							break;

						case 20:
							Flat = "RROCK07";
							Text = c3Text;
							break;

						case 30:
							Flat = "RROCK17";
							Text = c4Text;
							break;

						case 15:
							Flat = "RROCK13";
							Text = c5Text;
							break;

						case 31:
							Flat = "RROCK19";
							Text = c6Text;
							break;

						default:
							break;
					}
					break;

				default:
					options.Music.StartMusic(Bgm.READ_M, true);
					Flat = "F_SKY1";
					Text = DoomInfo.Strings.C1TEXT;
					break;
			}

			Stage = 0;
			Count = 0;

			Scrolled = 0;
			ShowTheEnd = false;
			TheEndIndex = 0;
		}

		public UpdateResult Update()
		{
			updateResult = UpdateResult.None;

			// Check for skipping.
			if (Options.GameMode == GameMode.Commercial && Count > 50)
			{
				int i;

				// Go on to the next level.
				for (i = 0; i < Player.MaxPlayerCount; i++)
				{
					if (Options.Players[i].Cmd.Buttons != 0)
					{
						break;
					}
				}

				if (i < Player.MaxPlayerCount && Stage != 2)
				{
					if (Options.Map == 30)
					{
						StartCast();
					}
					else
					{
						return UpdateResult.Completed;
					}
				}
			}

			// Advance animation.
			Count++;

			if (Stage == 2)
			{
				UpdateCast();
				return updateResult;
			}

			if (Options.GameMode == GameMode.Commercial)
			{
				return updateResult;
			}

			if (Stage == 0 && Count > Text.Length * TextSpeed + TextWait)
			{
				Count = 0;
				Stage = 1;
				updateResult = UpdateResult.NeedWipe;
				if (Options.Episode == 3)
				{
					Options.Music.StartMusic(Bgm.BUNNY, true);
				}
			}

			if (Stage == 1 && Options.Episode == 3)
			{
				BunnyScroll();
			}

			return updateResult;
		}

		private void BunnyScroll()
		{
			Scrolled = 320 - (Count - 230) / 2;
			if (Scrolled > 320)
			{
				Scrolled = 320;
			}
			if (Scrolled < 0)
			{
				Scrolled = 0;
			}

			if (Count < 1130)
			{
				return;
			}

			ShowTheEnd = true;

			if (Count < 1180)
			{
				TheEndIndex = 0;
				return;
			}

			var stage = (Count - 1180) / 5;
			if (stage > 6)
			{
				stage = 6;
			}
			if (stage > TheEndIndex)
			{
				StartSound(Sfx.PISTOL);
				TheEndIndex = stage;
			}
		}



		private static readonly CastInfo[] castorder =
		[
			new CastInfo(DoomInfo.Strings.CC_ZOMBIE, MobjType.Possessed),
			new CastInfo(DoomInfo.Strings.CC_SHOTGUN, MobjType.Shotguy),
			new CastInfo(DoomInfo.Strings.CC_HEAVY, MobjType.Chainguy),
			new CastInfo(DoomInfo.Strings.CC_IMP, MobjType.Troop),
			new CastInfo(DoomInfo.Strings.CC_DEMON, MobjType.Sergeant),
			new CastInfo(DoomInfo.Strings.CC_LOST, MobjType.Skull),
			new CastInfo(DoomInfo.Strings.CC_CACO, MobjType.Head),
			new CastInfo(DoomInfo.Strings.CC_HELL, MobjType.Knight),
			new CastInfo(DoomInfo.Strings.CC_BARON, MobjType.Bruiser),
			new CastInfo(DoomInfo.Strings.CC_ARACH, MobjType.Baby),
			new CastInfo(DoomInfo.Strings.CC_PAIN, MobjType.Pain),
			new CastInfo(DoomInfo.Strings.CC_REVEN, MobjType.Undead),
			new CastInfo(DoomInfo.Strings.CC_MANCU, MobjType.Fatso),
			new CastInfo(DoomInfo.Strings.CC_ARCH, MobjType.Vile),
			new CastInfo(DoomInfo.Strings.CC_SPIDER, MobjType.Spider),
			new CastInfo(DoomInfo.Strings.CC_CYBER, MobjType.Cyborg),
			new CastInfo(DoomInfo.Strings.CC_HERO, MobjType.Player)
		];

		private int castNumber;
		private int castTics;
		private int castFrames;
		private bool castDeath;
		private bool castOnMelee;
		private bool castAttacking;

		private void StartCast()
		{
			Stage = 2;

			castNumber = 0;
			CastState = DoomInfo.States[(int)DoomInfo.MobjInfos[(int)castorder[castNumber].Type].SeeState];
			castTics = CastState.Tics;
			castFrames = 0;
			castDeath = false;
			castOnMelee = false;
			castAttacking = false;

			updateResult = UpdateResult.NeedWipe;

			Options.Music.StartMusic(Bgm.EVIL, true);
		}

		private void UpdateCast()
		{
			if (--castTics > 0)
			{
				// Not time to change state yet.
				return;
			}

			if (CastState.Tics == -1 || CastState.Next == MobjState.Null)
			{
				// Switch from deathstate to next monster.
				castNumber++;
				castDeath = false;
				if (castNumber == castorder.Length)
				{
					castNumber = 0;
				}
				if (DoomInfo.MobjInfos[(int)castorder[castNumber].Type].SeeSound != 0)
				{
					StartSound(DoomInfo.MobjInfos[(int)castorder[castNumber].Type].SeeSound);
				}
				CastState = DoomInfo.States[(int)DoomInfo.MobjInfos[(int)castorder[castNumber].Type].SeeState];
				castFrames = 0;
			}
			else
			{
				// Just advance to next state in animation.
				if (CastState == DoomInfo.States[(int)MobjState.PlayAtk1])
				{
					// Oh, gross hack!
					castAttacking = false;
					CastState = DoomInfo.States[(int)DoomInfo.MobjInfos[(int)castorder[castNumber].Type].SeeState];
					castFrames = 0;
					goto stopAttack;
				}
				var st = CastState.Next;
				CastState = DoomInfo.States[(int)st];
				castFrames++;

				// Sound hacks....
				Sfx sfx;
				switch (st)
				{
					case MobjState.PlayAtk1:
						sfx = Sfx.DSHTGN;
						break;

					case MobjState.PossAtk2:
						sfx = Sfx.PISTOL;
						break;

					case MobjState.SposAtk2:
						sfx = Sfx.SHOTGN;
						break;

					case MobjState.VileAtk2:
						sfx = Sfx.VILATK;
						break;

					case MobjState.SkelFist2:
						sfx = Sfx.SKESWG;
						break;

					case MobjState.SkelFist4:
						sfx = Sfx.SKEPCH;
						break;

					case MobjState.SkelMiss2:
						sfx = Sfx.SKEATK;
						break;

					case MobjState.FattAtk8:
					case MobjState.FattAtk5:
					case MobjState.FattAtk2:
						sfx = Sfx.FIRSHT;
						break;

					case MobjState.CposAtk2:
					case MobjState.CposAtk3:
					case MobjState.CposAtk4:
						sfx = Sfx.SHOTGN;
						break;

					case MobjState.TrooAtk3:
						sfx = Sfx.CLAW;
						break;

					case MobjState.SargAtk2:
						sfx = Sfx.SGTATK;
						break;

					case MobjState.BossAtk2:
					case MobjState.Bos2Atk2:
					case MobjState.HeadAtk2:
						sfx = Sfx.FIRSHT;
						break;

					case MobjState.SkullAtk2:
						sfx = Sfx.SKLATK;
						break;

					case MobjState.SpidAtk2:
					case MobjState.SpidAtk3:
						sfx = Sfx.SHOTGN;
						break;

					case MobjState.BspiAtk2:
						sfx = Sfx.PLASMA;
						break;

					case MobjState.CyberAtk2:
					case MobjState.CyberAtk4:
					case MobjState.CyberAtk6:
						sfx = Sfx.RLAUNC;
						break;

					case MobjState.PainAtk3:
						sfx = Sfx.SKLATK;
						break;

					default:
						sfx = 0;
						break;
				}

				if (sfx != 0)
				{
					StartSound(sfx);
				}
			}

			if (castFrames == 12)
			{
				// Go into attack frame.
				castAttacking = true;
				if (castOnMelee)
				{
					CastState = DoomInfo.States[(int)DoomInfo.MobjInfos[(int)castorder[castNumber].Type].MeleeState];
				}
				else
				{
					CastState = DoomInfo.States[(int)DoomInfo.MobjInfos[(int)castorder[castNumber].Type].MissileState];
				}

				castOnMelee = !castOnMelee;
				if (CastState == DoomInfo.States[(int)MobjState.Null])
				{
					if (castOnMelee)
					{
						CastState = DoomInfo.States[(int)DoomInfo.MobjInfos[(int)castorder[castNumber].Type].MeleeState];
					}
					else
					{
						CastState = DoomInfo.States[(int)DoomInfo.MobjInfos[(int)castorder[castNumber].Type].MissileState];
					}
				}
			}

			if (castAttacking)
			{
				if (castFrames == 24 ||
					CastState == DoomInfo.States[(int)DoomInfo.MobjInfos[(int)castorder[castNumber].Type].SeeState])
				{
					castAttacking = false;
					CastState = DoomInfo.States[(int)DoomInfo.MobjInfos[(int)castorder[castNumber].Type].SeeState];
					castFrames = 0;
				}
			}

			stopAttack:

			castTics = CastState.Tics;
			if (castTics == -1)
			{
				castTics = 15;
			}
		}

		public bool DoEvent(DoomEvent e)
		{
			if (Stage != 2)
			{
				return false;
			}

			if (e.Type == EventType.KeyDown)
			{
				if (castDeath)
				{
					// Already in dying frames.
					return true;
				}

				// Go into death frame.
				castDeath = true;
				CastState = DoomInfo.States[(int)DoomInfo.MobjInfos[(int)castorder[castNumber].Type].DeathState];
				castTics = CastState.Tics;
				castFrames = 0;
				castAttacking = false;
				if (DoomInfo.MobjInfos[(int)castorder[castNumber].Type].DeathSound != 0)
				{
					StartSound(DoomInfo.MobjInfos[(int)castorder[castNumber].Type].DeathSound);
				}

				return true;
			}

			return false;
		}

		private void StartSound(Sfx sfx)
		{
			Options.Sound.StartSound(sfx);
		}



		public GameOptions Options { get; }

		public string Flat { get; }

		public string Text { get; }

		public int Count { get; private set; }

		public int Stage { get; private set; }

		// For cast.
		public string CastName => castorder[castNumber].Name;
		public MobjStateDef CastState { get; private set; }

		// For bunny scroll.
		public int Scrolled { get; private set; }

		public int TheEndIndex { get; private set; }

		public bool ShowTheEnd { get; private set; }


		private class CastInfo
		{
			public readonly string Name;
			public readonly MobjType Type;

			public CastInfo(string name, MobjType type)
			{
				Name = name;
				Type = type;
			}
		}
	}
}
