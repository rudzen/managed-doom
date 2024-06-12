using ManagedDoom.Doom.Game;

namespace ManagedDoom.Doom.Opening;

public interface IOpeningSequence
{
    void Reset();
    OpeningSequenceState State { get; }
    DoomGame? DemoGame { get; }
    UpdateResult Update();
}