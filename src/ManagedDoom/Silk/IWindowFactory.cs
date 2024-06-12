using Silk.NET.Windowing;

namespace ManagedDoom.Silk;

public interface IWindowFactory
{
    IWindow GetWindow();
}