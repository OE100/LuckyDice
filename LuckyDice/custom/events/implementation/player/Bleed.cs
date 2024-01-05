﻿#region

using System.Collections;
using System.Collections.Generic;
using GameNetcodeStuff;
using LuckyDice.custom.events.prototype;
using LuckyDice.custom.network;
using Unity.Netcode;
using UnityEngine;

#endregion

namespace LuckyDice.custom.events.implementation.player
{
    public class Bleed : MultiplierDiceEvent<int>
    {
        public override bool IsOneTime() => false;

        public override void AddPlayer(PlayerControllerB player)
        {
            if (playersToMult.ContainsKey(player))
            {
                playersToMult[player]++;
            }
            else
            {
                EventManager.Instance.DisplayMessageClientRPC(
                    new NetworkObjectReference(player.GetComponentInParent<NetworkObject>()), 
                    "Unlucky roll!", 
                    "Your tongue begins to bleed..."
                    );
                playersToMult.Add(player, 1);
            }
        }

        public override void RemovePlayer(PlayerControllerB player)
        {
            if (playersToMult.ContainsKey(player))
            {
                if (playersToMult[player] > 0)
                {
                    playersToMult[player]--;
                }

                if (playersToMult[player] == 0)
                {
                    player.bleedingHeavily = false;
                }
            }
        }

        public override IEnumerator EventCoroutine()
        {
            List<PlayerControllerB> playersToRemove = new List<PlayerControllerB>();
            while (running)
            {
                if (playersToMult.Count > 0)
                {
                    if (IsPhaseForbidden())
                    {
                        foreach (PlayerControllerB player in playersToMult.Keys)
                        {
                            EventManager.Instance.BleedPlayerClientRPC(new NetworkObjectReference(player.GetComponentInParent<NetworkObject>()), false);
                        }
                    }
                    else
                    {
                        foreach (PlayerControllerB player in playersToMult.Keys)
                        {
                            if (player.isInHangarShipRoom)
                            {
                                playersToRemove.Add(player);
                            }
                            if (playersToMult[player] > 0)
                            {
                                EventManager.Instance.BleedPlayerClientRPC(new NetworkObjectReference(player.GetComponentInParent<NetworkObject>()), true, playersToMult[player]);
                            }
                        }
                        playersToRemove.ForEach(RemovePlayer);
                        playersToRemove.Clear();
                    }
                }
                yield return new WaitForSeconds(2.4f);
            }
        }
    }
}