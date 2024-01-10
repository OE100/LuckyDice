using System.Collections;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace LuckyDice.Utilities
{
    public static class Utils
    {
        // Dice
        public static Item D4;
        public static Item D6;
        public static Item D8;
        public static Item D12;
        public static Item D20;
        
        // Random items
        public static Item DiscoMonkey;
        
        // Outside AI nodes
        public static GameObject[] OutsideAINodes;
        public static GameObject[] InsideAINodes;
        
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
            var random = Random.insideUnitSphere * radius;
            if (!randomHeight)
                random.y = 0;
            var finalPos = random + position;
            Plugin.Log.LogDebug($"Utilities, get position: {finalPos}");
            return finalPos;
        }

        public static bool ClosestNavMeshToPosition(Vector3 position,
            out Vector3 closestPoint,
            float radius = Mathf.Infinity)
        {
            var found = NavMesh.SamplePosition(position, out var hit, radius, NavMesh.AllAreas);
            closestPoint = hit.hit ? hit.position : default;
            Plugin.Log.LogDebug($"Utilities found NavMesh: {found}, position: {closestPoint}");
            return found;
        }

        public static List<PlayerControllerB> GetAllLivingPlayers()
        {
            List<PlayerControllerB> players = [..StartOfRound.Instance.allPlayerScripts];
            players.RemoveAll(player => !player.isPlayerControlled || player.isPlayerDead);
            return players;
        }

        public static PlayerControllerB GetClosestPlayerToPosition(Vector3 position,
            List<PlayerControllerB> players, 
            bool inside = true)
        {
            float distance = Mathf.Infinity;
            PlayerControllerB nearestPlayer = null;
            
            foreach(var player in players)
            {
                var diff = player!.transform.position - position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance >= distance) continue;
                nearestPlayer = player;
                distance = curDistance;
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
            var nearest = Vector3.positiveInfinity;
    
            var aiNodes = inside ? InsideAINodes : OutsideAINodes;
            foreach(var thisObject in aiNodes)
            {
                var diff = thisObject.transform.position - position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance >= distance) continue;
                nearest = thisObject.transform.position;
                distance = curDistance;
            }

            return nearest;
        }
        
        public static EnemyAI SpawnEnemyPrefab(GameObject enemyPrefab, Vector3 position, Quaternion rotation)
        {
            var enemy = Object.Instantiate(enemyPrefab, position, rotation);
            var enemyNetworkObject = enemy.GetComponent<NetworkObject>();
            enemyNetworkObject.Spawn(destroyWithScene:true);
            var enemyAI = enemy.GetComponent<EnemyAI>();
            RoundManager.Instance.SpawnedEnemies.Add(enemyAI);
            return enemyAI;
        }
        
        public static IEnumerator DelayedSetOutside(EnemyAI enemyAI, bool isOutside)
        {
            while (enemyAI.isOutside != isOutside)
            {
                enemyAI.isOutside = isOutside;
                enemyAI.allAINodes = isOutside ? OutsideAINodes : InsideAINodes;
                enemyAI.SyncPositionToClients();
                yield return new WaitForSeconds(0.5f);
            }
        }

        public static IEnumerator DelayedDestroy(Func<bool> predicate, GameObject gameObject)
        {
            yield return new WaitUntil(predicate);
            Object.Destroy(gameObject);
        }
    }
}