using System;
using ManagedDoom.Doom.Math;

namespace ManagedDoom.Video.Renders.ThreeDee;

public sealed class SpriteRender
{
    public static readonly Fixed minZ = Fixed.FromInt(4);

    public SpriteRender()
    {
        VisSprites = new VisSprite[256];
        for (var i = 0; i < VisSprites.Length; i++)
            VisSprites[i] = new VisSprite();
    }

    public int VisSpriteCount { get; set; }
    public VisSprite[] VisSprites { get; }

    public void Clear()
    {
        VisSpriteCount = 0;
    }

    public bool HasTooManySprites()
    {
        return VisSpriteCount == VisSprites.Length;
    }

    public ReadOnlySpan<VisSprite> GetVisibleSprites()
    {
        var span = VisSprites.AsSpan(0, VisSpriteCount);
        span.Sort();
        return span;
    }
}