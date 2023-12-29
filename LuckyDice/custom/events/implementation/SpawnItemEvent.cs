using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using GameNetcodeStuff;
using LuckyDice.custom.events.prototype;
using LuckyDice.custom.network;
using Unity.Netcode;
using UnityEngine;

namespace LuckyDice.custom.events.implementation
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
            
            player.StartCoroutine(EventCoroutine());
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
            foreach (var item in playersToMult)
            {
                if (item.Value > 0 && item.Key.isInsideFactory && !item.Key.isPlayerDead)
                {
                    EventManager.Instance.SpawnScrapOnPlayer(
                        item.Key, 
                        numberOfItems: numberOfItems,
                        stackValue: stackValue,
                        itemId: itemId
                        );
                }
            }

            playersToRemove.ForEach(RemovePlayer);
            yield break;
        }
    }
}