namespace ManagedDoom.Tests;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class WadPath
{
    public static readonly string DemoPath = Path.Combine("demos");
    public static readonly string DataPath = Path.Combine("data");

    private const string Doom1Shareware = "DOOM1.WAD";
    private const string Doom1 = "DOOM.WAD";
    private const string Doom2 = "DOOM2.WAD";
    private const string Tnt = "TNT.WAD";
    private const string Plutonia = "PLUTONIA.WAD";

    private const string Requiem = "REQUIEM.WAD";
    private const string TntBlood = "TNTBLOOD.WAD";
    private const string MementoMori = "MM.WAD";

    private readonly Dictionary<WadFile, string> wadFiles = new();

    public WadPath()
    {
        const string wadPathFile = "wad_paths.txt";

        var exists = File.Exists(wadPathFile);

        var directories = exists
            ? File.ReadAllLines(wadPathFile)
            : [];

        var wadPaths = directories
                       .Concat([Environment.CurrentDirectory])
                       .Where(static x => !string.IsNullOrWhiteSpace(x))
                       .Where(x => x.Length > 1)
                       .Where(Directory.Exists)
                       .Where(static x => !x.StartsWith('#'))
                       .Distinct();

        foreach (var wadPath in wadPaths)
        {
            if (TryGetWadPath(wadPath, Doom1Shareware, out var path))
                wadFiles.TryAdd(WadFile.Doom1Shareware, path);

            if (TryGetWadPath(wadPath, Doom1, out path))
                wadFiles.TryAdd(WadFile.Doom1, path);

            if (TryGetWadPath(wadPath, Doom2, out path))
                wadFiles.TryAdd(WadFile.Doom2, path);

            if (TryGetWadPath(wadPath, Tnt, out path))
                wadFiles.TryAdd(WadFile.Tnt, path);

            if (TryGetWadPath(wadPath, Plutonia, out path))
                wadFiles.TryAdd(WadFile.Plutonia, path);

            if (TryGetWadPath(wadPath, Requiem, out path))
                wadFiles.TryAdd(WadFile.Requiem, path);

            if (TryGetWadPath(wadPath, TntBlood, out path))
                wadFiles.TryAdd(WadFile.TntBlood, path);

            if (TryGetWadPath(wadPath, MementoMori, out path))
                wadFiles.TryAdd(WadFile.MementoMori, path);
        }

        return;

        static bool TryGetWadPath(string rootPath, string wadName, out string path)
        {
            path = Path.Combine(rootPath, wadName);
            return File.Exists(path);
        }
    }

    public string GetWadPath(WadFile wadName)
    {
        return wadFiles[wadName];
    }
}