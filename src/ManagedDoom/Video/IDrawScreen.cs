using System.Collections.Generic;
using ManagedDoom.Doom.Graphics;

namespace ManagedDoom.Video;

public interface IDrawScreen
{
    byte[] Data { get; }
    int Width { get; }
    int Height { get; }
    void DrawPatch(Patch patch, int x, int y, int scale);
    void DrawPatchFlip(Patch patch, int x, int y, int scale);
    void DrawText(IReadOnlyList<char>? text, int x, int y, int scale);
    void DrawText(string text, int x, int y, int scale);
    void DrawChar(char ch, int x, int y, int scale);
    int MeasureChar(char ch, int scale);
    int MeasureText(IReadOnlyList<char>? text, int scale);
    int MeasureText(string text, int scale);
    void FillRect(int x, int y, int w, int h, int color);
    void DrawLine(float x1, float y1, float x2, float y2, int color);
}