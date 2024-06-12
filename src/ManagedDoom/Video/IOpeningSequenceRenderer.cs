using ManagedDoom.Doom.Opening;

namespace ManagedDoom.Video;

public interface IOpeningSequenceRenderer
{
    bool Render(IOpeningSequence sequence);
}