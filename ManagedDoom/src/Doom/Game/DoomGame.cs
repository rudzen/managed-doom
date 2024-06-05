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
using System.IO;
using ManagedDoom.Config;
using ManagedDoom.Doom.Event;
using ManagedDoom.Doom.Info;
using ManagedDoom.Doom.Intermission;

namespace ManagedDoom.Doom.Game
{
	public sealed class DoomGame
	{
		private readonly GameContent content;

		private GameAction gameAction;

		private int loadGameSlotNumber;
		private int saveGameSlotNumber;
		private string saveGameDescription;

		public DoomGame(GameContent content, GameOptions options)
		{
			this.content = content;
			this.Options = options;

			gameAction = GameAction.Nothing;

			GameTic = 0;
		}


		////////////////////////////////////////////////////////////
		// Public methods to control the game state
		////////////////////////////////////////////////////////////

		/// <summary>
		/// Start a new game.
		/// Can be called by the startup code or the menu task.
		/// </summary>
		public void DeferedInitNew()
		{
			gameAction = GameAction.NewGame;
		}

		/// <summary>
		/// Start a new game.
		/// Can be called by the startup code or the menu task.
		/// </summary>
		public void DeferedInitNew(GameSkill skill, int episode, int map)
		{
			Options.Skill = skill;
			Options.Episode = episode;
			Options.Map = map;
			gameAction = GameAction.NewGame;
		}

		/// <summary>
		/// Load the saved game at the given slot number.
		/// Can be called by the startup code or the menu task.
		/// </summary>
		public void LoadGame(int slotNumber)
		{
			loadGameSlotNumber = slotNumber;
			gameAction = GameAction.LoadGame;
		}

		/// <summary>
		/// Save the game at the given slot number.
		/// Can be called by the startup code or the menu task.
		/// </summary>
		public void SaveGame(int slotNumber, string description)
		{
			saveGameSlotNumber = slotNumber;
			saveGameDescription = description;
			gameAction = GameAction.SaveGame;
		}

		/// <summary>
		/// Advance the game one frame.
		/// </summary>
		public UpdateResult Update(ReadOnlySpan<TicCmd> cmds)
		{
			// Do player reborns if needed.
			var players = Options.Players.AsSpan();
			for (var i = 0; i < Player.MaxPlayerCount; i++)
			{
				if (players[i].InGame && players[i].PlayerState == PlayerState.Reborn)
					DoReborn(i);
			}

			// Do things to change the game state.
			while (gameAction != GameAction.Nothing)
			{
				switch (gameAction)
				{
					case GameAction.LoadLevel:
						DoLoadLevel();
						break;
					case GameAction.NewGame:
						DoNewGame();
						break;
					case GameAction.LoadGame:
						DoLoadGame();
						break;
					case GameAction.SaveGame:
						DoSaveGame();
						break;
					case GameAction.Completed:
						DoCompleted();
						break;
					case GameAction.Victory:
						DoFinale();
						break;
					case GameAction.WorldDone:
						DoWorldDone();
						break;
					case GameAction.Nothing:
						break;
				}
			}

			for (var i = 0; i < Player.MaxPlayerCount; i++)
			{
				if (players[i].InGame)
				{
					var cmd = players[i].Cmd;
					cmd.CopyFrom(cmds[i]);

					/*
					if (demorecording)
					{
						G_WriteDemoTiccmd(cmd);
					}
					*/

					// Check for turbo cheats.
					if (cmd.ForwardMove > GameConst.TurboThreshold &&
						(World.LevelTime & 31) == 0 &&
						((World.LevelTime >> 5) & 3) == i)
					{
						var player = players[Options.ConsolePlayer];
						player.SendMessage(players[i].Name + " is turbo!");
					}
				}
			}

			// Check for special buttons.
			for (var i = 0; i < Player.MaxPlayerCount; i++)
			{
				if (players[i].InGame)
				{
					if ((players[i].Cmd.Buttons & TicCmdButtons.Special) != 0)
					{
						if ((players[i].Cmd.Buttons & TicCmdButtons.SpecialMask) == TicCmdButtons.Pause)
						{
							Paused ^= true;
							if (Paused)
							{
								Options.Sound.Pause();
							}
							else
							{
								Options.Sound.Resume();
							}
						}
					}
				}
			}

			// Do main actions.
			var result = UpdateResult.None;
			switch (State)
			{
				case GameState.Level:
					if (!Paused || World.FirstTicIsNotYetDone)
					{
						result = World.Update();
						if (result == UpdateResult.Completed)
						{
							gameAction = GameAction.Completed;
						}
					}
					break;

				case GameState.Intermission:
					result = Intermission.Update();
					if (result == UpdateResult.Completed)
					{
						gameAction = GameAction.WorldDone;

						if (World.SecretExit)
						{
							players[Options.ConsolePlayer].DidSecret = true;
						}

						if (Options.GameMode == GameMode.Commercial)
						{
							switch (Options.Map)
							{
								case 6:
								case 11:
								case 20:
								case 30:
									DoFinale();
									result = UpdateResult.NeedWipe;
									break;

								case 15:
								case 31:
									if (World.SecretExit)
									{
										DoFinale();
										result = UpdateResult.NeedWipe;
									}
									break;
							}
						}
					}
					break;

				case GameState.Finale:
					result = Finale.Update();
					if (result == UpdateResult.Completed)
					{
						gameAction = GameAction.WorldDone;
					}
					break;
			}

			GameTic++;

			if (result == UpdateResult.NeedWipe)
			{
				return UpdateResult.NeedWipe;
			}

			return UpdateResult.None;
		}


		////////////////////////////////////////////////////////////
		// Actual game actions
		////////////////////////////////////////////////////////////

		// It seems that these methods should not be called directly
		// from outside for some reason.
		// So if you want to start a new game or do load / save, use
		// the following public methods.
		//
		//     - DeferedInitNew
		//     - LoadGame
		//     - SaveGame

		private void DoLoadLevel()
		{
			gameAction = GameAction.Nothing;

			State = GameState.Level;

			var players = Options.Players;
			for (var i = 0; i < Player.MaxPlayerCount; i++)
			{
				if (players[i].InGame && players[i].PlayerState == PlayerState.Dead)
				{
					players[i].PlayerState = PlayerState.Reborn;
				}
				Array.Clear(players[i].Frags, 0, players[i].Frags.Length);
			}

			Intermission = null;

			Options.Sound.Reset();

			World = new World.World(content, Options, this);

			Options.UserInput.Reset();
		}

		private void DoNewGame()
		{
			gameAction = GameAction.Nothing;

			InitNew(Options.Skill, Options.Episode, Options.Map);
		}

		private void DoLoadGame()
		{
			gameAction = GameAction.Nothing;

			var directory = ConfigUtilities.GetExeDirectory;
			var path = Path.Combine(directory, "doomsav" + loadGameSlotNumber + ".dsg");
			SaveAndLoad.Load(this, path);
		}

		private void DoSaveGame()
		{
			gameAction = GameAction.Nothing;

			var directory = ConfigUtilities.GetExeDirectory;
			var path = Path.Combine(directory, "doomsav" + saveGameSlotNumber + ".dsg");
			SaveAndLoad.Save(this, saveGameDescription, path);
			World.ConsolePlayer.SendMessage(DoomInfo.Strings.GGSAVED);
		}

		private void DoCompleted()
		{
			gameAction = GameAction.Nothing;

			for (var i = 0; i < Player.MaxPlayerCount; i++)
			{
				if (Options.Players[i].InGame)
				{
					// Take away cards and stuff.
					Options.Players[i].FinishLevel();
				}
			}

			if (Options.GameMode != GameMode.Commercial)
			{
				switch (Options.Map)
				{
					case 8:
						gameAction = GameAction.Victory;
						return;
					case 9:
						for (var i = 0; i < Player.MaxPlayerCount; i++)
						{
							Options.Players[i].DidSecret = true;
						}
						break;
				}
			}

			if ((Options.Map == 8) && (Options.GameMode != GameMode.Commercial))
			{
				// Victory.
				gameAction = GameAction.Victory;
				return;
			}

			if ((Options.Map == 9) && (Options.GameMode != GameMode.Commercial))
			{
				// Exit secret level.
				for (var i = 0; i < Player.MaxPlayerCount; i++)
				{
					Options.Players[i].DidSecret = true;
				}
			}

			var imInfo = Options.IntermissionInfo;

			imInfo.DidSecret = Options.Players[Options.ConsolePlayer].DidSecret;
			imInfo.Episode = Options.Episode - 1;
			imInfo.LastLevel = Options.Map - 1;

			// IntermissionInfo.Next is 0 biased, unlike GameOptions.Map.
			if (Options.GameMode == GameMode.Commercial)
			{
				if (World.SecretExit)
				{
					switch (Options.Map)
					{
						case 15:
							imInfo.NextLevel = 30;
							break;
						case 31:
							imInfo.NextLevel = 31;
							break;
					}
				}
				else
				{
					switch (Options.Map)
					{
						case 31:
						case 32:
							imInfo.NextLevel = 15;
							break;
						default:
							imInfo.NextLevel = Options.Map;
							break;
					}
				}
			}
			else
			{
				if (World.SecretExit)
				{
					// Go to secret level.
					imInfo.NextLevel = 8;
				}
				else if (Options.Map == 9)
				{
					// Returning from secret level.
					switch (Options.Episode)
					{
						case 1:
							imInfo.NextLevel = 3;
							break;
						case 2:
							imInfo.NextLevel = 5;
							break;
						case 3:
							imInfo.NextLevel = 6;
							break;
						case 4:
							imInfo.NextLevel = 2;
							break;
					}
				}
				else
				{
					// Go to next level.
					imInfo.NextLevel = Options.Map;
				}
			}

			imInfo.MaxKillCount = World.TotalKills;
			imInfo.MaxItemCount = World.TotalItems;
			imInfo.MaxSecretCount = World.TotalSecrets;
			imInfo.TotalFrags = 0;
			if (Options.GameMode == GameMode.Commercial)
			{
				imInfo.ParTime = 35 * DoomInfo.ParTimes.Doom2[Options.Map - 1];
			}
			else
			{
				imInfo.ParTime = 35 * DoomInfo.ParTimes.Doom1[Options.Episode - 1][Options.Map - 1];
			}

			var players = Options.Players;
			for (var i = 0; i < Player.MaxPlayerCount; i++)
			{
				imInfo.Players[i].InGame = players[i].InGame;
				imInfo.Players[i].KillCount = players[i].KillCount;
				imInfo.Players[i].ItemCount = players[i].ItemCount;
				imInfo.Players[i].SecretCount = players[i].SecretCount;
				imInfo.Players[i].Time = World.LevelTime;
				Array.Copy(players[i].Frags, imInfo.Players[i].Frags, Player.MaxPlayerCount);
			}

			State = GameState.Intermission;
			Intermission = new Intermission.Intermission(Options, imInfo);
		}

		private void DoWorldDone()
		{
			gameAction = GameAction.Nothing;

			State = GameState.Level;
			Options.Map = Options.IntermissionInfo.NextLevel + 1;
			DoLoadLevel();
		}

		private void DoFinale()
		{
			gameAction = GameAction.Nothing;

			State = GameState.Finale;
			Finale = new Finale(Options);
		}


		////////////////////////////////////////////////////////////
		// Miscellaneous things
		////////////////////////////////////////////////////////////

		public void InitNew(GameSkill skill, int episode, int map)
		{
			Options.Skill = (GameSkill)System.Math.Clamp((int)skill, (int)GameSkill.Baby, (int)GameSkill.Nightmare);

			if (Options.GameMode == GameMode.Retail)
			{
				Options.Episode = System.Math.Clamp(episode, 1, 4);
			}
			else if (Options.GameMode == GameMode.Shareware)
			{
				Options.Episode = 1;
			}
			else
			{
				Options.Episode = System.Math.Clamp(episode, 1, 4);
			}

			if (Options.GameMode == GameMode.Commercial)
			{
				Options.Map = System.Math.Clamp(map, 1, 32);
			}
			else
			{
				Options.Map = System.Math.Clamp(map, 1, 9);
			}

			Options.Random.Clear();

			// Force players to be initialized upon first level load.
			for (var i = 0; i < Player.MaxPlayerCount; i++)
			{
				Options.Players[i].PlayerState = PlayerState.Reborn;
			}

			DoLoadLevel();
		}

		public bool DoEvent(in DoomEvent e)
		{
			return State switch
			{
				GameState.Level  => World.DoEvent(in e),
				GameState.Finale => Finale.DoEvent(in e),
				_                => false
			};
		}

		private void DoReborn(int playerNumber)
		{
			if (!Options.NetGame)
			{
				// Reload the level from scratch.
				gameAction = GameAction.LoadLevel;
			}
			else
			{
				// Respawn at the start.

				// First dissasociate the corpse.
				Options.Players[playerNumber].Mobj.Player = null;

				var ta = World.ThingAllocation;

				// Spawn at random spot if in death match.
				if (Options.Deathmatch != 0)
				{
					ta.DeathMatchSpawnPlayer(playerNumber);
					return;
				}

				if (ta.CheckSpot(playerNumber, ta.PlayerStarts[playerNumber]))
				{
					ta.SpawnPlayer(ta.PlayerStarts[playerNumber]);
					return;
				}

				// Try to spawn at one of the other players spots.
				for (var i = 0; i < Player.MaxPlayerCount; i++)
				{
					if (ta.CheckSpot(playerNumber, ta.PlayerStarts[i]))
					{
						// Fake as other player.
						ta.PlayerStarts[i].Type = playerNumber + 1;

						World.ThingAllocation.SpawnPlayer(ta.PlayerStarts[i]);

						// Restore.
						ta.PlayerStarts[i].Type = i + 1;

						return;
					}
				}

				// He's going to be inside something.
				// Too bad.
				World.ThingAllocation.SpawnPlayer(ta.PlayerStarts[playerNumber]);
			}
		}


		public GameOptions Options { get; }

		public GameState State { get; private set; }

		public int GameTic { get; private set; }

		public World.World World { get; private set; }

		public Intermission.Intermission Intermission { get; private set; }

		public Finale Finale { get; private set; }

		public bool Paused { get; private set; }


		private enum GameAction
		{
			Nothing,
			LoadLevel,
			NewGame,
			LoadGame,
			SaveGame,
			Completed,
			Victory,
			WorldDone
		}
	}
}
