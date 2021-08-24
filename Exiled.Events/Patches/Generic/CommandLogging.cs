// -----------------------------------------------------------------------
// <copyright file="CommandLogging.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1118
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection.Emit;

    using Exiled.API.Features;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using RemoteAdmin;

    using static HarmonyLib.AccessTools;

    using Events = Exiled.Events.Events;

    /// <summary>
    /// Patches <see cref="RemoteAdmin.CommandProcessor.ProcessQuery"/> for command logging.
    /// </summary>
    [HarmonyPatch(typeof(CommandProcessor), nameof(CommandProcessor.ProcessQuery))]
    internal class CommandLogging
    {
        /// <summary>
        /// Logs a command to the RA log file.
        /// </summary>
        /// <param name="query">The command being logged.</param>
        /// <param name="sender">The sender of the command.</param>
        public static void LogCommand(string query, CommandSender sender)
        {
            if (query.ToUpperInvariant().StartsWith("REQUEST_DATA"))
                return;

            Player player = sender is PlayerCommandSender playerCommandSender
                ? Player.Get(playerCommandSender)
                : Server.Host;

            string logMessage =
                $"[{DateTime.Now}] {(player == Server.Host ? "Server Console" : $"{player.Nickname} ({player.UserId}) {player.IPAddress}")} has run the command {query}.\n";
            string directory = Path.Combine(Paths.Exiled, "Logs");
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            string filePath = Path.Combine(directory, $"{Server.Port}-RAlog.txt");
            if (!File.Exists(filePath))
                File.Create(filePath).Close();
            File.AppendAllText(filePath, logMessage);
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            const int index = 0;
            Label continueLabel = generator.DefineLabel();
            newInstructions[newInstructions.FindIndex(i => i.opcode == OpCodes.Ldarg_1)].WithLabels(continueLabel);

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Events), nameof(Events.Instance))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Events), nameof(Events.Config))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Config), nameof(Config.LogRaCommands))),
                new CodeInstruction(OpCodes.Brfalse, continueLabel),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, Method(typeof(CommandLogging), nameof(LogCommand))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
