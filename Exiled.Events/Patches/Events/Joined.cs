// -----------------------------------------------------------------------
// <copyright file="Joined.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events
{
    #pragma warning disable SA1313
    using Exiled.Events.Handlers;
    using Exiled.Events.Handlers.EventArgs;
    using HarmonyLib;
    using MEC;

    /// <summary>
    /// Patches <see cref="NicknameSync.SetNick(string)"/>.
    /// Adds the <see cref="Player.Joined"/> event.
    /// </summary>
    [HarmonyPatch(typeof(NicknameSync), nameof(NicknameSync.SetNick))]
    public class Joined
    {
        /// <summary>
        /// Prefix of <see cref="NicknameSync.SetNick(string)"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="NicknameSync"/> instance.</param>
        public static void Postfix(NicknameSync __instance)
        {
            if (__instance.hub.characterClassManager.IsHost || string.IsNullOrEmpty(__instance.hub.characterClassManager.UserId))
                return;

            if (!API.Features.Player.Dictionary.TryGetValue(__instance.gameObject, out API.Features.Player player))
            {
                player = new API.Features.Player(ReferenceHub.GetHub(__instance.gameObject));

                API.Features.Player.Dictionary.Add(__instance.gameObject, player);
            }

            API.Features.Log.Debug($"Player {player?.Nickname} ({player?.UserId}) connected with the IP: {player?.IPAddress}");

            if (PlayerManager.players.Count >= CustomNetworkManager.slots)
                API.Features.Log.Debug($"Server is full!");

            Timing.CallDelayed(0.25f, () =>
            {
                if (player != null && player.IsMuted)
                    player.ReferenceHub.characterClassManager.SetDirtyBit(1UL);
            });

            var ev = new JoinedEventArgs(API.Features.Player.Get(__instance.gameObject));

            Player.OnJoined(ev);
        }
    }
}