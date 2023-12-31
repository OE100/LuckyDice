#region

using System.Collections;
using System.Collections.Generic;
using GameNetcodeStuff;
using LuckyDice.custom.events.prototype;
using LuckyDice.custom.network;
using Unity.Netcode;
using UnityEngine;

#endregion

namespace LuckyDice.custom.events.implementation.map
{
    public class RandomizeLocks : BaseDiceEvent
    {
        public static List<DoorLock> doors = new List<DoorLock>();

        public override void AddPlayer(PlayerControllerB player)
        {
            if (StartOfRound.Instance.IsHost || StartOfRound.Instance.IsServer)
                player.StartCoroutine(EventCoroutine());
        }

        public override void Run()
        {
        }

        public override IEnumerator EventCoroutine()
        {
            EventManager.Instance.DisplayMessageClientRPC(
                new NetworkObjectReference(),
                "The locks... they are changing!",
                "Hope you brought a lock pick!"
                );
            foreach (DoorLock door in doors)
            {
                if (Random.Range(0, 2) == 0)
                {
                    if (!door.isLocked)
                        EventManager.Instance.LockDoorClientRPC(new NetworkObjectReference(door.GetComponentInParent<NetworkObject>()));
                }
                else if (door.isLocked)
                {
                    EventManager.Instance.UnlockDoorClientRPC(new NetworkObjectReference(door.GetComponentInParent<NetworkObject>()));
                }
            }

            yield break;
        }
    }
}