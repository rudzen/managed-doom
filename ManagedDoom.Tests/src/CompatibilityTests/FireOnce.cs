namespace ManagedDoom.Tests.CompatibilityTests;

public sealed class FireOnce
{
    [Fact]
    public void Map01()
    {
        using var content = GameContent.CreateDummy(WadPath.Doom2);
        var options = new GameOptions
        {
            Skill = GameSkill.Hard,
            Map = 1
        };
        options.Players[0].InGame = true;

        var ticCommands = Enumerable.Range(0, Player.MaxPlayerCount).Select(i => new TicCmd()).ToArray();
        var game = new DoomGame(content, options);
        game.DeferedInitNew();

        const int tics = 700;
        const int pressFireUntil = 20;
        const byte defaultButton = 0;

        var aggHash = 0;
        for (var i = 0; i < tics; i++)
        {
            ticCommands[0].Buttons = i < pressFireUntil ? TicCmdButtons.Attack : defaultButton;

            game.Update(ticCommands);
            aggHash = DoomDebug.CombineHash(aggHash, DoomDebug.GetMobjHash(game.World));
        }

        Assert.Equal(0xef1aa1d8u, (uint)DoomDebug.GetMobjHash(game.World));
        Assert.Equal(0xe6edcf39u, (uint)aggHash);
    }
}