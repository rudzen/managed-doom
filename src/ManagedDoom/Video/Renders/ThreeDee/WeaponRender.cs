using ManagedDoom.Doom.Math;

namespace ManagedDoom.Video.Renders.ThreeDee;

public sealed class WeaponRender
{
    public VisSprite WeaponSprite { get; } = new();
    public Fixed WeaponScale { get; private set; }
    public Fixed WeaponInvScale { get; private set; }

    public void Reset(WindowSettings windowSettings)
    {
        WeaponScale = new Fixed(Fixed.FracUnit * windowSettings.WindowWidth / 320);
        WeaponInvScale = new Fixed(Fixed.FracUnit * 320 / windowSettings.WindowWidth);
    }
}