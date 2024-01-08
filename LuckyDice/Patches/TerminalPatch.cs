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
            // Enemies
            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
                __instance.StartCoroutine(delayedRegistry(__instance.moonsCatalogueList));
            
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

        private static IEnumerator delayedRegistry(SelectableLevel[] catalogue)
        {
            for (int i = 0; i < catalogue.Length; i++)
            {
                SelectableLevel level = catalogue[i];
                for (int j = 0; j < level.Enemies.Count; j++)
                {
                    GameObject enemyPrefab = level.Enemies[j].enemyType.enemyPrefab;
                    EnemiesRegistry.RegisterEnemy(enemyPrefab.GetComponent<EnemyAI>().GetType(), enemyPrefab);
                }
            }
            yield break;
        }
    }
}