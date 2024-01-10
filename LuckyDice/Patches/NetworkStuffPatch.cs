using HarmonyLib;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LuckyDice.Patches
{
    [HarmonyPatch]
    public class NetworkStuffPatch
    {
        internal static List<GameObject> networkPrefabs = [];
        private static bool done;
        
        [HarmonyPatch(typeof(GameNetworkManager), "Start"), HarmonyPostfix]
        public static void PatchGameNetworkManagerStart()
        {
            if (done || networkPrefabs.Count == 0)
                return;
            
            foreach (var networkPrefab in networkPrefabs)
                NetworkManager.Singleton.AddNetworkPrefab(networkPrefab);

            done = true;
        }
        
        [HarmonyPatch(typeof(StartOfRound), "Awake"), HarmonyPostfix]
        public static void PatchStartOfRoundAwake()
        {
            if (!(NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)) 
                return;
            
            var networkHandlerHost = Object.Instantiate(networkPrefabs[0], Vector3.zero, Quaternion.identity);
            networkHandlerHost.GetComponent<NetworkObject>().Spawn(destroyWithScene: false);
            if (ModConfig.RegisterDiceToEventPools.Value)
                Plugin.RegisterEventsAndItems();
        }
    }
}