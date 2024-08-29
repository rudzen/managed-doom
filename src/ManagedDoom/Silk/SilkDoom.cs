//
// Copyright (C) 1993-1996 Id Software, Inc.
// Copyright (C) 2019-2020 Nobuaki Tanaka
// Copyright (C)      2024 Rudy Alex Kohn
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//

using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using ManagedDoom.Config;
using ManagedDoom.Doom.Event;
using ManagedDoom.Doom.Game;
using ManagedDoom.Doom.Math;
using ManagedDoom.Video;
using Silk.NET.OpenGL;

namespace ManagedDoom.Silk;

public sealed partial class SilkDoom : ISilkDoom
{
    private readonly ICommandLineArgs args;

    private readonly ISilkConfig silkConfig;

    private readonly IGameContent gameContent;
    private readonly IRenderer renderer;

    private readonly IWindow window;
    private GL? openGl;

    private SilkVideo? video;

    private readonly IAudioFactory audioFactory;

    private SilkUserInput? userInput;

    private Doom.Doom? doom;

    private int fpsScale;
    private int frameCount;

    private long frameTimes;
    private long frameTimesRender;
    private long fpsStamp;

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
            this.gameContent = gameContent;
            this.renderer = renderer;
            this.audioFactory = audioFactory;

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

        if (openGl is null)
            Quit();

        window.SwapBuffers();

        video = new SilkVideo(silkConfig.Config.Values, renderer, window, openGl!);
        userInput = new SilkUserInput(silkConfig.Config.Values, window, this, args);
        doom = new Doom.Doom(args, silkConfig, gameContent, video, audioFactory, userInput);

        fpsScale = args.TimeDemo.Present ? 1 : silkConfig.Config.Values.VideoFpsScale;
        frameCount = -1;
        fpsStamp = Stopwatch.GetTimestamp();
    }

    private void OnUpdate(double obj)
    {
        try
        {
            frameCount++;

            if (frameCount % fpsScale == 0 && doom!.Update() == UpdateResult.Completed)
                window.Close();

            frameTimes++;

            if (Stopwatch.GetElapsedTime(fpsStamp) >= TimeSpan.FromSeconds(1))
            {
                fpsStamp = Stopwatch.GetTimestamp();
                Console.WriteLine("fps: " + frameTimes + " frames per second.");
                frameTimesRender = frameTimes;
                frameTimes = default;
            }
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
            video!.Render(doom!, frameFrac, in frameTimesRender);
            // if (frameCount == 0)
            // {
            //     frameTimesRender = 0;
            // }
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

        if (!args.TimeDemo.Present)
            silkConfig.Config.Save(ConfigUtilities.GetConfigPath());
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