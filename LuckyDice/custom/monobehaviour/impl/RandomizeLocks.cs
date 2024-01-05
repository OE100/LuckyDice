using System.Collections.Generic;
using LuckyDice.custom.monobehaviour.def;
using LuckyDice.custom.network;
using Unity.Netcode;
using UnityEngine;

namespace LuckyDice.custom.monobehaviour.impl
{
    public class RandomizeLocks : BaseEventBehaviour
    {
        public static List<DoorLock> doors = new List<DoorLock>();
        private float timeToNext = 0f;
        private float timeBeforeSwitching = 3f;
        
        private void Awake()
        {
            Plugin.Log.LogDebug($"RandomizeLocks event Awake!");
            IsOneTimeEvent = true;
            NeedsRemoval = false;
            
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
                int i = Random.Range(0, doors.Count);
                DoorLock door = doors[i];
                if (Random.Range(0, 2) == 0)
                {
                    if (!door.isLocked)
                    {
                        LockDoorClientRPC(new NetworkObjectReference(door.GetComponentInParent<NetworkObject>()));
                    }
                }
                else if (door.isLocked)
                {
                    UnlockDoorClientRPC(new NetworkObjectReference(door.GetComponentInParent<NetworkObject>()));
                }
                
                timeToNext = timeBeforeSwitching;
            }
        }
        
        [ClientRpc]
        private void LockDoorClientRPC(NetworkObjectReference doorLockRef)
        {
            Plugin.Log.LogDebug($"Trying to lock door");
            if (doorLockRef.TryGet(out NetworkObject networkObject))
            {
                DoorLock doorLock = networkObject.GetComponentInChildren<DoorLock>();
                bool original = doorLock.isLocked;
                doorLock.LockDoor();
                doorLock.doorLockSFX.PlayOneShot(doorLock.unlockSFX);
                Plugin.Log.LogDebug($"Door locked: {original} -> {doorLock.isLocked}");
            }
        }
            
        [ClientRpc]
        private void UnlockDoorClientRPC(NetworkObjectReference doorLockRef)
        {
            Plugin.Log.LogDebug($"Trying to unlock door");
            if (doorLockRef.TryGet(out NetworkObject networkObject))
            {
                DoorLock doorLock = networkObject.GetComponentInChildren<DoorLock>();
                bool original = !doorLock.isLocked;
                doorLock.UnlockDoor();
                doorLock.doorLockSFX.PlayOneShot(doorLock.unlockSFX);
                Plugin.Log.LogDebug($"Door unlocked: {original} -> {!doorLock.isLocked}");
            }
        }
    }
}