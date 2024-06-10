namespace ManagedDoom.Video.Renders.ThreeDee;

public sealed class FuzzEffects
{
    private static readonly sbyte[] FuzzTable =
    [
        1, -1, 1, -1, 1, 1, -1,
        1, 1, -1, 1, 1, 1, -1,
        1, 1, 1, -1, -1, -1, -1,
        1, -1, -1, 1, 1, 1, 1, -1,
        1, -1, 1, 1, -1, -1, 1,
        1, -1, -1, -1, -1, 1, 1,
        1, 1, -1, 1, 1, -1, 1
    ];

    private int _fuzzPos;

    public sbyte GetAndIncrementPosition()
    {
        var current = FuzzTable[_fuzzPos];

        ++_fuzzPos;
        if (_fuzzPos == FuzzTable.Length)
            _fuzzPos = 0;

        return current;
    }
}