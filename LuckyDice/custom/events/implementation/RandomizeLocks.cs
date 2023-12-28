using System.Collections;
using System.Collections.Generic;
using GameNetcodeStuff;
using LuckyDice.custom.events.prototype;
using LuckyDice.custom.network;
using Unity.Netcode;
using UnityEngine;

namespace LuckyDice.custom.events.implementation
{
    public class RandomizeLocks : BaseDiceEvent
    {
        public List<DoorLock> doors = new List<DoorLock>();

        public override void AddPlayer(PlayerControllerB player)
        {
            base.AddPlayer(player);
            player.StartCoroutine(EventCoroutine());
        }

        public override IEnumerator EventCoroutine()
        {
            foreach (DoorLock door in doors)
            {
                if (Random.Range(0, 2) == 0)
                    EventManager.Instance.LockDoorClientRPC(new NetworkObjectReference(door.gameObject));
                else
                    door.UnlockDoorSyncWithServer();
            }

            yield break;
        }
    }
}