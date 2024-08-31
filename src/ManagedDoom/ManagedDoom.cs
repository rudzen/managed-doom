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
using ManagedDoom;
using ManagedDoom.Config;
using ManagedDoom.Doom.Game;
using ManagedDoom.Doom.Graphics;
using ManagedDoom.Host;
using ManagedDoom.Silk;
using ManagedDoom.Video;
using ManagedDoom.Video.Renders.ThreeDee;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Silk.NET.Input.Glfw;
using Silk.NET.Windowing.Glfw;

Console.WriteLine(ApplicationInfo.Logo());
Console.BackgroundColor = ConsoleColor.DarkGreen;
Console.ForegroundColor = ConsoleColor.White;
Console.WriteLine(ApplicationInfo.Title);
Console.ResetColor();

// register early (required)
GlfwWindowing.RegisterPlatform();
GlfwInput.RegisterPlatform();

// configure host
await Host.CreateDefaultBuilder(args)
          .ConfigureLogging(builder => { builder.ClearProviders(); })
          .ConfigureServices((_, services) =>
          {
              // related to Silk.NET etc
              services.AddSingleton<IWindowFactory, WindowFactory>();
              services.AddSingleton<IOpenGlFactory, OpenGlFactory>();

              // command line args
              services.AddSingleton(_ =>
              {
                  ICommandLineArgs commandLineArgs = new CommandLineArgs(args);
                  return commandLineArgs;
              });

              // configuration
              services.AddSingleton(_ =>
              {
                  IConfig config = new Config(ConfigUtilities.GetConfigPath());
                  return config;
              });

              // game fundamentals
              services.AddSingleton<ISilkConfig, SilkConfig>();
              services.AddSingleton<ISilkDoom, SilkDoom>();
              services.AddSingleton<IAudioFactory, AudioFactory>();

                // game content
              services.AddSingleton<IDrawScreen, DrawScreen>();
              services.AddSingleton<IRenderer, Renderer>();
              services.AddSingleton<IMenuRenderer, MenuRenderer>();
              services.AddSingleton<IThreeDeeRenderer, ThreeDeeRenderer>();
              services.AddSingleton<IStatusBarRenderer, StatusBarRenderer>();
              services.AddSingleton<IIntermissionRenderer, IntermissionRenderer>();
              services.AddSingleton<IOpeningSequenceRenderer, OpeningSequenceRenderer>();
              services.AddSingleton<IAutoMapRenderer, AutoMapRenderer>();
              services.AddSingleton<IFinaleRenderer, FinaleRenderer>();
              services.AddSingleton<IGameContent, GameContent>();
              services.AddSingleton<IPatchCache, PatchCache>();

              services.AddHostedService<DoomHost>();
          })
          .RunConsoleAsync();