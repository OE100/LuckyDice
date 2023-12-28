using System.Collections;
using System.Collections.Generic;
using GameNetcodeStuff;
using LuckyDice.custom.events.prototype;
using LuckyDice.custom.network;
using Unity.Netcode;
using UnityEngine;

namespace LuckyDice.custom.events.implementation
{
    public class Bleed : MultiplierDiceEvent<int>
    {
        public override void AddPlayer(PlayerControllerB player)
        {
            if (playersToMult.ContainsKey(player))
            {
                playersToMult[player]++;
            }
            else
            {
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
                            EventManager.Instance.BleedPlayerClientRPC(new NetworkObjectReference(player.gameObject), false);
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
                                EventManager.Instance.BleedPlayerClientRPC(new NetworkObjectReference(player.gameObject), true);
                                player.DamagePlayer(damageNumber: playersToMult[player], hasDamageSFX: false);
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