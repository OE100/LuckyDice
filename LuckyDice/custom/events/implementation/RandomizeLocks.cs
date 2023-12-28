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
            foreach (DoorLock door in doors)
            {
                if (Random.Range(0, 2) == 0)
                {
                    if (!door.isLocked)
                        EventManager.Instance.LockDoorClientRPC(new NetworkObjectReference(door.gameObject));
                }
                else if (door.isLocked)
                {
                    EventManager.Instance.UnlockDoorClientRPC(new NetworkObjectReference(door.gameObject));
                }
            }

            yield break;
        }
    }
}