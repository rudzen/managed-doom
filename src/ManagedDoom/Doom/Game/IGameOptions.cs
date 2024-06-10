using ManagedDoom.Audio;
using ManagedDoom.Doom.Common;
using ManagedDoom.Doom.Intermission;
using ManagedDoom.UserInput;
using ManagedDoom.Video;

namespace ManagedDoom.Doom.Game;

public interface IGameOptions
{
    GameVersion GameVersion { get; set; }
    GameMode GameMode { get; set; }
    MissionPack MissionPack { get; set; }
    Player[] Players { get; }
    int ConsolePlayer { get; set; }
    int Episode { get; set; }
    int Map { get; set; }
    GameSkill Skill { get; set; }
    bool DemoPlayback { get; set; }
    bool NetGame { get; set; }
    int Deathmatch { get; set; }
    bool FastMonsters { get; set; }
    bool RespawnMonsters { get; set; }
    bool NoMonsters { get; set; }
    IntermissionInfo IntermissionInfo { get; }
    DoomRandom Random { get; }
    IVideo Video { get; set; }
    ISound Sound { get; set; }
    IMusic Music { get; set; }
    IUserInput UserInput { get; }
}