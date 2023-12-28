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
        public static Dictionary<ulong, DoorLock> DoorLocks = new Dictionary<ulong, DoorLock>();

        public override void AddPlayer(PlayerControllerB player)
        {
            base.AddPlayer(player);
            player.StartCoroutine(EventCoroutine());
        }

        public override IEnumerator EventCoroutine()
        {
            foreach (KeyValuePair<ulong,DoorLock> pair in DoorLocks)
            {
                if (Random.Range(0, 2) == 0)
                    EventManager.Instance.LockDoorClientRPC(pair.Key);
                else
                    pair.Value.UnlockDoorSyncWithServer();
            }

            yield break;
        }
    }
}