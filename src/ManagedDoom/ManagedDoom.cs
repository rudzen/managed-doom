using System;
using ManagedDoom;
using ManagedDoom.Config;
using ManagedDoom.Doom.Game;
using ManagedDoom.Doom.Graphics;
using ManagedDoom.Host;
using ManagedDoom.Silk;
using ManagedDoom.Video;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Silk.NET.Input.Glfw;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Glfw;

Console.WriteLine(ApplicationInfo.Logo());
Console.ForegroundColor = ConsoleColor.White;
Console.BackgroundColor = ConsoleColor.DarkGreen;
Console.WriteLine(ApplicationInfo.Title);
Console.ResetColor();

// register early
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
              services.AddSingleton<IGameContent, GameContent>();
              services.AddSingleton<IPatchCache, PatchCache>();

              services.AddHostedService<DoomHost>();
          })
          .RunConsoleAsync();