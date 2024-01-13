#region

using System.Collections.Generic;
using HarmonyLib;
using LuckyDice.custom.events;
using LuckyDice.custom.monobehaviour.impl;
using LuckyDice.custom.monobehaviour.impl.map;
using LuckyDice.Utilities;
using UnityEngine;

#endregion

namespace LuckyDice.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    public class StartOfRoundPatch
    {
        private static bool _registeredCommands;
        
        [HarmonyPatch(nameof(StartOfRound.ShipLeave)), HarmonyPostfix]
        private static void PatchShipLeave()
        {
            EventRegistry.EndOfRoundCleanup();
            
            RandomizeLocks.Doors.Clear();
        }
        
        [HarmonyPatch(nameof(StartOfRound.OnShipLandedMiscEvents)), HarmonyPostfix]
        private static void PatchOnShipLandedMiscEvents()
        {
            // Register commands if not registered
            if (!_registeredCommands)
            {
                _registeredCommands = true;
                Plugin.RegisterCommands();
            }
            
            // Get AI nodes
            Utils.OutsideAINodes = GameObject.FindGameObjectsWithTag("OutsideAINode");
            Utils.InsideAINodes = GameObject.FindGameObjectsWithTag("AINode");
            
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