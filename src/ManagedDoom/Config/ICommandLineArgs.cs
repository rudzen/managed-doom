namespace ManagedDoom.Config;

public interface ICommandLineArgs
{
    Arg<string> Iwad { get; }
    Arg<string[]> File { get; }
    Arg<string[]> Deh { get; }
    Arg<Warp> Warp { get; }
    Arg<int> Episode { get; }
    Arg<int> Skill { get; }
    Arg DeathMatch { get; }
    Arg AltDeath { get; }
    Arg Fast { get; }
    Arg Respawn { get; }
    Arg NoMonsters { get; }
    Arg SoloNet { get; }
    Arg<string> PlayDemo { get; }
    Arg<string> TimeDemo { get; }
    Arg<int> LoadGame { get; }
    Arg NoMouse { get; }
    Arg NoSound { get; }
    Arg NoSfx { get; }
    Arg NoMusic { get; }
    Arg NoDeh { get; }
}