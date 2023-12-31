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
    public class SpawnItemEvent : MultiplierDiceEvent<int>
    {
        private int stackValue;
        private int numberOfItems;
        private int itemId;
        
        public SpawnItemEvent(int stackValue = 70, int numberOfItems = 1, int itemId = -1)
        {
            this.stackValue = stackValue;
            this.numberOfItems = numberOfItems;
            this.itemId = itemId;
        }

        public override void AddPlayer(PlayerControllerB player)
        {
            if (!NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsServer)
                return;
            
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
                    if (!IsPhaseForbidden())
                    {
                        foreach (var item in playersToMult)
                        {
                            if (item.Value <= 0)
                                continue;

                            EventManager.Instance.DisplayMessageClientRPC(
                                new NetworkObjectReference(item.Key.GetComponentInParent<NetworkObject>()),
                                "The air begins to shift",
                                "The molecules around you start to shift and rearrange...");

                            yield return new WaitForSeconds(5);
                            
                            int count = numberOfItems;

                            while (count > 0)
                            {
                                Vector3 randomPos = Utilities.Utilities.GetRandomLocationAroundPosition(
                                    item.Key.transform.position,
                                    radius: 5,
                                    randomHeight: true);

                                bool found = Utilities.Utilities.ReturnClosestNavMeshPoint(
                                    randomPos,
                                    out var closestPoint);

                                if (!found)
                                    continue;
                                
                                EventManager.Instance.SpawnItemAroundPositionServerRPC(
                                    position: closestPoint,
                                    itemId: itemId,
                                    stackValue: stackValue);
                                
                                count--;
                            }

                            playersToRemove.Add(item.Key);
                        }
                    }

                    playersToRemove.ForEach(RemovePlayer);
                }

                yield return new WaitForSeconds(5);
            }
        }
    }
}