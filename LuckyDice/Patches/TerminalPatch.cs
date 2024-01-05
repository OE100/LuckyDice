#region

using System.Collections.Generic;
using HarmonyLib;
using LuckyDice.Utilities;
using UnityEngine;

#endregion

namespace LuckyDice.Patches
{
    [HarmonyPatch(typeof(Terminal))]
    public class TerminalPatch
    {
        internal static Dictionary<string, SpawnableEnemyWithRarity> Enemies = new Dictionary<string, SpawnableEnemyWithRarity>();
        
        [HarmonyPatch("Start"), HarmonyPostfix]
        private static void PatchStart(Terminal __instance)
        {
            // Enemies
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
            
            // Get weather
            Utils.TimeAndWeather = GameObject.Find("Systems/GameSystems/TimeAndWeather");
            // stormy
            Utils.StormyWeatherContainer = Utils.TimeAndWeather.transform.Find("Stormy").gameObject;
            Utils.StormyWeather = Utils.StormyWeatherContainer.GetComponent<StormyWeather>();
            Utils.StormyRainContainer = Utils.TimeAndWeather.transform.Find("StormyRainParticleContainer").gameObject;
            // foggy
            Utils.FoggyWeatherContainer = Utils.TimeAndWeather.transform.Find("Foggy").gameObject;
            // flooding
            Utils.FloodingWeatherContainer = Utils.TimeAndWeather.transform.Find("Flooding").gameObject;
            // eclipse
            Utils.EclipseWeatherContainer = Utils.TimeAndWeather.transform.Find("Eclipse").gameObject;
            // dust storm
            Utils.DustStormWeatherContainer = Utils.TimeAndWeather.transform.Find("DustStorm").gameObject;
            // rainy
            Utils.RainyWeatherContainer = Utils.TimeAndWeather.transform.Find("RainParticleContainer").gameObject;
        }
    }
}