using System.Collections.Generic;
using GameNetcodeStuff;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace LuckyDice.Utilities
{
    public static class Utils
    {
        // Dice
        public static Item D4 = null;
        public static Item D6 = null;
        public static Item D8 = null;
        public static Item D12 = null;
        public static Item D20 = null;
        
        // Random items
        public static Item DiscoMonkey = null;
        
        // Outside AI nodes
        public static GameObject[] OutsideAINodes = null;
        public static GameObject[] InsideAINodes = null;
        
        // weather container
        public static GameObject TimeAndWeather;
        
        // stormy
        public static GameObject StormyWeatherContainer;
        public static StormyWeather StormyWeather;
        public static GameObject StormyRainContainer;
        
        // foggy
        public static GameObject FoggyWeatherContainer;
        
        // flooding
        public static GameObject FloodingWeatherContainer;
        
        // eclipse
        public static GameObject EclipseWeatherContainer;
        
        // dust storm
        public static GameObject DustStormWeatherContainer;
        
        // rainy
        public static GameObject RainyWeatherContainer;
        
        
        
        public static Vector3 GetRandomLocationAroundPosition(
            Vector3 position,
            float radius = 35,
            bool randomHeight = false
            )
        {
            Vector3 random = Random.insideUnitSphere * radius;
            if (!randomHeight)
                random.y = 0;
            Vector3 finalPos = random + position;
            Plugin.Log.LogDebug($"Utilities, get position: {finalPos}");
            return finalPos;
        }

        public static bool ClosestNavMeshToPosition(Vector3 position,
            out Vector3 closestPoint,
            float radius = Mathf.Infinity)
        {
            bool found = NavMesh.SamplePosition(position, out NavMeshHit hit, radius, NavMesh.AllAreas);
            if (hit.hit)
                closestPoint = hit.position;
            else
                closestPoint = default;
            Plugin.Log.LogDebug($"Utilities found NavMesh: {found}, position: {closestPoint}");
            return found;
        }

        public static List<PlayerControllerB> GetAllLivingPlayers()
        {
            List<PlayerControllerB> players = new List<PlayerControllerB>(StartOfRound.Instance.allPlayerScripts);
            players.RemoveAll(player => !player.isPlayerControlled || player.isPlayerDead);
            return players;
        }

        public static PlayerControllerB GetClosestPlayerToPosition(Vector3 position,
            List<PlayerControllerB> players, 
            bool inside = true)
        {
            float distance = Mathf.Infinity;
            PlayerControllerB nearestPlayer = null;
            
            foreach(PlayerControllerB player in players)
            {
                Vector3 diff = player.transform.position - position;
                float curDistance = diff.sqrMagnitude; 
                if(curDistance < distance)
                {
                    nearestPlayer = player;
                    distance = curDistance;
                }
            }

            return nearestPlayer;
        }
        
        public static Vector3 GetClosestAINodeToPosition(Vector3 position, bool inside = false)
        {
            if (inside ? InsideAINodes == null : OutsideAINodes == null)
            {
                Plugin.Log.LogError($"{(inside ? "Inside" : "Outside")} AI nodes not found!");
                return Vector3.positiveInfinity;
            }
            
            float distance = Mathf.Infinity;
            Vector3 nearest = Vector3.positiveInfinity;
    
            GameObject[] aiNodes = inside ? InsideAINodes : OutsideAINodes;
            foreach(GameObject thisObject in aiNodes)
            {
                Vector3 diff = thisObject.transform.position - position;
                float curDistance = diff.sqrMagnitude; 
                if(curDistance < distance)
                {
                    nearest = thisObject.transform.position;
                    distance = curDistance;
                }
            }

            return nearest;
        }
    }
}