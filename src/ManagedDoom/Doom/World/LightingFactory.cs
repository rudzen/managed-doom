using System.Linq;
using System.Runtime.CompilerServices;
using ManagedDoom.Doom.Map;

namespace ManagedDoom.Doom.World;

public static class LightingFactory
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SpawnFireFlicker(World world, Sector sector)
    {
        // Note that we are resetting sector attributes.
        // Nothing special about it during gameplay.
        sector.Special = 0;

        var flicker = new FireFlicker(world);

        world.Thinkers.Add(flicker);

        flicker.Sector = sector;
        flicker.MaxLight = sector.LightLevel;
        flicker.MinLight = FindMinSurroundingLight(sector, sector.LightLevel) + 16;
        flicker.Count = 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SpawnLightFlash(World world, Sector sector)
    {
        // Nothing special about it during gameplay.
        sector.Special = 0;

        var light = new LightFlash(world.Random);

        world.Thinkers.Add(light);

        light.Sector = sector;
        light.MaxLight = sector.LightLevel;

        light.MinLight = FindMinSurroundingLight(sector, sector.LightLevel);
        light.MaxTime = 64;
        light.MinTime = 7;
        light.Count = (world.Random.Next() & light.MaxTime) + 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SpawnStrobeFlash(World world, Sector sector, int time, bool inSync)
    {
        var strobe = new StrobeFlash();

        world.Thinkers.Add(strobe);

        strobe.Sector = sector;
        strobe.DarkTime = time;
        strobe.BrightTime = StrobeFlash.StrobeBright;
        strobe.MaxLight = sector.LightLevel;
        strobe.MinLight = FindMinSurroundingLight(sector, sector.LightLevel);

        if (strobe.MinLight == strobe.MaxLight)
            strobe.MinLight = 0;

        // Nothing special about it during gameplay.
        sector.Special = 0;

        strobe.Count = inSync ? 1 : (world.Random.Next() & 7) + 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SpawnGlowingLight(World world, Sector sector)
    {
        var glowing = new GlowingLight();

        world.Thinkers.Add(glowing);

        glowing.Sector = sector;
        glowing.MinLight = FindMinSurroundingLight(sector, sector.LightLevel);
        glowing.MaxLight = sector.LightLevel;
        glowing.Direction = -1;

        sector.Special = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int FindMinSurroundingLight(Sector sector, int max)
    {
        return sector.Lines
                     .Select(line => SectorAction.GetNextSector(line, sector))
                     .OfType<Sector>()
                     .Select(check => check.LightLevel)
                     .Prepend(max)
                     .Min();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int FindMaxSurroundingLight(Sector sector, int max)
    {
        return sector.Lines
                     .Select(line => SectorAction.GetNextSector(line, sector))
                     .OfType<Sector>()
                     .Select(check => check.LightLevel)
                     .Prepend(max)
                     .Max();
    }
}