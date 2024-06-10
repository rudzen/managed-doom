using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading;
using Silk.NET.Input;
using Silk.NET.Input.Glfw;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Glfw;
using DrippyAL;
using ManagedDoom.Config;
using ManagedDoom.Doom.Event;
using ManagedDoom.Doom.Game;
using ManagedDoom.Doom.Math;

namespace ManagedDoom.Silk;

public sealed partial class SilkDoom
{
    private readonly Semaphore saveSemaphore = new(1, 1);
    private readonly CommandLineArgs args;

    private readonly Config.Config config;
    private readonly GameContent content;

    private IWindow window;

    private GL gl;
    private SilkVideo video;

    private AudioDevice audioDevice;
    private SilkSound sound;
    private SilkMusic music;

    private SilkUserInput userInput;

    private Doom.Doom doom;

    private int fpsScale;
    private int frameCount;

    public SilkDoom(CommandLineArgs args)
    {
        try
        {
            var start = Stopwatch.GetTimestamp();
            this.args = args;

            GlfwWindowing.RegisterPlatform();
            GlfwInput.RegisterPlatform();

            config = SilkConfigUtilities.GetConfig();
            content = new GameContent(args);

            config.Values.VideoScreenWidth = Math.Clamp(config.Values.VideoScreenWidth, 320, 3200);
            config.Values.VideoScreenHeight = Math.Clamp(config.Values.VideoScreenHeight, 200, 2000);

            var windowOptions = WindowOptions.Default;
            windowOptions.Size = new Vector2D<int>(config.Values.VideoScreenWidth, config.Values.VideoScreenHeight);
            windowOptions.Title = ApplicationInfo.Title;
            windowOptions.VSync = config.Values.VideoVsync;
            windowOptions.WindowState = config.Values.VideoFullscreen ? WindowState.Fullscreen : WindowState.Normal;

            window = Window.Create(windowOptions);
            window.Load += OnLoad;
            window.Update += OnUpdate;
            window.Render += OnRender;
            window.Resize += OnResize;
            window.Closing += OnClose;

            Console.WriteLine($"Startup time: [{Stopwatch.GetElapsedTime(start)}]");
        }
        catch (Exception e)
        {
            Dispose();
            ExceptionDispatchInfo.Throw(e);
        }
    }

    private void Quit()
    {
        if (Exception is not null)
            ExceptionDispatchInfo.Throw(Exception);
    }

    private void OnLoad()
    {
        gl = window.CreateOpenGL();
        gl.ClearColor(0.15F, 0.15F, 0.15F, 1F);
        gl.Clear(ClearBufferMask.ColorBufferBit);
        window.SwapBuffers();

        video = new SilkVideo(config.Values, content, window, gl);

        if (!args.NoSound.Present && !(args.NoSfx.Present && args.NoMusic.Present))
        {
            audioDevice = new AudioDevice();
            if (!args.NoSfx.Present)
                sound = new SilkSound(config.Values, content, audioDevice);
            if (!args.NoMusic.Present)
                music = SilkConfigUtilities.GetMusicInstance(config.Values, content, audioDevice);
        }

        userInput = new SilkUserInput(config.Values, window, this, !args.NoMouse.Present);

        doom = new Doom.Doom(args, config.Values, content, video, sound, music, userInput);

        fpsScale = args.TimeDemo.Present ? 1 : config.Values.VideoFpsScale;
        frameCount = -1;
    }

    private void OnUpdate(double obj)
    {
        try
        {
            frameCount++;

            if (frameCount % fpsScale == 0 && doom.Update() == UpdateResult.Completed)
                window.Close();
        }
        catch (Exception e)
        {
            Exception = e;
        }

        if (Exception is not null)
            window.Close();
    }

    private void OnRender(double obj)
    {
        try
        {
            var frameFrac = Fixed.FromInt(frameCount % fpsScale + 1) / fpsScale;
            video.Render(doom, frameFrac);
        }
        catch (Exception e)
        {
            Exception = e;
        }
    }

    private void OnResize(Vector2D<int> obj)
    {
        video.Resize(obj.X, obj.Y);
    }

    private void OnClose()
    {
        if (userInput is not null)
        {
            userInput.Dispose();
            userInput = null;
        }

        if (music is not null)
        {
            music.Dispose();
            music = null;
        }

        if (sound is not null)
        {
            sound.Dispose();
            sound = null;
        }

        if (audioDevice is not null)
        {
            audioDevice.Dispose();
            audioDevice = null;
        }

        if (video is not null)
        {
            video.Dispose();
            video = null;
        }

        if (gl is not null)
        {
            gl.Dispose();
            gl = null;
        }

        saveSemaphore.WaitOne();
        try
        {
            config.Save(ConfigUtilities.GetConfigPath());
        }
        finally
        {
            saveSemaphore.Release();
        }
    }

    public void KeyDown(Key key)
    {
        doom.PostEvent(new DoomEvent(EventType.KeyDown, SilkUserInput.SilkToDoom(key)));
    }

    public void KeyUp(Key key)
    {
        doom.PostEvent(new DoomEvent(EventType.KeyUp, SilkUserInput.SilkToDoom(key)));
    }

    public void Dispose()
    {
        if (window != null)
        {
            window.Close();
            window.Dispose();
            window = null;
        }
    }

    public string QuitMessage => doom.QuitMessage;
    public Exception Exception { get; private set; }
}