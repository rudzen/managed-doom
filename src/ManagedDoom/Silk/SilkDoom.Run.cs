using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
#if WINDOWS_RELEASE
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
#endif
using Silk.NET.Windowing;

namespace ManagedDoom.Silk;

public sealed partial class SilkDoom
{
    // The main loop provided by Silk.NET with the IWindow.Run method uses a busy loop
    // to control the drawing timing, which is CPU-intensive.
    // Here, I have implemented my own main loop to reduce the CPU load, targeting only
    // Windows, which I can test at hand.
    // In other environments, the IWindow.Run method provided by Silk.NET is used.

#if !WINDOWS_RELEASE
    public Task Run()
    {
        if (args.TimeDemo.Present)
        {
            window.UpdatesPerSecond = 0;
            window.FramesPerSecond = 0;
        }
        else
        {
            config.Values.VideoFpsScale = Math.Clamp(config.Values.VideoFpsScale, 1, 100);
            var targetFps = 35 * config.Values.VideoFpsScale;
            window.UpdatesPerSecond = targetFps;
            window.FramesPerSecond = targetFps;
        }

        window.Run();

        Quit();

        return Task.CompletedTask;
    }
#else
    [LibraryImport("winmm.dll")]
    private static partial uint timeBeginPeriod(uint uPeriod);

    [LibraryImport("winmm.dll")]
    private static partial uint timeEndPeriod(uint uPeriod);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Sleep(int ms)
    {
        timeBeginPeriod(1);
        Thread.Sleep(ms);
        timeEndPeriod(1);
    }

    public Task Run()
    {
        config.Values.VideoFpsScale = Math.Clamp(config.Values.VideoFpsScale, 1, 100);
        var targetFps = 35 * config.Values.VideoFpsScale;

        window.FramesPerSecond = 0;
        window.UpdatesPerSecond = 0;

        if (args.TimeDemo.Present)
            window.Run();
        else
        {
            window.Initialize();

            var gameTime = TimeSpan.Zero;
            var gameTimeStep = TimeSpan.FromSeconds(1.0 / targetFps);
            var startTime = Stopwatch.GetTimestamp();

            while (true)
            {
                window.DoEvents();

                if (!window.IsClosing)
                {
                    window.DoUpdate();
                    gameTime += gameTimeStep;
                }

                if (!window.IsClosing)
                {
                    var elapsed = Stopwatch.GetElapsedTime(startTime);
                    if (elapsed >= gameTime) continue;
                    window.DoRender();
                    var sleepTime = gameTime - elapsed;
                    if (sleepTime > TimeSpan.Zero)
                        Sleep((int)sleepTime.TotalMilliseconds);
                }
                else
                    break;
            }

            window.DoEvents();
            window.Reset();
        }

        Quit();

        return Task.CompletedTask;
    }
#endif
}