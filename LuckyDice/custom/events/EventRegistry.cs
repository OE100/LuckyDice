using System;
using System.Collections.Generic;
using System.Linq;
using LuckyDice.custom.monobehaviour.def;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace LuckyDice.custom.events
{
    public static class EventRegistry
    {
        private static Dictionary<Type, string> itemToPool = new Dictionary<Type, string>();
        private static Dictionary<string, List<(Type, GameObject)>> eventPools = new Dictionary<string, List<(Type, GameObject)>>();
        private static Dictionary<string, List<(Type, GameObject)>> removedEventPools = new Dictionary<string, List<(Type, GameObject)>>();
        private static Dictionary<GameObject, Type> mountedEvents = new Dictionary<GameObject, Type>();

        public static void RegisterItem(Type item, string pool)
        {
            Plugin.Log.LogDebug($"Associating item: {item.Name}, with pool: {pool}");
            itemToPool[item] = pool;
        }
        
        public static string GetPoolFromItem(Type item)
        {
            Plugin.Log.LogDebug($"Getting pool for item: {item.Name}");
            if (!itemToPool.ContainsKey(item))
            {
                Plugin.Log.LogError($"Item: {item.Name}, does not have a pool!");
                return null;
            }

            string poolFromItem = itemToPool[item];
            Plugin.Log.LogDebug($"Pool for item: {item.Name}, is: {poolFromItem}");
            return poolFromItem;
        }
        
        public static void RegisterEvent(string pool, Type eventMonoBehaviourType, GameObject mountingPoint)
        {
            Plugin.Log.LogDebug($"Registering event: {eventMonoBehaviourType.Name}, to pool: {pool}");
            // check for inheritance requirement
            if (!typeof(BaseEventBehaviour).IsAssignableFrom(eventMonoBehaviourType))
            {
                Plugin.Log.LogError($"Event: {eventMonoBehaviourType.Name}, does not inherit from BaseEventBehaviour!");
                return;
            }
            
            if (!eventPools.ContainsKey(pool))
            {
                Plugin.Log.LogDebug($"Creating new event pool: {pool}");
                eventPools.Add(pool, new List<(Type, GameObject)>());
                removedEventPools.Add(pool, new List<(Type, GameObject)>());
            }
            Plugin.Log.LogDebug($"Adding event: {eventMonoBehaviourType.Name}, to pool: {pool}");
            eventPools[pool].Add((eventMonoBehaviourType, mountingPoint));
        }
        
        public static List<(Type, GameObject)> GetEventPool(string pool)
        {
            if (!eventPools.ContainsKey(pool))
            {
                Plugin.Log.LogError($"Event pool: {pool}, does not exist");
                return new List<(Type, GameObject)>();
            }
            return eventPools[pool];
        }

        // restore events from used lists to pools
        private static void RestoreEventPools()
        {
            Plugin.Log.LogDebug("Restoring event pools");
            if (removedEventPools.Count > 0)
            {
                foreach (KeyValuePair<string, List<(Type, GameObject)>> pool in removedEventPools)
                {
                    Plugin.Log.LogDebug($"Restoring event pool: {pool.Key}");
                    List<(Type, GameObject)> tuples = pool.Value;
                    if (tuples.Count > 0)
                    {
                        tuples.ForEach(tuple => RegisterEvent(pool.Key, tuple.Item1, tuple.Item2));
                        tuples.Clear();
                    }
                }
            }
        }

        // remove event from pool and add it to used list
        public static int GetRandomEventIndexFromPool(string pool)
        {
            if (!eventPools.ContainsKey(pool))
            {
                Plugin.Log.LogError($"Event pool: {pool}, does not exist");
                return -1;
            }
            List<(Type, GameObject)> types = eventPools[pool];
            return Random.Range(0, types.Count);
        }

        public static void RunEventFromPool(string pool, int eventIndex)
        {
            if (!eventPools.ContainsKey(pool))
            {
                Plugin.Log.LogError($"Event pool: {pool}, does not exist");
                return;
            }
            List<(Type, GameObject)> tuples = eventPools[pool];
            if (eventIndex < 0 || eventIndex >= tuples.Count)
            {
                Plugin.Log.LogError($"Event index: {eventIndex}, is out of range for pool: {pool}");
                return;
            }
            (Type, GameObject) eEvent = tuples[eventIndex];
            if (MountEvent(eEvent.Item2, eEvent.Item1))
            {
                Plugin.Log.LogDebug($"Event: {eEvent.Item1.Name}, is one time event, removing from pool: {pool}");
                tuples.RemoveAt(eventIndex);
                removedEventPools[pool].Add(eEvent);
            }
        }
        
        // mount an event and if requires deletion add to the mounted events list
        // returns true if event is one time
        public static bool MountEvent(GameObject gameObject, Type eventType)
        {
            gameObject.AddComponent(eventType);
            Component[] components = gameObject.GetComponents(typeof(BaseEventBehaviour));
            Component found;
            try
            {
                found = components.First(component => component.GetType() == eventType);
            }
            catch (InvalidOperationException)
            {
                return true;
            }

            BaseEventBehaviour baseEventBehaviour = ((BaseEventBehaviour)found);
            if (baseEventBehaviour.NeedsRemoval)
                mountedEvents.Add(gameObject, eventType);
            
            return baseEventBehaviour.IsOneTimeEvent;
        }

        
        // remove mounted active event by destroying it
        private static void UnMountEvent(GameObject gameObject, Type eventType)
        {
            Component[] components = gameObject.GetComponents(typeof(BaseEventBehaviour));
            Component found;
            try
            {
                found = components.First(component => component.GetType() == eventType);
            }
            catch (InvalidOperationException)
            {
                return;
            }
            Object.Destroy(found);
        }

        public static void EndOfRoundCleanup()
        {
            if (mountedEvents.Count > 0)
                foreach (KeyValuePair<GameObject,Type> pair in mountedEvents)
                    UnMountEvent(pair.Key, pair.Value);
            
            RestoreEventPools();
        }
    }
}