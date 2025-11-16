using ManagedDoom.Config;

// Test Arg<int>
var emptyInt = Arg<int>.Empty;
Console.WriteLine($"Arg<int>.Empty.Present = {emptyInt.Present}");
Console.WriteLine($"Arg<int>.Empty.Value = {emptyInt.Value}");

// Test with actual value
var withValue = new Arg<int>(5);
Console.WriteLine($"Arg<int>(5).Present = {withValue.Present}");
Console.WriteLine($"Arg<int>(5).Value = {withValue.Value}");

// Test CommandLineArgs with empty args
var args = new CommandLineArgs(Array.Empty<string>());
Console.WriteLine($"Episode.Present = {args.Episode.Present}");
Console.WriteLine($"Episode.Value = {args.Episode.Value}");

