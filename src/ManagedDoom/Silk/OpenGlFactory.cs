using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace ManagedDoom.Silk;

public sealed class OpenGlFactory : IOpenGlFactory
{
    private readonly IWindow window;
    private GL openGl;

    public OpenGlFactory(IWindowFactory windowFactory)
    {
        window = windowFactory.GetWindow();
    }

    public void Initialize()
    {
        openGl = window.CreateOpenGL();
        openGl.ClearColor(0.15F, 0.15F, 0.15F, 1F);
        openGl.Clear(ClearBufferMask.ColorBufferBit);
    }

    public GL GetOpenGl() => openGl;
}