using System;
using System.Collections.Generic;
using HarmonyLib;

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
                terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
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
    }
}