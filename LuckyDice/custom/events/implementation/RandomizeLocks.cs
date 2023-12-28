using System.Collections;
using System.Collections.Generic;
using GameNetcodeStuff;
using LuckyDice.custom.events.prototype;
using LuckyDice.custom.network;
using UnityEngine;

namespace LuckyDice.custom.events.implementation
{
    public class RandomizeLocks : BaseDiceEvent
    {
        public static List<DoorLock> DoorLocks = new List<DoorLock>();

        public override void AddPlayer(PlayerControllerB player)
        {
            if (RoundManager.Instance.IsServer)
            {
                base.AddPlayer(player);
                player.StartCoroutine(EventCoroutine());
            }
        }

        public override IEnumerator EventCoroutine()
        {
            foreach (DoorLock doorLock in DoorLocks)
            {
                if (Random.Range(0, 2) == 0)
                    EventManager.Instance.LockDoorClientRPC(doorLock);
                else
                    doorLock.UnlockDoorSyncWithServer();
            }

            yield break;
        }
    }
}