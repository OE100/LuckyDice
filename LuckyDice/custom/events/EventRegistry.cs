using System;
using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
using LuckyDice.custom.monobehaviour.attributes;
using LuckyDice.custom.monobehaviour.def;
using LuckyDice.custom.network;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace LuckyDice.custom.events
{
    public static class EventRegistry
    {
        private static Dictionary<Type, string> _itemToPool = new();
        private static Dictionary<string, List<(Type, GameObject)>> _eventPools = new();
        private static Dictionary<string, List<(Type, GameObject)>> _removedEventPools = new();
        private static Dictionary<GameObject, Type> _mountedEvents = new();
        private static List<Type> _removedOneTimeEvents = new();

        public static string UnRegisterItem<TItem>() where TItem : GrabbableObject
        {
            if (!_itemToPool.TryGetValue(typeof(TItem), out var pool))
            {
                Plugin.Log.LogDebug($"Item {typeof(TItem).Name} wasn't even registered");
                return null;
            }
            _itemToPool.Remove(typeof(TItem));
            Plugin.Log.LogDebug($"Item {typeof(TItem).Name} was unregistered from pool {pool}");
            return pool;
        }
        
        public static string RegisterItem<TItem>(string pool = null) where TItem : GrabbableObject
        {
            pool ??= typeof(TItem).Name;
            Plugin.Log.LogDebug($"Associating item: {typeof(TItem).Name}, with pool: {pool}");
            _itemToPool[typeof(TItem)] = pool;
            return pool;
        }
        
        public static string GetPoolFromItem(Type item)
        {
            Plugin.Log.LogDebug($"Getting pool for item: {item.Name}");
            if (!_itemToPool.ContainsKey(item))
            {
                Plugin.Log.LogError($"Item: {item.Name}, does not have a pool!");
                return null;
            }

            string poolFromItem = _itemToPool[item];
            Plugin.Log.LogDebug($"Pool for item: {item.Name}, is: {poolFromItem}");
            return poolFromItem;
        }

        public static void RegisterEvent<TEvent>(string pool) where TEvent : BaseEventBehaviour
        {
            RegisterEvent<TEvent>(pool: pool, mountingPoint: EventManager.Instance.gameObject);
        }
        
        public static void RegisterEvent<TEvent>(string pool, GameObject mountingPoint) where TEvent : BaseEventBehaviour
        {
            Plugin.Log.LogDebug($"Registering event: {typeof(TEvent).Name}, to pool: {pool}");
            // check for inheritance requirement
            if (!typeof(BaseEventBehaviour).IsAssignableFrom(typeof(TEvent)))
            {
                Plugin.Log.LogError($"Event: {typeof(TEvent).Name}, does not inherit from BaseEventBehaviour!");
                return;
            }
            
            if (!_eventPools.ContainsKey(pool))
            {
                Plugin.Log.LogDebug($"Creating new event pool: {pool}");
                _eventPools.Add(pool, []);
                _removedEventPools.Add(pool, []);
            }
            Plugin.Log.LogDebug($"Adding event: {typeof(TEvent).Name}, to pool: {pool}");
            _eventPools[pool].Add((typeof(TEvent), mountingPoint));
            
            if (Attribute.GetCustomAttribute(typeof(TEvent), typeof(MountAtRegistry)) != null)
            {
                Plugin.Log.LogDebug($"Event: {typeof(TEvent).Name}, is a mount at registry event, mounting now");
                MountEvent(mountingPoint, typeof(TEvent));
            }
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
            
            if (!_eventPools.ContainsKey(pool))
            {
                Plugin.Log.LogDebug($"Creating new event pool: {pool}");
                _eventPools.Add(pool, []);
                _removedEventPools.Add(pool, []);
            }
            Plugin.Log.LogDebug($"Adding event: {eventMonoBehaviourType.Name}, to pool: {pool}");
            _eventPools[pool].Add((eventMonoBehaviourType, mountingPoint));
            
            if (Attribute.GetCustomAttribute(eventMonoBehaviourType, typeof(MountAtRegistry)) != null)
            {
                Plugin.Log.LogDebug($"Event: {eventMonoBehaviourType.Name}, is a mount at registry event, mounting now");
                MountEvent(mountingPoint, eventMonoBehaviourType);
            }
        }
        
        public static List<(Type, GameObject)> GetEventPool(string pool)
        {
            if (!_eventPools.ContainsKey(pool))
            {
                Plugin.Log.LogError($"Event pool: {pool}, does not exist");
                return [];
            }
            return _eventPools[pool];
        }

        // restore events from used lists to pools
        private static void RestoreEventPools()
        {
            Plugin.Log.LogDebug("Restoring event pools");
            if (_removedEventPools.Count > 0)
            {
                foreach (var (key, list) in _removedEventPools)
                {
                    Plugin.Log.LogDebug($"Restoring event pool: {key}");
                    if (list.Count > 0)
                    {
                        list.ForEach(tuple => RegisterEvent(key, tuple.Item1, tuple.Item2));
                        list.Clear();
                    }
                }
            }
        }

        // remove event from pool and add it to used list
        public static int GetRandomEventIndexFromPool(string pool)
        {
            if (!_eventPools.ContainsKey(pool))
            {
                Plugin.Log.LogError($"Event pool: {pool}, does not exist");
                return -1;
            }
            List<(Type, GameObject)> types = _eventPools[pool];
            bool found = false;
            int index = -1;
            while (!found && types.Count > 0)
            {
                index = Random.Range(0, types.Count);
                (Type, GameObject) tuple = types[index];
                if (Attribute.GetCustomAttribute(tuple.Item1, typeof(OneTimeEvent)) != null &&
                    _removedOneTimeEvents.Contains(tuple.Item1))
                {
                    _removedEventPools[pool].Add(tuple);
                    types.RemoveAt(index);
                }
                else
                    found = true;
            }

            if (types.Count == 0)
                return -1;
            return index;
        }

        public static void RunEventFromPool(string pool, int eventIndex, PlayerControllerB player)
        {
            if (!_eventPools.ContainsKey(pool))
            {
                Plugin.Log.LogError($"Event pool: {pool}, does not exist");
                return;
            }
            List<(Type, GameObject)> tuples = _eventPools[pool];
            if (eventIndex < 0 || eventIndex >= tuples.Count)
            {
                Plugin.Log.LogError($"Event index: {eventIndex}, is out of range for pool: {pool}");
                return;
            }
            (Type, GameObject) eEvent = tuples[eventIndex];
            // check if the event is a mount at registry event and if skip mounting it
            if (Attribute.GetCustomAttribute(eEvent.Item1, typeof(MountAtRegistry)) != null)
            {
                // check if the event is a one time event and if so remove it from the pool
                BaseEventBehaviour component = (BaseEventBehaviour)eEvent.Item2.GetComponent(eEvent.Item1);
                if (Attribute.GetCustomAttribute(eEvent.Item1, typeof(OneTimeEvent)) != null)
                {
                    Plugin.Log.LogDebug($"Event: {eEvent.Item1.Name}, is one time event, removing from pool: {pool}");
                    tuples.RemoveAt(eventIndex);
                    _removedEventPools[pool].Add(eEvent);
                }
                if (typeof(BasePlayerEvent).IsAssignableFrom(eEvent.Item1))
                {
                    Plugin.Log.LogDebug($"Event: {eEvent.Item1.Name}, is a player event, adding player: {player.playerUsername}");
                    ((BasePlayerEvent)component).AddPlayer(player);
                }
            }
            else if (MountEvent(eEvent.Item2, eEvent.Item1))
            {
                // check if the event is a one time event and if so remove it from the pool
                Plugin.Log.LogDebug($"Event: {eEvent.Item1.Name}, is one time event, removing from pool: {pool}");
                tuples.RemoveAt(eventIndex);
                _removedEventPools[pool].Add(eEvent);
            }
        }
        
        // mount an event and if requires deletion add to the mounted events list
        // returns true if event is one time
        public static bool MountEvent(GameObject gameObject, Type eventType)
        {
            // check if event already is mounted and if so return false so it won't try to clean it up later
            Component component = gameObject.GetComponent(eventType);
            if (component != null)
                return false;
            
            // if it isn't mount it and check if it needs removal
            component = gameObject.AddComponent(eventType);

            if (component == null)
            {
                Plugin.Log.LogDebug($"Failed to mount event: {eventType.Name}, to: {gameObject.name}, this shouldn't happen!");
                return false;
            }
            
            if (Attribute.GetCustomAttribute(eventType, typeof(NeedsRemoval)) != null)
                _mountedEvents.Add(gameObject, eventType);
            
            return Attribute.GetCustomAttribute(eventType, typeof(OneTimeEvent)) != null;
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
            if (_mountedEvents.Count > 0)
                foreach (KeyValuePair<GameObject,Type> pair in _mountedEvents)
                    UnMountEvent(pair.Key, pair.Value);
            
            _removedOneTimeEvents.Clear();
            RestoreEventPools();
        }
    }
}