using System.Linq;
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
        public void StartEventServerRPC(Event e)
        {
            StartEventClientRPC(e);
        }
        
        [ClientRpc]
        public void StartEventClientRPC(Event e)
        {
            Events[(int)e].Run();
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void StopEventServerRPC(Event e)
        {
            StopEventClientRPC(e);
        }
        
        [ClientRpc]
        public void StopEventClientRPC(Event e)
        {
            Events[(int)e].Stop();
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void AddPlayerToEventServerRPC(Event e, NetworkObjectReference playerRef)
        {
            AddPlayerToEventClientRPC(e, playerRef);
        }
        
        [ClientRpc]
        public void AddPlayerToEventClientRPC(Event e, NetworkObjectReference playerRef)
        {
            if (playerRef.TryGet(out NetworkObject networkObject))
                Events[(int)e].AddPlayer(networkObject.GetComponent<PlayerControllerB>());
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void RemovePlayerFromEventServerRPC(Event e, NetworkObjectReference playerRef)
        {
            RemovePlayerFromEventClientRPC(e, playerRef);
        }
        
        [ClientRpc]
        public void RemovePlayerFromEventClientRPC(Event e, NetworkObjectReference playerRef)
        {
            if (playerRef.TryGet(out NetworkObject networkObject))
                Events[(int)e].RemovePlayer(networkObject.GetComponent<PlayerControllerB>());
        }
        
        [ClientRpc]
        public void LockDoorClientRPC(NetworkObjectReference doorLockRef)
        {
            if (doorLockRef.TryGet(out NetworkObject networkObject))
                networkObject.GetComponent<DoorLock>().LockDoor();
        }
        
        [ClientRpc]
        public void BleedPlayerClientRPC(NetworkObjectReference playerRef, bool bleed)
        {
            if (playerRef.TryGet(out NetworkObject networkObject))
                networkObject.GetComponent<PlayerControllerB>().bleedingHeavily = bleed;
        }
    }
}