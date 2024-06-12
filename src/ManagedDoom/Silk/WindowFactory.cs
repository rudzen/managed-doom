using ManagedDoom.Config;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace ManagedDoom.Silk;

public sealed class WindowFactory : IWindowFactory
{
    private readonly IWindow window;

    public WindowFactory(IConfig config)
    {
        var windowOptions = WindowOptions.Default;
        windowOptions.Size = new Vector2D<int>(config.Values.VideoScreenWidth, config.Values.VideoScreenHeight);
        windowOptions.Title = ApplicationInfo.Title;
        windowOptions.VSync = config.Values.VideoVsync;
        windowOptions.WindowState = config.Values.VideoFullscreen ? WindowState.Fullscreen : WindowState.Normal;
        window = Window.Create(windowOptions);
    }

    public IWindow GetWindow() => window;
}