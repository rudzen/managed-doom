using Silk.NET.OpenGL;

namespace ManagedDoom.Silk;

public interface IOpenGlFactory
{
    void Initialize();
    GL GetOpenGl();
}