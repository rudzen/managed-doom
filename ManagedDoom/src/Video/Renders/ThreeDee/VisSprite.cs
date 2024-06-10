using System;
using System.Collections.Generic;
using ManagedDoom.Doom.Graphics;
using ManagedDoom.Doom.Math;
using ManagedDoom.Doom.World;

namespace ManagedDoom.Video.Renders.ThreeDee;

public sealed class VisSprite : IComparer<VisSprite>, IComparable<VisSprite>
{
    public int X1 { get; set; }
    public int X2 { get; set; }

    // For line side calculation.
    public Fixed GlobalX { get; set; }
    public Fixed GlobalY { get; set; }

    // Global bottom / top for silhouette clipping.
    public Fixed GlobalBottomZ { get; set; }
    public Fixed GlobalTopZ { get; set; }

    // Horizontal position of x1.
    public Fixed StartFrac { get; set; }

    public Fixed Scale { get; set; }

    // Negative if flipped.
    public Fixed InvScale { get; set; }

    public Fixed TextureAlt { get; set; }
    public Patch Patch { get; set; }

    // For color translation and shadow draw.
    public byte[] ColorMap { get; set; }

    public MobjFlags MobjFlags { get; set; }

    // to avoid reverse iteration, x - y is used instead of y - x
    // if y - x is used, the sprites should be iterated in reverse order
    public int Compare(VisSprite? x, VisSprite? y)
    {
        return x!.Scale.Data - y!.Scale.Data;
    }

    public int CompareTo(VisSprite? other)
    {
        return Scale.Data - other!.Scale.Data;
    }
}
