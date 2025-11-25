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

using ManagedDoom.Config;

namespace ManagedDoom.Tests.UnitTests;

public sealed class CommandLineArgsTest
{
    #region Iwad Tests

    [Fact]
    public void Iwad_WithValidArgument_ShouldParseCorrectly()
    {
        // Arrange
        var args = new[] { "-iwad", "doom2.wad" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.True(commandLineArgs.Iwad.Present);
        Assert.Equal("doom2.wad", commandLineArgs.Iwad.Value);
    }

    [Fact]
    public void Iwad_WithoutArgument_ShouldNotBePresent()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.False(commandLineArgs.Iwad.Present);
    }

    [Fact]
    public void Iwad_WithMultipleValues_ShouldTakeFirstOnly()
    {
        // Arrange
        var args = new[] { "-iwad", "doom2.wad", "doom.wad" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.True(commandLineArgs.Iwad.Present);
        Assert.Equal("doom2.wad", commandLineArgs.Iwad.Value);
    }

    [Fact]
    public void Iwad_WithNoValue_ShouldNotBePresent()
    {
        // Arrange
        var args = new[] { "-iwad" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.False(commandLineArgs.Iwad.Present);
    }

    #endregion

    #region File Tests

    [Fact]
    public void File_WithSingleFile_ShouldParseCorrectly()
    {
        // Arrange
        var args = new[] { "-file", "test.wad" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.True(commandLineArgs.File.Present);
        Assert.Single(commandLineArgs.File.Value!);
        Assert.Equal("test.wad", commandLineArgs.File.Value![0]);
    }

    [Fact]
    public void File_WithMultipleFiles_ShouldParseAll()
    {
        // Arrange
        var args = new[] { "-file", "test1.wad", "test2.wad", "test3.wad" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.True(commandLineArgs.File.Present);
        Assert.Equal(3, commandLineArgs.File.Value!.Length);
        Assert.Equal("test1.wad", commandLineArgs.File.Value![0]);
        Assert.Equal("test2.wad", commandLineArgs.File.Value![1]);
        Assert.Equal("test3.wad", commandLineArgs.File.Value![2]);
    }

    [Fact]
    public void File_StopsAtNextFlag_ShouldParseCorrectly()
    {
        // Arrange
        var args = new[] { "-file", "test1.wad", "test2.wad", "-skill", "4" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.True(commandLineArgs.File.Present);
        Assert.Equal(2, commandLineArgs.File.Value!.Length);
        Assert.Equal("test1.wad", commandLineArgs.File.Value![0]);
        Assert.Equal("test2.wad", commandLineArgs.File.Value![1]);
    }

    [Fact]
    public void File_WithNoValue_ShouldNotBePresent()
    {
        // Arrange
        var args = new[] { "-file" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.False(commandLineArgs.File.Present);
    }

    [Fact]
    public void File_WithoutArgument_ShouldNotBePresent()
    {
        // Arrange
        string[] args = [];

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.False(commandLineArgs.File.Present);
    }

    #endregion

    #region Deh Tests

    [Fact]
    public void Deh_WithSingleFile_ShouldParseCorrectly()
    {
        // Arrange
        var args = new[] { "-deh", "test.deh" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.True(commandLineArgs.Deh.Present);
        Assert.Single(commandLineArgs.Deh.Value!);
        Assert.Equal("test.deh", commandLineArgs.Deh.Value![0]);
    }

    [Fact]
    public void Deh_WithMultipleFiles_ShouldParseAll()
    {
        // Arrange
        var args = new[] { "-deh", "test1.deh", "test2.deh" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.True(commandLineArgs.Deh.Present);
        Assert.Equal(2, commandLineArgs.Deh.Value!.Length);
        Assert.Equal("test1.deh", commandLineArgs.Deh.Value![0]);
        Assert.Equal("test2.deh", commandLineArgs.Deh.Value![1]);
    }

    [Fact]
    public void Deh_WithoutArgument_ShouldNotBePresent()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.False(commandLineArgs.Deh.Present);
    }

    #endregion

    #region Warp Tests

    [Fact]
    public void Warp_WithSingleValue_ShouldParseAsMapWithEpisode1()
    {
        // Arrange
        var args = new[] { "-warp", "15" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.True(commandLineArgs.Warp.Present);
        Assert.Equal(1, commandLineArgs.Warp.Value!.Episode);
        Assert.Equal(15, commandLineArgs.Warp.Value!.Map);
    }

    [Fact]
    public void Warp_WithTwoValues_ShouldParseEpisodeAndMap()
    {
        // Arrange
        var args = new[] { "-warp", "2", "8" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.True(commandLineArgs.Warp.Present);
        Assert.Equal(2, commandLineArgs.Warp.Value!.Episode);
        Assert.Equal(8, commandLineArgs.Warp.Value!.Map);
    }

    [Fact]
    public void Warp_WithInvalidValue_ShouldNotBePresent()
    {
        // Arrange
        var args = new[] { "-warp", "invalid" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.False(commandLineArgs.Warp.Present);
    }

    [Fact]
    public void Warp_WithNoValue_ShouldNotBePresent()
    {
        // Arrange
        var args = new[] { "-warp" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.False(commandLineArgs.Warp.Present);
    }

    [Fact]
    public void Warp_WithoutArgument_ShouldNotBePresent()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.False(commandLineArgs.Warp.Present);
    }

    [Fact]
    public void Warp_WithThreeValues_ShouldOnlyTakeFirstTwo()
    {
        // Arrange
        var args = new[] { "-warp", "3", "5", "7" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.True(commandLineArgs.Warp.Present);
        Assert.Equal(3, commandLineArgs.Warp.Value!.Episode);
        Assert.Equal(5, commandLineArgs.Warp.Value!.Map);
    }

    #endregion

    #region Episode Tests

    [Fact]
    public void Episode_WithValidValue_ShouldParseCorrectly()
    {
        // Arrange
        var args = new[] { "-episode", "3" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.True(commandLineArgs.Episode.Present);
        Assert.Equal(3, commandLineArgs.Episode.Value);
    }

    [Fact]
    public void Episode_WithInvalidValue_ShouldNotBePresent()
    {
        // Arrange
        var args = new[] { "-episode", "invalid" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.False(commandLineArgs.Episode.Present);
    }

    [Fact]
    public void Episode_WithoutArgument_ShouldNotBePresent()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.False(commandLineArgs.Episode.Present);
    }

    #endregion

    #region Skill Tests

    [Fact]
    public void Skill_WithValidValue_ShouldParseCorrectly()
    {
        // Arrange
        var args = new[] { "-skill", "4" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.True(commandLineArgs.Skill.Present);
        Assert.Equal(4, commandLineArgs.Skill.Value);
    }

    [Fact]
    public void Skill_WithInvalidValue_ShouldNotBePresent()
    {
        // Arrange
        var args = new[] { "-skill", "easy" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.False(commandLineArgs.Skill.Present);
    }

    [Fact]
    public void Skill_WithoutArgument_ShouldNotBePresent()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.False(commandLineArgs.Skill.Present);
    }

    #endregion

    #region Boolean Flag Tests

    [Fact]
    public void DeathMatch_WhenPresent_ShouldBeTrue()
    {
        // Arrange
        var args = new[] { "-deathmatch" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.True(commandLineArgs.DeathMatch.Present);
    }

    [Fact]
    public void DeathMatch_WhenNotPresent_ShouldBeFalse()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.False(commandLineArgs.DeathMatch.Present);
    }

    [Fact]
    public void AltDeath_WhenPresent_ShouldBeTrue()
    {
        // Arrange
        var args = new[] { "-altdeath" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.True(commandLineArgs.AltDeath.Present);
    }

    [Fact]
    public void AltDeath_WhenNotPresent_ShouldBeFalse()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.False(commandLineArgs.AltDeath.Present);
    }

    [Fact]
    public void Fast_WhenPresent_ShouldBeTrue()
    {
        // Arrange
        var args = new[] { "-fast" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.True(commandLineArgs.Fast.Present);
    }

    [Fact]
    public void Fast_WhenNotPresent_ShouldBeFalse()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.False(commandLineArgs.Fast.Present);
    }

    [Fact]
    public void Respawn_WhenPresent_ShouldBeTrue()
    {
        // Arrange
        var args = new[] { "-respawn" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.True(commandLineArgs.Respawn.Present);
    }

    [Fact]
    public void Respawn_WhenNotPresent_ShouldBeFalse()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.False(commandLineArgs.Respawn.Present);
    }

    [Fact]
    public void NoMonsters_WhenPresent_ShouldBeTrue()
    {
        // Arrange
        var args = new[] { "-nomonsters" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.True(commandLineArgs.NoMonsters.Present);
    }

    [Fact]
    public void NoMonsters_WhenNotPresent_ShouldBeFalse()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.False(commandLineArgs.NoMonsters.Present);
    }

    [Fact]
    public void SoloNet_WhenPresent_ShouldBeTrue()
    {
        // Arrange
        var args = new[] { "-solo-net" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.True(commandLineArgs.SoloNet.Present);
    }

    [Fact]
    public void SoloNet_WhenNotPresent_ShouldBeFalse()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.False(commandLineArgs.SoloNet.Present);
    }

    #endregion

    #region PlayDemo Tests

    [Fact]
    public void PlayDemo_WithValidValue_ShouldParseCorrectly()
    {
        // Arrange
        var args = new[] { "-playdemo", "demo1.lmp" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.True(commandLineArgs.PlayDemo.Present);
        Assert.Equal("demo1.lmp", commandLineArgs.PlayDemo.Value);
    }

    [Fact]
    public void PlayDemo_WithoutArgument_ShouldNotBePresent()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.False(commandLineArgs.PlayDemo.Present);
    }

    #endregion

    #region TimeDemo Tests

    [Fact]
    public void TimeDemo_WithValidValue_ShouldParseCorrectly()
    {
        // Arrange
        var args = new[] { "-timedemo", "demo1.lmp" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.True(commandLineArgs.TimeDemo.Present);
        Assert.Equal("demo1.lmp", commandLineArgs.TimeDemo.Value);
    }

    [Fact]
    public void TimeDemo_WithoutArgument_ShouldNotBePresent()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.False(commandLineArgs.TimeDemo.Present);
    }

    #endregion

    #region LoadGame Tests

    [Fact]
    public void LoadGame_WithValidValue_ShouldParseCorrectly()
    {
        // Arrange
        var args = new[] { "-loadgame", "2" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.True(commandLineArgs.LoadGame.Present);
        Assert.Equal(2, commandLineArgs.LoadGame.Value);
    }

    [Fact]
    public void LoadGame_WithInvalidValue_ShouldNotBePresent()
    {
        // Arrange
        var args = new[] { "-loadgame", "save" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.False(commandLineArgs.LoadGame.Present);
    }

    [Fact]
    public void LoadGame_WithoutArgument_ShouldNotBePresent()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.False(commandLineArgs.LoadGame.Present);
    }

    #endregion

    #region Device Control Tests

    [Fact]
    public void NoMouse_WhenPresent_ShouldBeTrue()
    {
        // Arrange
        var args = new[] { "-nomouse" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.True(commandLineArgs.NoMouse.Present);
    }

    [Fact]
    public void NoMouse_WhenNotPresent_ShouldBeFalse()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.False(commandLineArgs.NoMouse.Present);
    }

    [Fact]
    public void NoSound_WhenPresent_ShouldBeTrue()
    {
        // Arrange
        var args = new[] { "-nosound" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.True(commandLineArgs.NoSound.Present);
    }

    [Fact]
    public void NoSound_WhenNotPresent_ShouldBeFalse()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.False(commandLineArgs.NoSound.Present);
    }

    [Fact]
    public void NoSfx_WhenPresent_ShouldBeTrue()
    {
        // Arrange
        var args = new[] { "-nosfx" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.True(commandLineArgs.NoSfx.Present);
    }

    [Fact]
    public void NoSfx_WhenNotPresent_ShouldBeFalse()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.False(commandLineArgs.NoSfx.Present);
    }

    [Fact]
    public void NoMusic_WhenPresent_ShouldBeTrue()
    {
        // Arrange
        var args = new[] { "-nomusic" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.True(commandLineArgs.NoMusic.Present);
    }

    [Fact]
    public void NoMusic_WhenNotPresent_ShouldBeFalse()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.False(commandLineArgs.NoMusic.Present);
    }

    [Fact]
    public void NoDeh_WhenPresent_ShouldBeTrue()
    {
        // Arrange
        var args = new[] { "-nodeh" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.True(commandLineArgs.NoDeh.Present);
    }

    [Fact]
    public void NoDeh_WhenNotPresent_ShouldBeFalse()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.False(commandLineArgs.NoDeh.Present);
    }

    #endregion

    #region Complex Scenarios

    [Fact]
    public void ComplexCommandLine_WithMultipleArguments_ShouldParseAll()
    {
        // Arrange
        var args = new[]
        {
            "-iwad", "doom2.wad",
            "-file", "test1.wad", "test2.wad",
            "-deh", "test.deh",
            "-warp", "15",
            "-skill", "4",
            "-fast",
            "-nomonsters",
            "-nomouse"
        };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.True(commandLineArgs.Iwad.Present);
        Assert.Equal("doom2.wad", commandLineArgs.Iwad.Value);

        Assert.True(commandLineArgs.File.Present);
        Assert.Equal(2, commandLineArgs.File.Value!.Length);

        Assert.True(commandLineArgs.Deh.Present);
        Assert.Single(commandLineArgs.Deh.Value!);

        Assert.True(commandLineArgs.Warp.Present);
        Assert.Equal(1, commandLineArgs.Warp.Value!.Episode);
        Assert.Equal(15, commandLineArgs.Warp.Value!.Map);

        Assert.True(commandLineArgs.Skill.Present);
        Assert.Equal(4, commandLineArgs.Skill.Value);

        Assert.True(commandLineArgs.Fast.Present);
        Assert.True(commandLineArgs.NoMonsters.Present);
        Assert.True(commandLineArgs.NoMouse.Present);
    }

    [Fact]
    public void ComplexCommandLine_WithDemoPlayback_ShouldParseCorrectly()
    {
        // Arrange
        var args = new[]
        {
            "-iwad", "doom2.wad",
            "-playdemo", "demo1.lmp",
            "-nosound"
        };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.True(commandLineArgs.Iwad.Present);
        Assert.True(commandLineArgs.PlayDemo.Present);
        Assert.Equal("demo1.lmp", commandLineArgs.PlayDemo.Value);
        Assert.True(commandLineArgs.NoSound.Present);
    }

    [Fact]
    public void ComplexCommandLine_WithMultiplayer_ShouldParseCorrectly()
    {
        // Arrange
        var args = new[]
        {
            "-iwad", "doom2.wad",
            "-deathmatch",
            "-warp", "1",
            "-skill", "4"
        };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.True(commandLineArgs.Iwad.Present);
        Assert.True(commandLineArgs.DeathMatch.Present);
        Assert.True(commandLineArgs.Warp.Present);
        Assert.Equal(1, commandLineArgs.Warp.Value!.Map);
        Assert.True(commandLineArgs.Skill.Present);
    }

    [Fact]
    public void EmptyCommandLine_ShouldHaveNoArgumentsPresent()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.False(commandLineArgs.Iwad.Present);
        Assert.False(commandLineArgs.File.Present);
        Assert.False(commandLineArgs.Deh.Present);
        Assert.False(commandLineArgs.Warp.Present);
        Assert.False(commandLineArgs.Episode.Present);
        Assert.False(commandLineArgs.Skill.Present);
        Assert.False(commandLineArgs.DeathMatch.Present);
        Assert.False(commandLineArgs.AltDeath.Present);
        Assert.False(commandLineArgs.Fast.Present);
        Assert.False(commandLineArgs.Respawn.Present);
        Assert.False(commandLineArgs.NoMonsters.Present);
        Assert.False(commandLineArgs.SoloNet.Present);
        Assert.False(commandLineArgs.PlayDemo.Present);
        Assert.False(commandLineArgs.TimeDemo.Present);
        Assert.False(commandLineArgs.LoadGame.Present);
        Assert.False(commandLineArgs.NoMouse.Present);
        Assert.False(commandLineArgs.NoSound.Present);
        Assert.False(commandLineArgs.NoSfx.Present);
        Assert.False(commandLineArgs.NoMusic.Present);
        Assert.False(commandLineArgs.NoDeh.Present);
    }

    [Fact]
    public void ArgumentOrder_ShouldNotMatter()
    {
        // Arrange
        var args1 = new[] { "-iwad", "doom2.wad", "-skill", "4", "-warp", "15" };
        var args2 = new[] { "-warp", "15", "-iwad", "doom2.wad", "-skill", "4" };

        // Act
        var commandLineArgs1 = new CommandLineArgs(args1);
        var commandLineArgs2 = new CommandLineArgs(args2);

        // Assert
        Assert.Equal(commandLineArgs1.Iwad.Value, commandLineArgs2.Iwad.Value);
        Assert.Equal(commandLineArgs1.Skill.Value, commandLineArgs2.Skill.Value);
        Assert.Equal(commandLineArgs1.Warp.Value!.Map, commandLineArgs2.Warp.Value!.Map);
    }

    [Fact]
    public void DuplicateArguments_ShouldUseFirstOccurrence()
    {
        // Arrange
        var args = new[] { "-iwad", "doom2.wad", "-iwad", "doom.wad" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.True(commandLineArgs.Iwad.Present);
        Assert.Equal("doom2.wad", commandLineArgs.Iwad.Value);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Argument_AtEndOfArray_WithNoValue_ShouldNotCrash()
    {
        // Arrange
        var args = new[] { "-iwad" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert - Should not throw and argument should not be present
        Assert.False(commandLineArgs.Iwad.Present);
    }

    [Fact]
    public void MultipleFlags_Consecutive_ShouldAllBeParsed()
    {
        // Arrange
        var args = new[] { "-fast", "-respawn", "-nomonsters" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.True(commandLineArgs.Fast.Present);
        Assert.True(commandLineArgs.Respawn.Present);
        Assert.True(commandLineArgs.NoMonsters.Present);
    }

    [Fact]
    public void ValueStartingWithDash_ShouldStopParsing()
    {
        // Arrange
        var args = new[] { "-file", "test1.wad", "-test2.wad", "-skill", "4" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.True(commandLineArgs.File.Present);
        Assert.Single(commandLineArgs.File.Value!);
        Assert.Equal("test1.wad", commandLineArgs.File.Value![0]);
    }

    [Fact]
    public void Warp_WithOneInvalidValue_ShouldNotBePresent()
    {
        // Arrange
        var args = new[] { "-warp", "invalid", "5" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.False(commandLineArgs.Warp.Present);
    }

    [Fact]
    public void Warp_WithSecondInvalidValue_ShouldNotBePresent()
    {
        // Arrange
        var args = new[] { "-warp", "2", "invalid" };

        // Act
        var commandLineArgs = new CommandLineArgs(args);

        // Assert
        Assert.False(commandLineArgs.Warp.Present);
    }

    #endregion
}

