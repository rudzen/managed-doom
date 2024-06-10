namespace ManagedDoom.Doom.Graphics;

public interface ILookup<out T>
{
    int GetNumber(string name);
    T this[string name] { get; }
}