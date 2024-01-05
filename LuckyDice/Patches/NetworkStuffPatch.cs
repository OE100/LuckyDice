#region

using System.Collections.Generic;
using HarmonyLib;
using LuckyDice.custom.events;
using LuckyDice.custom.items.dice;
using LuckyDice.custom.monobehaviour.impl.player;
using LuckyDice.custom.monobehaviour.impl.tweak;
using LuckyDice.custom.network;
using Unity.Netcode;
using UnityEngine;

#endregion

namespace LuckyDice.Patches
{
    [HarmonyPatch]
    public class NetworkStuffPatch
    {
        internal static List<GameObject> networkPrefabs = new List<GameObject>();
        private static bool done = false;
        
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
                RegisterEventsAndItems();
            }
        }

        private static void RegisterEventsAndItems()
        {
            if (EventManager.Instance == null || EventManager.Instance.gameObject == null)
            {
                Plugin.Log.LogError("We're fucked!!!");
                return;
            }
            
            // register d4 pool
            EventRegistry.RegisterItem(typeof(D4), "D4");
            EventRegistry.RegisterEvent("D4", typeof(MaskedChaos), EventManager.Instance.gameObject);
            
            // register d20 pool
            EventRegistry.RegisterItem(typeof(D20), "D20");
            EventRegistry.RegisterEvent("D20", typeof(TroubleInTerroristTown), EventManager.Instance.gameObject);
        }
    }
}