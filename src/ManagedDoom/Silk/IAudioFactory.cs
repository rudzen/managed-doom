using ManagedDoom.Audio;

namespace ManagedDoom.Silk;

public interface IAudioFactory
{
    ISound GetSound();
    IMusic GetMusic();
}