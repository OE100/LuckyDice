using System.Collections;
using GameNetcodeStuff;
using LuckyDice.custom.events;
using LuckyDice.custom.monobehaviour.impl.persistent.weathercredits;
using LuckyDice.Patches;
using LuckyDice.Utilities;
using TerminalApi.Classes;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace LuckyDice.custom.network
{
    public class EventManager : NetworkBehaviour
    {
        public static EventManager Instance { get; private set; } = null!;
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Instance = this;
        }

        [ServerRpc(RequireOwnership = false)]
        public void TriggerEventFromPoolServerRPC(NetworkObjectReference triggerRef, NetworkObjectReference playerRef, int slot = -1)
        {
            if (!(NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer))
            {
                Plugin.Log.LogDebug("Only host can trigger events!");
                return;
            }
            
            Plugin.Log.LogDebug($"Player: {StartOfRound.Instance.localPlayerController.playerUsername} is the host!");

            if (!triggerRef.TryGet(out var networkObject))
            {
                Plugin.Log.LogDebug("Didn't find trigger object!");
                return;
            }

            var trigger = networkObject.GetComponentInParent<GrabbableObject>();

            var pool = EventRegistry.GetPoolFromItem(trigger.GetType());
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

            if (!playerRef.TryGet(out var playerNetworkObject))
            {
                Plugin.Log.LogError("Didn't find player object!");
                return;
            }
            
            var playerHeldBy = playerNetworkObject.GetComponentInChildren<PlayerControllerB>();
            
            EventRegistry.RunEventFromPool(pool, eventIndex, playerHeldBy);
        }


        // RPCs
        
        [ClientRpc]
        public void DisplayMessageClientRPC(NetworkObjectReference playerRef, string header, string body)
        {
            if (playerRef.TryGet(out var networkObject))
            {
                PlayerControllerB player = networkObject.GetComponentInChildren<PlayerControllerB>();
                if (player == StartOfRound.Instance.localPlayerController)
                    HUDManager.Instance.DisplayTip(headerText: header, bodyText: body);
                return;
            }
            HUDManager.Instance.DisplayTip(headerText: header, bodyText: body);
        }
        
        [ClientRpc]
        public void BleedPlayerClientRPC(NetworkObjectReference playerRef, bool bleed, int damage = 0)
        {
            if (playerRef.TryGet(out var networkObject))
            {
                PlayerControllerB player = networkObject.GetComponentInChildren<PlayerControllerB>();
                player.bleedingHeavily = bleed;
                if (bleed)
                    player.DamagePlayer(damageNumber: damage, hasDamageSFX: false);
            }
        }
        
        [ServerRpc]
        public void SpawnItemAroundPositionServerRPC(Vector3 position, int itemId, int stackValue = 0)
        {
            var itemsList = StartOfRound.Instance.allItemsList.itemsList;
            var itemToSpawn = itemId == -1 ? itemsList[Random.Range(0, itemsList.Count)] : itemsList[itemId];
            
            var parent = RoundManager.Instance.spawnedScrapContainer == null ? StartOfRound.Instance.elevatorTransform : RoundManager.Instance.spawnedScrapContainer;
            position.y += 1;
            
            var itemObject = Instantiate(
                itemToSpawn.spawnPrefab, 
                position, 
                Random.rotation,
                parent
            );
            
            var component = itemObject.GetComponent<GrabbableObject>();
            component.NetworkObject.Spawn();

            InitializeItemClientRPC(new NetworkObjectReference(component.NetworkObject), stackValue);
        }

        private IEnumerator delayedItemSpawn(NetworkObjectReference itemRef, int value)
        {
            Plugin.Log.LogDebug("Waiting for item to spawn");
            NetworkObject networkObject = null!;
            yield return new WaitUntil(() => itemRef.TryGet(out networkObject));
            Plugin.Log.LogDebug("Item spawned, syncing to clients");

            GrabbableObject component = networkObject.GetComponentInParent<GrabbableObject>();
            yield return new WaitForEndOfFrame();
            component.SetScrapValue(value);
            Plugin.Log.LogDebug($"Spawned item: {component.GetType().Name}, with value: {value}, at position: {component.transform.position}");
        }
        
        [ClientRpc]
        private void InitializeItemClientRPC(NetworkObjectReference itemRef, int value)
        {
            StartCoroutine(delayedItemSpawn(itemRef, value));
        } 

        [ClientRpc]
        public void PlayTerroristSoundFromPlayerClientRPC(NetworkObjectReference playerRef)
        {
            if (playerRef.TryGet(out NetworkObject networkObject))
            {
                var player = networkObject.GetComponentInChildren<PlayerControllerB>();
                player.voiceMuffledByEnemy = true;
                // player.currentVoiceChatAudioSource.PlayOneShot(HolyJihad.beepClip, 1f); // todo: fix this
                // WalkieTalkie.TransmitOneShotAudio(player.currentVoiceChatAudioSource, HolyJihad.beepClip); // todo: fix this
            }
        }
        
        [ClientRpc]
        public void SpawnExplosionOnPlayerClientRPC(NetworkObjectReference playerRef)
        {
            if (playerRef.TryGet(out NetworkObject networkObject))
            {
                var player = networkObject.GetComponentInChildren<PlayerControllerB>();
                // player.currentVoiceChatAudioSource.PlayOneShot(HolyJihad.jihadClip); // todo: fix this
                // WalkieTalkie.TransmitOneShotAudio(player.currentVoiceChatAudioSource, HolyJihad.jihadClip); // todo: fix this
                Landmine.SpawnExplosion(
                    player.transform.position,
                    killRange: 10f,
                    damageRange: 14f,
                    spawnExplosionEffect: true
                );
            }
        }
        
        [ClientRpc]
        public void SetStormClientRPC(bool storm)
        {
            if (Utils.StormyWeatherContainer == null || Utils.StormyRainContainer == null)
            {
                Plugin.Log.LogError("Stormy weather not found, event not running!");
                return;
            }
            Utils.StormyWeatherContainer.SetActive(storm);
            Utils.StormyRainContainer.SetActive(storm);
        }
        
        [ClientRpc]
        public void LockDoorClientRPC(NetworkObjectReference doorLockRef)
        {
            Plugin.Log.LogDebug($"Trying to lock door");
            if (doorLockRef.TryGet(out var networkObject))
            {
                var doorLock = networkObject.GetComponentInChildren<DoorLock>();
                var original = doorLock.isLocked;
                doorLock.LockDoor();
                doorLock.doorLockSFX.PlayOneShot(doorLock.unlockSFX);
                Plugin.Log.LogDebug($"Door locked: {original} -> {doorLock.isLocked}");
            }
        }
            
        [ClientRpc]
        public void UnlockDoorClientRPC(NetworkObjectReference doorLockRef)
        {
            Plugin.Log.LogDebug($"Trying to unlock door");
            if (doorLockRef.TryGet(out var networkObject))
            {
                var doorLock = networkObject.GetComponentInChildren<DoorLock>();
                var original = !doorLock.isLocked;
                doorLock.UnlockDoor();
                doorLock.doorLockSFX.PlayOneShot(doorLock.unlockSFX);
                Plugin.Log.LogDebug($"Door unlocked: {original} -> {!doorLock.isLocked}");
            }
        }

        [ClientRpc]
        public void SetMaskedEnemyChangesClientRPC(bool triggered)
        {
            MaskedEnemyChanges.Triggered = triggered;
        }

        [ClientRpc]
        public void TeleportPlayerClientRPC(NetworkObjectReference playerRef, Vector3 position)
        {
            if (!playerRef.TryGet(out var networkObject))
            {
                Plugin.Log.LogError("Couldn't find player object!");
                return;
            }
            
            var player = networkObject.GetComponentInChildren<PlayerControllerB>();
            player.TeleportPlayer(position);
        }
        
        [ClientRpc]
        public void TeleportEntityClientRPC(NetworkObjectReference entityRef, Vector3 position)
        {
            if (!entityRef.TryGet(out var networkObject))
            {
                Plugin.Log.LogError("Couldn't find entity object!");
                return;
            }
            
            var agent = networkObject.GetComponentInChildren<NavMeshAgent>();
            agent.Warp(position);
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void SetWeatherServerRPC(Weather weather)
        {
            if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
                SetWeatherClientRPC(weather);
        }
        
        [ClientRpc]
        public void SetWeatherClientRPC(Weather weather)
        {
            Utils.DisableAllWeather();
            switch (weather)
            {
                case Weather.Clear:
                    break;
                case Weather.Rainy:
                    Utils.RainyWeatherContainer.SetActive(true);
                    break;
                case Weather.Thunder:
                    Utils.StormyWeatherContainer.SetActive(true);
                    Utils.StormyRainContainer.SetActive(true);
                    break;
                case Weather.Flooding:
                    Utils.FloodingWeatherContainer.SetActive(true);
                    break;
                case Weather.Foggy:
                    Utils.FoggyWeatherContainer.SetActive(true);
                    break;
                case Weather.Eclipse:
                    Utils.EclipseWeatherContainer.SetActive(true);
                    break;
                case Weather.DustStorm:
                    Utils.DustStormWeatherContainer.SetActive(true);
                    break;
                default:
                    Plugin.Log.LogDebug("Why are you trying to break my code using reflection?!");
                    break;
            }
        }
    }
}