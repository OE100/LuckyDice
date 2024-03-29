﻿using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LuckyDice.Patches
{
    [HarmonyPatch]
    public class MaskedEnemyChanges
    {
        public static bool Triggered = false;
        
        [HarmonyPatch(typeof(EnemyAI), "CheckLineOfSightForClosestPlayer"), HarmonyPrefix]
        private static bool PatchCheckLineOfSightForClosestPlayerPrefix(
            EnemyAI __instance,
            float width,
            int range,
            int proximityAwareness,
            float bufferDistance,
            ref PlayerControllerB __result)
        {
            if (!Triggered)
                return true;
            if (__instance is not MaskedPlayerEnemy)
                return true;
            
            if (__instance.isOutside && !__instance.enemyType.canSeeThroughFog && TimeOfDay.Instance.currentLevelWeather == LevelWeatherType.Foggy)
                range = Mathf.Clamp(range, 0, 30);
            float optimalDistance = 1000f;
            int chosenPlayerIndex = -1;
            PlayerControllerB[] allPlayerScripts = StartOfRound.Instance.allPlayerScripts;
            for (int currentPlayerIndex = 0; currentPlayerIndex < allPlayerScripts.Length; ++currentPlayerIndex)
            {
                // check if player has 2 masks in inventory
                if (PlayerHas2Masks(allPlayerScripts[currentPlayerIndex]))
                    continue;
                
                Vector3 position = allPlayerScripts[currentPlayerIndex].gameplayCamera.transform.position;
                if (!Physics.Linecast(__instance.eye.position, position, StartOfRound.Instance.collidersAndRoomMaskAndDefault))
                {
                    Vector3 to = position - __instance.eye.position;
                    float distance = Vector3.Distance(__instance.eye.position, position);
                    if ((Vector3.Angle(__instance.eye.forward, to) < width || proximityAwareness != -1 && distance < proximityAwareness) && distance < optimalDistance)
                    {
                        optimalDistance = distance;
                        chosenPlayerIndex = currentPlayerIndex;
                    }
                }
            }

            if (__instance.targetPlayer != null && chosenPlayerIndex != -1 &&
                __instance.targetPlayer != StartOfRound.Instance.allPlayerScripts[chosenPlayerIndex] &&
                bufferDistance > 0.0 && Mathf.Abs(optimalDistance - Vector3.Distance(__instance.transform.position,
                    __instance.targetPlayer.transform.position)) < bufferDistance)
            {
                __result = null!;
                return false;
            }
            if (chosenPlayerIndex < 0)
            {
                __result = null!;
                return false;
            }
            __instance.mostOptimalDistance = optimalDistance;
            __result = StartOfRound.Instance.allPlayerScripts[chosenPlayerIndex];
            return false;
        }

        [HarmonyPatch(typeof(MaskedPlayerEnemy), "OnCollideWithPlayer"), HarmonyPrefix]
        private static bool PatchOnCollideWithPlayerPrefix(MaskedPlayerEnemy __instance, Collider other)
        {
            PlayerControllerB player;
            return !Triggered ||
                   (player = other.GetComponentInParent<PlayerControllerB>()) == null ||
                   !PlayerHas2Masks(player);
        }
        
        private static bool PlayerHas2Masks(PlayerControllerB player)
        {
            var maskCount = player.ItemSlots.OfType<HauntedMaskItem>().Count();
            return maskCount >= 2;
        } 
    }
}