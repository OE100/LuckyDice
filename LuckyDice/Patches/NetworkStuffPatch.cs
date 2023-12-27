﻿using System.Collections.Generic;
using HarmonyLib;
using LuckyDice.custom.network;
using Unity.Netcode;
using UnityEngine;

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
            
            networkPrefabs.Add(Plugin.ab.LoadAsset<GameObject>("EventManagerObject.prefab"));
            networkPrefabs.Add(Plugin.ab.LoadAsset<Item>("assets/custom/luckydice/scrap/d20/20SidedDice.asset").spawnPrefab);
            
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