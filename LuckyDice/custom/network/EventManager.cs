﻿using System.Linq;
using GameNetcodeStuff;
using LuckyDice.custom.events;
using LuckyDice.custom.events.implementation;
using LuckyDice.custom.events.prototype;
using LuckyDice.Patches;
using Unity.Netcode;
using UnityEngine;
using Event = LuckyDice.custom.network.Event;

namespace LuckyDice.custom.network
{
    public class EventManager : NetworkBehaviour
    {
        public static EventManager Instance { get; private set; }
        private readonly IDiceEvent[] Events = {
            new Bleed(),
            new RandomizeLocks(),
            new SpawnEnemyEvent(Enemies.SpringMan),
            new SpawnEnemyEvent(Enemies.Flowerman),
            new SpawnEnemyEvent(Enemies.MaskedPlayerEnemy),
            new SpawnEnemyEvent(Enemies.Jester),
            new SpawnEnemyEvent(Enemies.Centipede, amountPerStack: 4),
        };
        
        public override void OnNetworkSpawn()
        {
            // initialize singleton
            base.OnNetworkSpawn();
            Instance = this;
            
            // start all events
            foreach (IDiceEvent e in Events)
            {
                e.Run();
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void AddPlayerToEventServerRPC(Event e, NetworkObjectReference playerRef)
        {
            if (playerRef.TryGet(out NetworkObject networkObject))
                Events[(int)e].AddPlayer(networkObject.GetComponent<PlayerControllerB>());
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void RemovePlayerFromEventServerRPC(Event e, NetworkObjectReference playerRef)
        {
            if (playerRef.TryGet(out NetworkObject networkObject))
                Events[(int)e].RemovePlayer(networkObject.GetComponent<PlayerControllerB>());
        }
        
        [ClientRpc]
        public void LockDoorClientRPC(NetworkObjectReference doorLockRef)
        {
            Plugin.Log.LogMessage($"Trying to lock door");
            if (doorLockRef.TryGet(out NetworkObject networkObject))
            {
                DoorLock doorLock = networkObject.GetComponent<DoorLock>();
                bool original = doorLock.isLocked;
                doorLock.LockDoor();
                doorLock.doorLockSFX.PlayOneShot(doorLock.unlockSFX);
                Plugin.Log.LogMessage($"Door locked: {original} -> {doorLock.isLocked}");
            }
        }
        
        [ClientRpc]
        public void UnlockDoorClientRPC(NetworkObjectReference doorLockRef)
        {
            Plugin.Log.LogMessage($"Trying to unlock door");
            if (doorLockRef.TryGet(out NetworkObject networkObject))
            {
                DoorLock doorLock = networkObject.GetComponent<DoorLock>();
                bool original = !doorLock.isLocked;
                doorLock.UnlockDoor();
                doorLock.doorLockSFX.PlayOneShot(doorLock.unlockSFX);
                Plugin.Log.LogMessage($"Door unlocked: {original} -> {!doorLock.isLocked}");
            }
        }
        
        [ClientRpc]
        public void BleedPlayerClientRPC(NetworkObjectReference playerRef, bool bleed)
        {
            if (playerRef.TryGet(out NetworkObject networkObject))
                networkObject.GetComponent<PlayerControllerB>().bleedingHeavily = bleed;
        }
    }
}