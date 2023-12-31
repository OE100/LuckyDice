#region

using System.Collections.Generic;
using HarmonyLib;
using LuckyDice.custom.events.implementation;
using LuckyDice.custom.events.implementation.map;
using UnityEngine;

#endregion

namespace LuckyDice.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    public class StartOfRoundPatch
    {
        private static Terminal terminal = null;
        
        [HarmonyPatch("ChangeLevel"), HarmonyPrefix]
        private static void PatchChangeLevel(StartOfRound __instance, int levelID)
        {
            if (terminal == null)
            {
                terminal = Object.FindObjectOfType<Terminal>();
            }

            List<SpawnableEnemyWithRarity> enemyWithRarities = terminal.moonsCatalogueList[levelID].Enemies;
            foreach (string enemy in Enemies.EnemiesList)
            {
                if (!enemyWithRarities.Exists(er => er.enemyType.name == enemy))
                {
                    enemyWithRarities.Add(TerminalPatch.Enemies[enemy]);
                }
            }
        }

        [HarmonyPatch("ShipLeave"), HarmonyPostfix]
        private static void PatchShipLeave()
        {
            RandomizeLocks.doors.Clear();
        }
    }
}