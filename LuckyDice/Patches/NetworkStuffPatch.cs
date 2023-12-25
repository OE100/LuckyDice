using HarmonyLib;
using LuckyDice.custom.network;
using Unity.Netcode;
using UnityEngine;

namespace LuckyDice.Patches
{
    [HarmonyPatch]
    public class NetworkStuffPatch
    {
        private static GameObject networkPrefab;
        
        [HarmonyPatch(typeof(GameNetworkManager), "Start"), HarmonyPostfix]
        public static void PatchGameNetworkManagerStart()
        {
            if (networkPrefab != null)
                return;

            networkPrefab = (GameObject)Plugin.ab.LoadAsset("EventManagerObject.prefab");
            networkPrefab.AddComponent<EventManager>();
            
            NetworkManager.Singleton.AddNetworkPrefab(networkPrefab);
        }
        
        [HarmonyPatch(typeof(StartOfRound), "Awake"), HarmonyPostfix]
        public static void PatchStartOfRoundAwake()
        {
            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {
                var networkHandlerHost = Object.Instantiate(networkPrefab, Vector3.zero, Quaternion.identity);
                networkHandlerHost.GetComponent<NetworkObject>().Spawn();
            }
        }
    }
}