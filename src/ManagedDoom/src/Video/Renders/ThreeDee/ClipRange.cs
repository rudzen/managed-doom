namespace ManagedDoom.Video.Renders.ThreeDee;

public sealed class ClipRange
{
    public int First { get; set; }
    public int Last { get; set; }
    
    public void CopyFrom(ClipRange from)
    {
        First = from.First;
        Last = from.Last;
    }
}