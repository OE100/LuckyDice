#region

using System.Collections.Generic;
using GameNetcodeStuff;
using LuckyDice.custom.events.implementation;
using LuckyDice.custom.events.prototype;
using LuckyDice.Patches;
using Unity.Netcode;
using UnityEngine;

#endregion

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
            new SpawnItemEvent(stackValue: 50, numberOfItems: 2, itemId: 25), // clown horn
            new SpawnItemEvent(stackValue: 200, numberOfItems: 1, itemId: 36), // gold bar
            new SpawnItemEvent(stackValue: 50, numberOfItems: 10, itemId: 44), // pickle jar
            new SpawnItemForAllEvent(stackValue: 100, numberOfItems: 1, itemId: 36), // gold bar for all
            new HolyJihad(),
            new MaskedChaos()
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

        [ClientRpc]
        public void DisplayMessageClientRPC(NetworkObjectReference playerRef, string header, string body)
        {
            if (playerRef.TryGet(out NetworkObject networkObject))
            {
                PlayerControllerB player = networkObject.GetComponentInChildren<PlayerControllerB>();
                if (player == StartOfRound.Instance.localPlayerController)
                    HUDManager.Instance.DisplayTip(headerText: header, bodyText: body);
                return;
            }
            HUDManager.Instance.DisplayTip(headerText: header, bodyText: body);
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void AddPlayerToEventServerRPC(Event e, NetworkObjectReference playerRef)
        {
            if (playerRef.TryGet(out NetworkObject networkObject))
                Events[(int)e].AddPlayer(networkObject.GetComponentInChildren<PlayerControllerB>());
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void RemovePlayerFromEventServerRPC(Event e, NetworkObjectReference playerRef)
        {
            if (playerRef.TryGet(out NetworkObject networkObject))
                Events[(int)e].RemovePlayer(networkObject.GetComponentInChildren<PlayerControllerB>());
        }
        
        [ClientRpc]
        public void LockDoorClientRPC(NetworkObjectReference doorLockRef)
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
        public void UnlockDoorClientRPC(NetworkObjectReference doorLockRef)
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
        
        [ClientRpc]
        public void BleedPlayerClientRPC(NetworkObjectReference playerRef, bool bleed, int damage = 0)
        {
            if (playerRef.TryGet(out NetworkObject networkObject))
            {
                PlayerControllerB player = networkObject.GetComponentInChildren<PlayerControllerB>();
                player.bleedingHeavily = bleed;
                if (bleed)
                    player.DamagePlayer(damageNumber: damage, hasDamageSFX: false);
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void SpawnItemAroundPositionServerRPC(Vector3 position, int itemId, int stackValue = 0)
        {
            List<Item> itemsList = StartOfRound.Instance.allItemsList.itemsList;
            Item itemToSpawn = itemId == -1 ? itemsList[Random.Range(0, itemsList.Count)] : itemsList[itemId];
        
            GameObject gameObject = Instantiate(
                itemToSpawn.spawnPrefab, 
                position, 
                Random.rotation
            );
            gameObject.AddComponent<ScanNodeProperties>().scrapValue = stackValue;
            GrabbableObject component = gameObject.GetComponent<GrabbableObject>();
            component.SetScrapValue(stackValue);
            gameObject.GetComponent<NetworkObject>().Spawn();

            Plugin.Log.LogDebug($"Spawned item: {itemToSpawn.itemName}, with value: {stackValue}, at position: ({position.x}, {position.y}, {position.z})");
        }

        [ClientRpc]
        public void PlayJihadSoundFromPlayerClientRPC(NetworkObjectReference playerRef)
        {
            if (playerRef.TryGet(out NetworkObject networkObject))
            {
                PlayerControllerB player = networkObject.GetComponentInChildren<PlayerControllerB>();
                player.voiceMuffledByEnemy = true;
                player.currentVoiceChatAudioSource.PlayOneShot(HolyJihad.beepClip, 1f);
                WalkieTalkie.TransmitOneShotAudio(player.currentVoiceChatAudioSource, HolyJihad.beepClip);
            }
        }
        
        [ClientRpc]
        public void SpawnExplosionOnPlayerClientRPC(NetworkObjectReference playerRef)
        {
            if (playerRef.TryGet(out NetworkObject networkObject))
            {
                PlayerControllerB player = networkObject.GetComponentInChildren<PlayerControllerB>();
                player.currentVoiceChatAudioSource.PlayOneShot(HolyJihad.jihadClip);
                WalkieTalkie.TransmitOneShotAudio(player.currentVoiceChatAudioSource, HolyJihad.jihadClip);
                Landmine.SpawnExplosion(
                    player.transform.position,
                    killRange: 10f,
                    damageRange: 14f,
                    spawnExplosionEffect: true
                );
            }
        }
    }
}