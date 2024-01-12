#region

using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using LuckyDice.Utilities;
using Unity.Netcode;
using UnityEngine;

#endregion

namespace LuckyDice.Patches
{
    [HarmonyPatch(typeof(Terminal))]
    public class TerminalPatch
    {
        [HarmonyPatch("Start"), HarmonyPostfix]
        private static void PatchStart(Terminal __instance)
        {
            Utils.Terminal = __instance;
            
            // Enemies
            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
                __instance.StartCoroutine(delayedRegistry(__instance.moonsCatalogueList));
        }

        private static IEnumerator delayedRegistry(SelectableLevel[] catalogue)
        {
            foreach (var level in catalogue)
            {
                foreach (var t in level.Enemies)
                {
                    var enemyPrefab = t.enemyType.enemyPrefab;
                    EnemiesRegistry.RegisterEnemy(enemyPrefab.GetComponent<EnemyAI>().GetType(), enemyPrefab);
                }
            }

            yield break;
        }
    }
}