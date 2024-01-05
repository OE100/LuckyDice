using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace LuckyDice.Utilities
{
    public static class Utils
    {
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
            Vector3 origin,
            float radius = 35,
            bool randomHeight = false
            )
        {
            Vector3 random = Random.insideUnitSphere * radius;
            if (!randomHeight)
                random.y = 0;
            Vector3 finalPos = random + origin;
            Plugin.Log.LogDebug($"Utilities, get position: {finalPos}");
            return finalPos;
        }

        public static bool ReturnClosestNavMeshPoint(Vector3 origin,
            out Vector3 closestPoint,
            float radius = Mathf.Infinity)
        {
            bool found = NavMesh.SamplePosition(origin, out NavMeshHit hit, radius, NavMesh.AllAreas);
            if (hit.hit)
                closestPoint = hit.position;
            else
                closestPoint = default;
            Plugin.Log.LogDebug($"Utilities found NavMesh: {found}, position: {closestPoint}");
            return found;
        }
    }
}