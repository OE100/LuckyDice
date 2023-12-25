using System.Collections.Generic;
using HarmonyLib;
using LuckyDice.custom.events;

namespace LuckyDice.Patches
{
    [HarmonyPatch(typeof(Terminal))]
    public class TerminalPatch
    {
        internal static Dictionary<string, SpawnableEnemyWithRarity> Enemies = new Dictionary<string, SpawnableEnemyWithRarity>();
        
        [HarmonyPatch("Start"), HarmonyPostfix]
        private static void PatchStart(Terminal __instance)
        {
            Plugin.Log.LogMessage("Printing all enemies in each moon:");
            for (int i = 0; i < __instance.moonsCatalogueList.Length; i++)
            {
                SelectableLevel level = __instance.moonsCatalogueList[i];
                for (int j = 0; j < level.Enemies.Count; j++)
                {
                    SpawnableEnemyWithRarity spawnableEnemyWithRarity = level.Enemies[j];
                    SpawnableEnemyWithRarity clean = new SpawnableEnemyWithRarity();
                    clean.rarity = 0;
                    clean.enemyType = spawnableEnemyWithRarity.enemyType;
                    if (!Enemies.ContainsKey(spawnableEnemyWithRarity.enemyType.name))
                    {
                        Enemies.Add(spawnableEnemyWithRarity.enemyType.name, clean);
                    }
                }
            }
        }
    }
}