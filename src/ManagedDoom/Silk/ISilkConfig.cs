using DrippyAL;
using ManagedDoom.Config;
using ManagedDoom.Doom.Game;

namespace ManagedDoom.Silk;

public interface ISilkConfig
{
    IConfig Config { get; }
    SilkMusic GetMusicInstance(IGameContent content, AudioDevice device);
}