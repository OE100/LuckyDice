#region

using System.Collections.Generic;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

#endregion

namespace LuckyDice.Patches
{
    [HarmonyPatch]
    public class NetworkStuffPatch
    {
        internal static List<GameObject> networkPrefabs = new List<GameObject>();
        private static bool done;
        
        [HarmonyPatch(typeof(GameNetworkManager), "Start"), HarmonyPostfix]
        public static void PatchGameNetworkManagerStart()
        {
            if (done || networkPrefabs.Count == 0)
                return;
            
            foreach (GameObject networkPrefab in networkPrefabs)
                NetworkManager.Singleton.AddNetworkPrefab(networkPrefab);

            done = true;
        }
        
        [HarmonyPatch(typeof(StartOfRound), "Awake"), HarmonyPostfix]
        public static void PatchStartOfRoundAwake()
        {
            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {
                var networkHandlerHost = Object.Instantiate(networkPrefabs[0], Vector3.zero, Quaternion.identity);
                networkHandlerHost.GetComponent<NetworkObject>().Spawn(destroyWithScene: false);
                if (ModConfig.RegisterDiceToEventPools.Value)
                    Plugin.RegisterEventsAndItems();
            }
        }
    }
}