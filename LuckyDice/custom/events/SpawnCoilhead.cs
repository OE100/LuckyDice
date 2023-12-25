using System.Collections;
using System.Collections.Generic;
using GameNetcodeStuff;
using LuckyDice.custom.events.prototype;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace LuckyDice.custom.events
{
    public class SpawnCoilhead : MultiplierDiceEvent<int>
    {
        private List<NetworkObject> coilheads = new List<NetworkObject>();
        public static EnemyType coilhead = null;

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
            if (coilhead == null)
            {
                Plugin.Log.LogError("Coilhead enemy not found!");
            }
            while (running)
            {
                if (playersToMult.Count > 0)
                {
                    if (IsPhaseForbidden())
                    {
                        if (coilheads.Count > 0)
                        {
                            coilheads.ForEach(coil => coil.Despawn());
                            coilheads.Clear();
                        }
                        yield return new WaitForSeconds(10);
                    }
                    else
                    {
                        foreach (var item in playersToMult)
                        {
                            if (item.Value > 0 && item.Key.isInsideFactory && !item.Key.isPlayerDead)
                            {
                                Object.Instantiate(coilhead.enemyPrefab,
                                    (Vector3)(Random.insideUnitCircle * 10) + item.Key.transform.position, Random.rotation);
                                RemovePlayer(item.Key);
                            }
                        }
                    }
                }
                
                yield return new WaitForSeconds(5);
            }
        }
    }
}