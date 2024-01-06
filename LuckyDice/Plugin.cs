#region

using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LethalLib.Modules;
using LuckyDice.custom.events;
using LuckyDice.custom.items.dice;
using LuckyDice.custom.monobehaviour;
using LuckyDice.custom.monobehaviour.impl.player;
using LuckyDice.custom.monobehaviour.impl.spawn.Enemies;
using LuckyDice.custom.monobehaviour.impl.tweak;
using LuckyDice.custom.network;
using LuckyDice.Patches;
using UnityEngine;

#endregion

namespace LuckyDice
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony(GUID);

        private const string GUID = "oe.tweaks.luckydice";
        private const string NAME = "Lucky Dice";
        private const string VERSION = "1.0.0";

        internal static Plugin Instance;

        internal static ManualLogSource Log;

        internal static AssetBundle ab = null;

        private void Awake()
        {
            Log = this.Logger;
            Log.LogInfo($"'{NAME}' is loading...");

            if (Instance == null)
                Instance = this;
            
            Log.LogMessage("Trying to load asset bundle");
            ab = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(Assembly.GetExecutingAssembly().GetManifestResourceNames()[0]));
            if (ab == null)
            {
                Log.LogError("Failed to load asset bundle");
            }
            
            InitializeNetworkRoutine();
            
            NetworkStuffPatch.networkPrefabs.Add(ab.LoadAsset<GameObject>("EventManagerObject.prefab"));
            RegisterItem("assets/custom/luckydice/scrap/disco_monkey/DiscoMonkey.asset", 1, Levels.LevelTypes.All);
            RegisterItem("assets/custom/luckydice/scrap/d4/D4.asset", 50, Levels.LevelTypes.All);
            RegisterItem("assets/custom/luckydice/scrap/d20/D20.asset", 15, Levels.LevelTypes.All);

            harmony.PatchAll();

            Log.LogInfo($"'{NAME}' loaded!");
        }

        private void InitializeNetworkRoutine()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }
        }

        private Item RegisterItem(string path, int rarity, Levels.LevelTypes levelTypes)
        {
            Item item = ab.LoadAsset<Item>(path);
            if (item == null)
                Log.LogError($"Failed to load item: {path}");
            else
            {
                Items.RegisterScrap(item, rarity, levelTypes);
                NetworkStuffPatch.networkPrefabs.Add(item.spawnPrefab);
            }

            return item;
        }
        
        internal static void RegisterEventsAndItems()
        {
            if (EventManager.Instance == null || EventManager.Instance.gameObject == null)
            {
                Log.LogError("We're fucked!!!");
                return;
            }
            
            // register d4 pool
            string d4Pool = EventRegistry.RegisterItem<D4>();
            EventRegistry.RegisterEvent<SpawnCentipede>(d4Pool);
            EventRegistry.RegisterEvent<Bleed>(d4Pool);
            
            // register d20 pool
            string d20Pool = EventRegistry.RegisterItem<D20>();
            EventRegistry.RegisterEvent<TroubleInTerroristTown>(d20Pool);
        }
    }
}