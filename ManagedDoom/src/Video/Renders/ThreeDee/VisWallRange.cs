using ManagedDoom.Doom.Map;
using ManagedDoom.Doom.Math;

namespace ManagedDoom.Video.Renders.ThreeDee;

public sealed class VisWallRange
{
    public Seg Seg;

    public int X1;
    public int X2;

    public Fixed Scale1;
    public Fixed Scale2;
    public Fixed ScaleStep;

    public Silhouette Silhouette;
    public Fixed UpperSilHeight;
    public Fixed LowerSilHeight;

    public int UpperClip;
    public int LowerClip;
    public int MaskedTextureColumn;

    public Fixed FrontSectorFloorHeight;
    public Fixed FrontSectorCeilingHeight;
    public Fixed BackSectorFloorHeight;
    public Fixed BackSectorCeilingHeight;
}