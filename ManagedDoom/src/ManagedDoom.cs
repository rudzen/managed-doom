using System;
using ManagedDoom;
using ManagedDoom.Silk;

Console.ForegroundColor = ConsoleColor.White;
Console.BackgroundColor = ConsoleColor.DarkGreen;
Console.WriteLine(ApplicationInfo.Title);
Console.ResetColor();

try
{
    string quitMessage;

    using (var doom = new SilkDoom(new CommandLineArgs(args)))
    {
        doom.Run();
        quitMessage = doom.QuitMessage;
    }

    if (!string.IsNullOrWhiteSpace(quitMessage))
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(quitMessage);
        Console.ResetColor();
        Console.Write("Press any key to exit.");
        Console.ReadKey();
    }
}
catch (Exception e)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(e);
    Console.ResetColor();
    Console.Write("Press any key to exit.");
    Console.ReadKey();
}