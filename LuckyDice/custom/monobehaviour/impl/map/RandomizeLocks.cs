using System.Collections.Generic;
using LuckyDice.custom.monobehaviour.attributes;
using LuckyDice.custom.monobehaviour.def;
using LuckyDice.custom.network;
using Unity.Netcode;
using UnityEngine;

namespace LuckyDice.custom.monobehaviour.impl.map
{
    [OneTimeEvent]
    public class RandomizeLocks : BaseEventBehaviour
    {
        public static List<DoorLock> doors = new List<DoorLock>();
        private float timeToNext = 0f;
        private float timeBeforeSwitching = 1f;
        
        private void Awake()
        {
            Plugin.Log.LogDebug($"RandomizeLocks event Awake!");
            
            EventManager.Instance.DisplayMessageClientRPC(
                new NetworkObjectReference(),
                "The locks... they are changing!",
                "Hope you brought a lock pick!"
            );

            timeToNext = timeBeforeSwitching;
        }
        
        protected override void Update()
        {
            if (IsPhaseForbidden())
            {
                Plugin.Log.LogDebug($"RandomizeLocks cleanup!");
                Destroy(this);
            }
            
            if (timeToNext < 0f)
            {
                for (int j = 0; j < 5; j++)
                {
                    int i = Random.Range(0, doors.Count);
                    DoorLock door = doors[i];
                    if (Random.Range(0, 2) == 0)
                    {
                        if (!door.isLocked)
                        {
                            EventManager.Instance.LockDoorClientRPC(new NetworkObjectReference(door.GetComponentInParent<NetworkObject>()));
                        }
                    }
                    else if (door.isLocked)
                    {
                        EventManager.Instance.UnlockDoorClientRPC(new NetworkObjectReference(door.GetComponentInParent<NetworkObject>()));
                    }
                }
                
                timeToNext = timeBeforeSwitching;
            }
        }
    }
}