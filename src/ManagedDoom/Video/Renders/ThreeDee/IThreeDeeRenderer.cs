using ManagedDoom.Doom.Game;
using ManagedDoom.Doom.Math;

namespace ManagedDoom.Video.Renders.ThreeDee;

public interface IThreeDeeRenderer
{
    int WindowSize { get; set; }
    void Render(Player player, Fixed frameFrac);
}