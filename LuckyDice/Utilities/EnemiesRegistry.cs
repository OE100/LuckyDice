using UnityEngine;

namespace LuckyDice.Utilities
{
    public static class EnemiesRegistry
    {
        internal static Dictionary<Type, GameObject> TypeToPrefab = new Dictionary<Type, GameObject>();

        public static bool RegisterEnemy<TEnemy>(GameObject enemyPrefab) where TEnemy : EnemyAI
        {
            return RegisterEnemy(typeof(TEnemy), enemyPrefab);
        }

        public static bool RegisterEnemy(Type enemyType, GameObject enemyPrefab)
        {
            if (TypeToPrefab.ContainsKey(enemyType))
            {
                Plugin.Log.LogDebug($"Enemy prefab for {enemyType.Name} already registered");
                return false;
            }
            
            TypeToPrefab.Add(enemyType, enemyPrefab);
            Plugin.Log.LogDebug($"Registered enemy prefab for: {enemyType.Name}");
            return true;
        }
        
        public static GameObject GetEnemyPrefab<TEnemy>() where TEnemy : EnemyAI
        {
            return GetEnemyPrefab(typeof(TEnemy));
        }
        
        public static GameObject GetEnemyPrefab(Type enemyType)
        {
            return TypeToPrefab[enemyType];
        }
        
        public static bool UnRegisterEnemy<TEnemy>() where TEnemy : EnemyAI
        {
            return UnRegisterEnemy(typeof(TEnemy));
        }
        
        public static bool UnRegisterEnemy(Type enemyType)
        {
            return TypeToPrefab.Remove(enemyType);
        }
    }
}