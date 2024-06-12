namespace ManagedDoom.Doom.Graphics;

public interface IPatchCache
{
    Patch this[string name] { get; }
    int GetWidth(string name);
    int GetHeight(string name);
}