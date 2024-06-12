using System;
using ManagedDoom.Doom.Game;
using ManagedDoom.Doom.Math;

namespace ManagedDoom.Video;

public interface IRenderer
{
    int Width { get; }
    int Height { get; }
    int WipeBandCount { get; }
    int WipeHeight { get; }
    int MaxWindowSize { get; }
    int WindowSize { get; set; }
    bool DisplayMessage { get; set; }
    int GammaCorrectionLevel { get; set; }
    void RenderGame(DoomGame game, Fixed frameFrac);
    void Render(Doom.Doom doom, Span<byte> destination, Fixed frameFrac);
    void InitializeWipe();
}