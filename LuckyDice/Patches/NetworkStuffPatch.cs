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
        
        [HarmonyPatch(typeof(GameNetworkManager), "Start"), HarmonyPostfix]
        public static void PatchGameNetworkManagerStart()
        {
            if (networkPrefabs.Count != 0)
                return;
            
            // todo: maybe add a debug null check for each prefab
            networkPrefabs.Add(Plugin.ab.LoadAsset<GameObject>("EventManagerObject.prefab"));
            networkPrefabs.Add(Plugin.ab.LoadAsset<Item>("assets/custom/luckydice/scrap/d4/D4.asset").spawnPrefab);
            networkPrefabs.Add(Plugin.ab.LoadAsset<Item>("assets/custom/luckydice/scrap/d20/D20.asset").spawnPrefab);
            
            foreach (GameObject networkPrefab in networkPrefabs)
                NetworkManager.Singleton.AddNetworkPrefab(networkPrefab);
        }
        
        [HarmonyPatch(typeof(StartOfRound), "Awake"), HarmonyPostfix]
        public static void PatchStartOfRoundAwake()
        {
            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {
                var networkHandlerHost = Object.Instantiate(networkPrefabs[0], Vector3.zero, Quaternion.identity);
                networkHandlerHost.GetComponent<NetworkObject>().Spawn(destroyWithScene: false);
            }
        }
    }
}