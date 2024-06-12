using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using DrippyAL;
using ManagedDoom.Audio;
using ManagedDoom.Config;
using ManagedDoom.Doom.Event;
using ManagedDoom.Doom.Game;
using ManagedDoom.Doom.Math;
using ManagedDoom.Video;
using Silk.NET.OpenGL;

namespace ManagedDoom.Silk;

public sealed partial class SilkDoom : ISilkDoom
{
    private readonly Semaphore saveSemaphore = new(1, 1);
    private readonly ICommandLineArgs args;

    private readonly IConfig config;
    private readonly ISilkConfig silkConfig;

    private readonly IGameContent gameContent;
    private readonly IRenderer renderer;

    private readonly IWindow window;
    private GL openGl;

    private SilkVideo? video;

    private ISound? sound;
    private IMusic? music;

    private SilkUserInput? userInput;

    private Doom.Doom? doom;

    private int fpsScale;
    private int frameCount;

    public SilkDoom(
        ICommandLineArgs args,
        IGameContent gameContent,
        ISilkConfig silkConfig,
        IWindowFactory windowFactory,
        IRenderer renderer,
        IAudioFactory audioFactory
    )
    {
        try
        {
            var start = Stopwatch.GetTimestamp();
            this.args = args;
            this.window = windowFactory.GetWindow();

            this.silkConfig = silkConfig;
            this.config = silkConfig.Config;
            this.gameContent = gameContent;
            this.renderer = renderer;

            this.sound = audioFactory.GetSound();
            this.music = audioFactory.GetMusic();

            window.Load += OnLoad;
            window.Update += OnUpdate;
            window.Render += OnRender;
            window.Resize += OnResize;
            window.Closing += OnClose;

            Console.WriteLine($"Startup time: [{Stopwatch.GetElapsedTime(start)}]");
        }
        catch (Exception e)
        {
            ExceptionDispatchInfo.Throw(e);
        }
    }

    public string? QuitMessage => doom!.QuitMessage;

    public Exception? Exception { get; private set; }

    private void InitializeOpenGl()
    {
        openGl = window.CreateOpenGL();
        openGl.ClearColor(0.15F, 0.15F, 0.15F, 1F);
        openGl.Clear(ClearBufferMask.ColorBufferBit);
    }

    private void Quit()
    {
        if (Exception is not null)
            ExceptionDispatchInfo.Throw(Exception);
    }

    private void OnLoad()
    {
        InitializeOpenGl();
        window.SwapBuffers();

        video = new SilkVideo(config.Values, renderer, window, openGl);
        userInput = new SilkUserInput(config.Values, window, this, args);
        doom = new Doom.Doom(args, config, gameContent, video, sound, music, userInput);

        fpsScale = args.TimeDemo.Present ? 1 : config.Values.VideoFpsScale;
        frameCount = -1;
    }

    private void OnUpdate(double obj)
    {
        try
        {
            frameCount++;

            if (frameCount % fpsScale == 0 && doom!.Update() == UpdateResult.Completed)
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
            video!.Render(doom!, frameFrac);
        }
        catch (Exception e)
        {
            Exception = e;
        }
    }

    private void OnResize(Vector2D<int> obj)
    {
        video?.Resize(obj.X, obj.Y);
    }

    private void OnClose()
    {
        if (userInput is not null)
        {
            userInput.Dispose();
            userInput = null;
        }

        if (video is not null)
        {
            video.Dispose();
            video = null;
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
        doom!.PostEvent(new DoomEvent(EventType.KeyDown, SilkUserInput.SilkToDoom(key)));
    }

    public void KeyUp(Key key)
    {
        doom!.PostEvent(new DoomEvent(EventType.KeyUp, SilkUserInput.SilkToDoom(key)));
    }
}