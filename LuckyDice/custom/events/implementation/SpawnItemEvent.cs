using System.Collections;
using System.Collections.Generic;
using GameNetcodeStuff;
using LuckyDice.custom.events.prototype;
using UnityEngine;

namespace LuckyDice.custom.events.implementation
{
    public class SpawnItemEvent : MultiplierDiceEvent<int>
    {
        private int stackValue;
        private int numberOfItems;
        
        public SpawnItemEvent(int stackValue = 70, int numberOfItems = 1)
        {
            this.stackValue = stackValue;
            this.numberOfItems = this.numberOfItems;
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
                                // todo: spawn items of stackValue split between numberOfItems items
                            }
                        }

                        playersToRemove.ForEach(RemovePlayer);
                    }
                }

                yield return new WaitForSeconds(5);
            }
        }
        
        private void SpawnScrapOnPlayer(PlayerControllerB player)
        {
            Vector3 position = player.transform.position + (Vector3)(Random.insideUnitCircle * 5);
            float rotation = player.transform.rotation.y;
            // todo: finish implementing this
        }
    }
}