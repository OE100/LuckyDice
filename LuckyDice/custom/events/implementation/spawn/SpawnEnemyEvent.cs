#region

using System.Collections;
using System.Collections.Generic;
using GameNetcodeStuff;
using LuckyDice.custom.events.prototype;
using LuckyDice.custom.network;
using Unity.Netcode;
using UnityEngine;

#endregion

namespace LuckyDice.custom.events.implementation.spawn
{
    public class SpawnEnemyEvent : MultiplierDiceEvent<int>
    {
        private string name;
        private int amountPerStack;

        public override bool IsOneTime() => false;

        public SpawnEnemyEvent(string name, int amountPerStack = 1)
        {
            this.name = name;
            this.amountPerStack = amountPerStack;
        }
        
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
                        yield return new WaitForSeconds(10);
                    }
                    else
                    {
                        foreach (var item in playersToMult)
                        {
                            if (item.Value > 0 && item.Key.isInsideFactory && !item.Key.isPlayerDead)
                            {
                                int spawnIndex = RoundManager.Instance.currentLevel.Enemies
                                    .FindIndex(x => x.enemyType.name == name);
                                if (spawnIndex >= 0)
                                {
                                    EventManager.Instance.DisplayMessageClientRPC(
                                        new NetworkObjectReference(item.Key.GetComponentInParent<NetworkObject>()),
                                        "The air begins to shift",
                                        "The molecules around you start to shift and rearrange...");

                                    int count = amountPerStack;
                                    
                                    while (count > 0)
                                    {
                                        bool found = Utilities.Utilities.ReturnClosestNavMeshPoint(
                                            Utilities.Utilities.GetRandomLocationAroundPosition(
                                                item.Key.transform.position),
                                            out var position);
                                        if (found)
                                        {
                                            RoundManager.Instance.SpawnEnemyOnServer(
                                                position,
                                                item.Key.transform.rotation.y,
                                                spawnIndex);
                                            
                                            count--;
                                        }
                                        else
                                            Plugin.Log.LogDebug($"Didn't find NavMesh position around player {item.Key.playerUsername}, searching again...");
                                    }
                                    playersToRemove.Add(item.Key);
                                }
                                else
                                {
                                    Plugin.Log.LogError($"{name} spawn index not found!");
                                }
                            }
                        }

                        playersToRemove.ForEach(RemovePlayer);
                    }
                }

                yield return new WaitForSeconds(5);
            }
        }
    }
}