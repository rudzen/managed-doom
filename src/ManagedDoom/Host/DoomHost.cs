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
using System.Threading;
using System.Threading.Tasks;
using ManagedDoom.Silk;
using Microsoft.Extensions.Hosting;

namespace ManagedDoom.Host;

public sealed class DoomHost(ISilkDoom silkDoom, IHostApplicationLifetime lifetime) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await silkDoom.Run();
        lifetime.StopApplication();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        if (silkDoom.Exception is not null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(silkDoom.Exception.Message);
            Console.ResetColor();
            Console.WriteLine();
        }

        if (!string.IsNullOrWhiteSpace(silkDoom.QuitMessage))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(silkDoom.QuitMessage);
            Console.ResetColor();
        }

        return Task.CompletedTask;
    }
}