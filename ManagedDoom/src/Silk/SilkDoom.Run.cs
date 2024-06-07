﻿using System;
#if WINDOWS_RELEASE
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
#endif
using Silk.NET.Windowing;

namespace ManagedDoom.Silk;

public sealed partial class SilkDoom : IDisposable
{
    // The main loop provided by Silk.NET with the IWindow.Run method uses a busy loop
    // to control the drawing timing, which is CPU-intensive.
    // Here, I have implemented my own main loop to reduce the CPU load, targeting only
    // Windows, which I can test at hand.
    // In other environments, the IWindow.Run method provided by Silk.NET is used.

#if !WINDOWS_RELEASE
    public void Run()
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
    }
#else
        [DllImport("winmm.dll")]
        private static extern uint timeBeginPeriod(uint uPeriod);

        [DllImport("winmm.dll")]
        private static extern uint timeEndPeriod(uint uPeriod);

        private void Sleep(int ms)
        {
            timeBeginPeriod(1);
            Thread.Sleep(ms);
            timeEndPeriod(1);
        }

        public void Run()
        {
            config.Values.video_fpsscale = Math.Clamp(config.Values.video_fpsscale, 1, 100);
            var targetFps = 35 * config.Values.video_fpsscale;

            window.FramesPerSecond = 0;
            window.UpdatesPerSecond = 0;

            if (args.timedemo.Present)
            {
                window.Run();
            }
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
                        var elap = Stopwatch.GetElapsedTime(startTime);
                        if (elap >= gameTime) continue;
                        window.DoRender();
                        var sleepTime = gameTime - elap;
                        var ms = (int)sleepTime.TotalMilliseconds;
                        if (ms > 0)
                            Sleep(ms);
                    }
                    else
                        break;
                }

                window.DoEvents();
                window.Reset();
            }

            Quit();
        }
#endif
}