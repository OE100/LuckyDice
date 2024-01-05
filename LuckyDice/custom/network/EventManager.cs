#region

using System.Collections.Generic;
using GameNetcodeStuff;
using LuckyDice.custom.events;
using LuckyDice.custom.events.implementation.player;
using Unity.Netcode;
using UnityEngine;

#endregion

namespace LuckyDice.custom.network
{
    public class EventManager : NetworkBehaviour
    {
        public static EventManager Instance { get; private set; }
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Instance = this;
        }
        
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            Instance = null;
        }

        // rolls event on server
        [ServerRpc(RequireOwnership = false)]
        public void TriggerEventFromPoolServerRPC(NetworkObjectReference triggerRef)
        {
            if (!(NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer))
                return;

            if (!triggerRef.TryGet(out NetworkObject networkObject))
            {
                Plugin.Log.LogDebug($"Didn't find trigger object!");
                return;
            }

            GrabbableObject trigger = networkObject.GetComponentInParent<GrabbableObject>();

            string pool = EventRegistry.GetPoolFromItem(trigger.GetType());
            if (pool == null)
            {
                Plugin.Log.LogWarning($"Item of type: {trigger.GetType()} has no event pool!");
                return;
            }
            
            int eventIndex = EventRegistry.GetRandomEventIndexFromPool(pool);
            if (eventIndex == -1)
            {
                Plugin.Log.LogError($"Event pool: {pool}, is empty!");
                return;
            }

            EventRegistry.RunEventFromPool(pool, eventIndex);
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
        
        // todo: move to event monobehaviour
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
        
        // todo: move to event monobehaviour
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
        
        // todo: move to event monobehaviour
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
        
        // todo: move to event monobehaviour
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

        // todo: move to event monobehaviour
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
        
        // todo: move to event monobehaviour
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

        // todo: move to event monobehaviour
        [ClientRpc]
        public void SetStormClientRPC(bool storm)
        {
            GameObject stormyWeatherObject = GameObject.Find("Systems/GameSystems/TimeAndWeather/Stormy");
            if (stormyWeatherObject == null)
            {
                Plugin.Log.LogError("Stormy weather not found, event not running!");
                return;
            }
            stormyWeatherObject.SetActive(true);
            StormyWeather stormyWeather = stormyWeatherObject.GetComponent<StormyWeather>();
        }
    }
}