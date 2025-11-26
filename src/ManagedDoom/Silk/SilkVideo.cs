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
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using ManagedDoom.Config;
using ManagedDoom.Doom.Math;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using TrippyGL;
using ManagedDoom.Video;

namespace ManagedDoom.Silk;

public sealed class SilkVideo : IVideo
{
    private readonly Renderer renderer;

    private GraphicsDevice? device;

    private readonly int textureWidth;
    private readonly int textureHeight;

    private readonly byte[] textureData;
    private Texture2D? texture;

    private TextureBatcher? textureBatcher;
    private SimpleShaderProgram? shader;

    private int silkWindowWidth;
    private int silkWindowHeight;

    public SilkVideo(ConfigValues config, Renderer renderer, IWindow window, GL gl)
    {
        Console.Write("Initialize video: ");
        var start = Stopwatch.GetTimestamp();

        try
        {
            this.renderer = renderer;

            device = new GraphicsDevice(gl);

            if (config.VideoHighResolution)
            {
                textureWidth = 512;
                textureHeight = 1024;
            }
            else
            {
                textureWidth = 256;
                textureHeight = 512;
            }

            textureData = new byte[4 * renderer.Width * renderer.Height];
            texture = new Texture2D(device, (uint)textureWidth, (uint)textureHeight);
            texture.SetTextureFilters(TrippyGL.TextureMinFilter.Nearest, TrippyGL.TextureMagFilter.Nearest);

            textureBatcher = new TextureBatcher(device);
            shader = SimpleShaderProgram.Create<VertexColorTexture>(device);
            textureBatcher.SetShaderProgram(shader);

            device.BlendingEnabled = false;

            Resize(window.Size.X, window.Size.Y);

            var end = Stopwatch.GetElapsedTime(start);
            Console.WriteLine($"OK [{end}]");
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed");
            Dispose();
            ExceptionDispatchInfo.Throw(e);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Render(Doom.Doom doom, Fixed frameFrac, in long fps)
    {
        renderer.Render(doom, textureData, frameFrac, in fps);

        texture!.SetData(textureData, 0, 0, (uint)renderer.Height, (uint)renderer.Width);

        var u = (float)renderer.Height / textureWidth;
        var v = (float)renderer.Width / textureHeight;
        var tl = new VertexColorTexture(Vector3.Zero, Color4b.White, Vector2.Zero);
        var tr = new VertexColorTexture(new Vector3(silkWindowWidth, 0, 0), Color4b.White, new Vector2(0, v));
        var br = new VertexColorTexture(new Vector3(silkWindowWidth, silkWindowHeight, 0), Color4b.White, new Vector2(u, v));
        var bl = new VertexColorTexture(new Vector3(0, silkWindowHeight, 0), Color4b.White, new Vector2(u, 0));

        textureBatcher!.Begin();
        textureBatcher.DrawRaw(texture, tl, tr, br, bl);
        textureBatcher.End();
    }

    public void Resize(int width, int height)
    {
        silkWindowWidth = width;
        silkWindowHeight = height;
        device!.SetViewport(0, 0, (uint)width, (uint)height);
        shader!.Projection = Matrix4x4.CreateOrthographicOffCenter(0, width, height, 0, 0, 1);
    }

    public void InitializeWipe()
    {
        renderer.InitializeWipe();
    }

    public bool HasFocus()
    {
        return true;
    }

    public void Dispose()
    {
        Console.WriteLine("Shutdown video.");

        shader?.Dispose();
        shader = null;

        textureBatcher?.Dispose();
        textureBatcher = null;

        texture?.Dispose();
        texture = null;

        device?.Dispose();
        device = null;
    }

    public int WipeBandCount => renderer.WipeBandCount;
    public int WipeHeight => renderer.WipeHeight;

    public int MaxWindowSize => renderer.MaxWindowSize;

    public int WindowSize
    {
        get => renderer.WindowSize;
        set => renderer.WindowSize = value;
    }

    public bool DisplayMessage
    {
        get => renderer.DisplayMessage;
        set => renderer.DisplayMessage = value;
    }

    public int MaxGammaCorrectionLevel => Renderer.MaxGammaCorrectionLevel;

    public int GammaCorrectionLevel
    {
        get => renderer.GammaCorrectionLevel;
        set => renderer.GammaCorrectionLevel = value;
    }
}