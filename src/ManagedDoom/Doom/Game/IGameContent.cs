using System;
using ManagedDoom.Doom.Graphics;

namespace ManagedDoom.Doom.Game;

public interface IGameContent : IDisposable
{
    Wad.Wad Wad { get; }
    Palette Palette { get; }
    ColorMap ColorMap { get; }
    ITextureLookup Textures { get; }
    IFlatLookup Flats { get; }
    ISpriteLookup Sprites { get; }
    TextureAnimation Animation { get; }
}