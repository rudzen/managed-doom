using System;
using ManagedDoom;
using ManagedDoom.Config;
using ManagedDoom.Doom.Game;
using ManagedDoom.Host;
using ManagedDoom.Silk;
using ManagedDoom.UserInput;
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
              services.AddSingleton(provider =>
              {
                  var silkConfig = provider.GetRequiredService<ISilkConfig>();
                  var configValues = silkConfig.Config.Values;
                  var windowOptions = WindowOptions.Default;
                  windowOptions.Size = new Vector2D<int>(configValues.VideoScreenWidth, configValues.VideoScreenHeight);
                  windowOptions.Title = ApplicationInfo.Title;
                  windowOptions.VSync = configValues.VideoVsync;
                  windowOptions.WindowState = configValues.VideoFullscreen ? WindowState.Fullscreen : WindowState.Normal;
                  return Window.Create(windowOptions);
              });

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

              services.AddSingleton<ISilkConfig, SilkConfig>();

              services.AddSingleton<IGameContent, GameContent>();

              services.AddSingleton<ISilkDoom, SilkDoom>();
              services.AddHostedService<DoomHost>();
          })
          .RunConsoleAsync();