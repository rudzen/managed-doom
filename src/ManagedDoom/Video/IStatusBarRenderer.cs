using ManagedDoom.Doom.Game;
using ManagedDoom.Doom.World;

namespace ManagedDoom.Video;

public interface IStatusBarRenderer
{
    void Render(Player player, BackgroundDrawingType backgroundDrawing);
}