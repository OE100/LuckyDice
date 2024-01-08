#region

using System.Collections.Generic;
using HarmonyLib;
using LuckyDice.custom.events;
using LuckyDice.custom.monobehaviour.impl;
using LuckyDice.custom.monobehaviour.impl.map;
using UnityEngine;

#endregion

namespace LuckyDice.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    public class StartOfRoundPatch
    {
        [HarmonyPatch(nameof(StartOfRound.ShipLeave)), HarmonyPostfix]
        private static void PatchShipLeave()
        {
            EventRegistry.EndOfRoundCleanup();
            
            RandomizeLocks.doors.Clear();
        }
    }
}