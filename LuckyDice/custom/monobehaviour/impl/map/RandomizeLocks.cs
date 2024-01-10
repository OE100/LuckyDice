using LuckyDice.custom.monobehaviour.attributes;
using LuckyDice.custom.monobehaviour.def;
using LuckyDice.custom.network;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LuckyDice.custom.monobehaviour.impl.map
{
    [OneTimeEvent]
    public class RandomizeLocks : BaseEventBehaviour
    {
        public static List<DoorLock> Doors = [];
        protected float TimeToNext = 0f;
        protected const float TimeBeforeSwitching = 10f; // todo: read from config

        private void Awake()
        {
            Plugin.Log.LogDebug($"RandomizeLocks event Awake!");
            
            EventManager.Instance.DisplayMessageClientRPC(
                new NetworkObjectReference(),
                "The locks... they are changing!",
                "Hope you brought a lock pick!"
            );

            TimeToNext = TimeBeforeSwitching;
        }
        
        protected override void Update()
        {
            if (IsPhaseForbidden())
            {
                Plugin.Log.LogDebug($"RandomizeLocks cleanup!");
                Destroy(this);
            }
            
            TimeToNext -= Time.deltaTime;
            
            if (TimeToNext < 0f)
            {
                for (var j = 0; j < 5; j++)
                {
                    int i = Random.Range(0, Doors.Count);
                    DoorLock door = Doors[i];
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
                
                TimeToNext = TimeBeforeSwitching;
            }
        }
    }
}