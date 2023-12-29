using System.Collections;
using System.Collections.Generic;
using GameNetcodeStuff;
using LuckyDice.custom.events.prototype;
using UnityEngine;

namespace LuckyDice.custom.events.implementation
{
    public class SpawnEnemyEvent : MultiplierDiceEvent<int>
    {
        private string name;
        private int amountPerStack;
        
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
                                    for (int i = 0; i < amountPerStack; i++)
                                    {
                                        Vector2 randVect2 = Random.insideUnitCircle * 35;
                                        Vector3 spawnPos = item.Key.transform.position +
                                                           new Vector3(randVect2.x, 0, randVect2.y);
                                        float spawnRot = item.Key.transform.rotation.y;
                                        RoundManager.Instance.SpawnEnemyOnServer(spawnPos, spawnRot, spawnIndex);
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