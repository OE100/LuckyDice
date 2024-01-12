using System.Collections;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LethalLib.Modules;
using LuckyDice.custom.events;
using LuckyDice.custom.items.dice;
using LuckyDice.custom.monobehaviour.impl.map;
using LuckyDice.custom.monobehaviour.impl.persistent.weathercredits;
using LuckyDice.custom.monobehaviour.impl.player;
using LuckyDice.custom.monobehaviour.impl.spawn.Enemies.all;
using LuckyDice.custom.monobehaviour.impl.spawn.Enemies.single;
using LuckyDice.custom.monobehaviour.impl.spawn.Items.all;
using LuckyDice.custom.monobehaviour.impl.spawn.Items.single;
using LuckyDice.custom.monobehaviour.impl.tweak;
using LuckyDice.custom.network;
using LuckyDice.Patches;
using LuckyDice.Utilities;
using TerminalApi.Classes;
using UnityEngine;

namespace LuckyDice
{
    [BepInPlugin(Guid, Name, Version)]
    [BepInDependency(LethalLib.Plugin.ModGUID, LethalLib.Plugin.ModVersion)]
    [BepInDependency("atomic.terminalapi", "1.5.0")]
    public class Plugin : BaseUnityPlugin
    {
        private readonly Harmony _harmony = new(Guid);
        
        public const string Guid = "oe.tweaks.luckydice";
        public const string Name = "Lucky Dice";
        public const string Version = "0.4.1";

        public static Plugin Instance;

        internal static ManualLogSource Log;

        public static AssetBundle ABundle;

        private void Awake()
        {
            Log = Logger;
            Log.LogInfo($"'{Name}' is loading...");

            if (Instance == null)
                Instance = this;
            
            Log.LogMessage("Trying to load asset bundle");
            ABundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(Assembly.GetExecutingAssembly().GetManifestResourceNames()[0]));
            if (ABundle == null)
            {
                Log.LogError("Failed to load asset bundle");
                return;
            }

            ModConfig.Init(Config);
            
            InitializeNetworkRoutine();
            
            NetworkStuffPatch.networkPrefabs.Add(ABundle.LoadAsset<GameObject>("EventManagerObject.prefab"));
            Utils.DiscoMonkey = RegisterItem("assets/custom/luckydice/scrap/disco_monkey/DiscoMonkey.asset", 1, Levels.LevelTypes.All);
            Utils.D4 = RegisterItem("assets/custom/luckydice/scrap/d4/D4.asset", ModConfig.D4Rarity.Value, Levels.LevelTypes.All);
            Utils.D6 = RegisterItem("assets/custom/luckydice/scrap/d6/D6.asset", ModConfig.D6Rarity.Value, Levels.LevelTypes.All);
            Utils.D8 = RegisterItem("assets/custom/luckydice/scrap/d8/D8.asset", ModConfig.D8Rarity.Value, Levels.LevelTypes.All);
            Utils.D12 = RegisterItem("assets/custom/luckydice/scrap/d12/D12.asset", ModConfig.D12Rarity.Value, Levels.LevelTypes.All);
            Utils.D20 = RegisterItem("assets/custom/luckydice/scrap/d20/D20.asset", ModConfig.D20Rarity.Value, Levels.LevelTypes.All);
            
            _harmony.PatchAll();

            Log.LogInfo($"'{Name}' loaded!");
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

        private static Item RegisterItem(string path, int rarity, Levels.LevelTypes levelTypes)
        {
            Item item = ABundle.LoadAsset<Item>(path);
            if (item == null)
                Log.LogError($"Failed to load item: {path}");
            else
            {
                if (ModConfig.EnableDiceSpawning.Value)
                    Items.RegisterScrap(item, rarity, levelTypes);
                NetworkStuffPatch.networkPrefabs.Add(item.spawnPrefab);
            }

            return item;
        }

        internal static void RegisterCommands()
        {
            if (!ModConfig.RegisterDiceToEventPools.Value) return;
            
            Log.LogDebug("Registering weather commands!");
            var values = Enum.GetValues(typeof(Weather));
            Log.LogDebug($"Found {values.Length} weather types!");
            foreach (Weather t in values)
            {
                var eName = t.ToString();
                Log.LogDebug($"{(int)t}, Registering terminal command: weather {eName.ToLower()}");
                TerminalApi.TerminalApi.AddCommand(eName.ToLower(), new CommandInfo
                    {
                        DisplayTextSupplier = () =>
                        {
                            var credits = NetworkVariableHolder.Instance.WeatherCredits.Value;
                            if (credits <= 0)
                                return $"Not enough credits! ({credits} left, need 1)";

                            if (StartOfRound.Instance.inShipPhase || !StartOfRound.Instance.shipHasLanded)
                            {
                                return "Cannot change weather during ship phase!";
                            }
                            if (StartOfRound.Instance.currentLevelID == 3)
                            {
                                if (WeatherCreditsEvent.TimeUntilNextWarning > 0)
                                {
                                    NetworkVariableHolder.Instance.WeatherCredits.Value -= 1;
                                    EventManager.Instance.SetWeatherServerRPC(t);
                                    FindObjectOfType<StartMatchLever>().EndGame();
                                    return "Now you've done it! You've been fired!";
                                }

                                WeatherCreditsEvent.TimeUntilNextWarning = ModConfig.SecondsPerWarningPeriod.Value;
                                return "No lasers allowed on company moons!\nYou've been warned, next time you'll be fired!";
                            }
                            
                            NetworkVariableHolder.Instance.WeatherCredits.Value -= 1;
                            EventManager.Instance.SetWeatherServerRPC(t);
                            return $"Changed weather to {eName.ToLower()}! ({credits} credits left)";
                        },
                        Category = "Other",
                        Description = $"Sets the weather to {eName.ToLower()}"
                    },
                    "weather");
            }
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
            EventRegistry.RegisterEvent<SpawnClownHorn>(d4Pool);
            EventRegistry.RegisterEvent<SpawnJarOfPickles>(d4Pool);
            
            // register d6 pool
            string d6Pool = EventRegistry.RegisterItem<D6>();
            EventRegistry.RegisterEvent<SpawnCrawler>(d6Pool);
            EventRegistry.RegisterEvent<SpawnHoarderbug>(d6Pool);
            EventRegistry.RegisterEvent<RandomizeLocks>(d6Pool);
            EventRegistry.RegisterEvent<Bleed>(d6Pool);
            EventRegistry.RegisterEvent<SpawnJarOfPickles>(d6Pool);
            EventRegistry.RegisterEvent<SpawnMetalSheet>(d6Pool);
            
            // register d8 pool
            string d8Pool = EventRegistry.RegisterItem<D8>();
            EventRegistry.RegisterEvent<StormyWeatherEvent>(d8Pool);
            EventRegistry.RegisterEvent<TeleportAllPlayers>(d8Pool);
            EventRegistry.RegisterEvent<RandomizeLocks>(d8Pool);
            EventRegistry.RegisterEvent<ExplodeLandmines>(d8Pool);
            EventRegistry.RegisterEvent<TroubleInTerroristTown>(d8Pool);
            EventRegistry.RegisterEvent<SpawnMetalSheet>(d8Pool);
            EventRegistry.RegisterEvent<SpawnMetalSheetForAll>(d8Pool);
            EventRegistry.RegisterEvent<WeatherCreditsEvent>(d8Pool);
            
            // register d12 pool
            string d12Pool = EventRegistry.RegisterItem<D12>();
            EventRegistry.RegisterEvent<MaskedChaos>(d12Pool);
            EventRegistry.RegisterEvent<SpawnDressGirl>(d12Pool);
            EventRegistry.RegisterEvent<SpawnCoilhead>(d12Pool);
            EventRegistry.RegisterEvent<StormyWeatherEvent>(d12Pool);
            EventRegistry.RegisterEvent<SpawnCrawlerForAll>(d12Pool);
            EventRegistry.RegisterEvent<SpawnNutcracker>(d12Pool);
            EventRegistry.RegisterEvent<ExplodeLandmines>(d12Pool);
            EventRegistry.RegisterEvent<TroubleInTerroristTown>(d12Pool);
            EventRegistry.RegisterEvent<SpawnMetalSheet>(d12Pool);
            EventRegistry.RegisterEvent<SpawnGoldBar>(d12Pool);
            EventRegistry.RegisterEvent<SpawnJarOfPicklesForAll>(d12Pool);
            EventRegistry.RegisterEvent<WeatherCreditsEvent>(d12Pool);
            
            // register d20 pool
            string d20Pool = EventRegistry.RegisterItem<D20>();
            EventRegistry.RegisterEvent<MaskedChaos>(d20Pool);
            EventRegistry.RegisterEvent<SpawnDressGirlForAll>(d20Pool);
            EventRegistry.RegisterEvent<SpawnDressGirlForAll>(d20Pool);
            EventRegistry.RegisterEvent<TeleportAllPlayers>(d20Pool);
            
            EventRegistry.RegisterEvent<StormyWeatherEvent>(d20Pool);
            EventRegistry.RegisterEvent<SpawnCoilheadForAll>(d20Pool);
            EventRegistry.RegisterEvent<SpawnCoilheadForAll>(d20Pool);
            EventRegistry.RegisterEvent<ExplodeLandmines>(d20Pool);
            
            EventRegistry.RegisterEvent<SpawnJesterForAll>(d20Pool);
            EventRegistry.RegisterEvent<SpawnJesterForAll>(d20Pool);
            EventRegistry.RegisterEvent<TroubleInTerroristTown>(d20Pool);
            EventRegistry.RegisterEvent<TroubleInTerroristTown>(d20Pool);
            
            EventRegistry.RegisterEvent<SpawnGoldBar>(d20Pool);
            EventRegistry.RegisterEvent<SpawnJarOfPicklesForAll>(d20Pool);
            EventRegistry.RegisterEvent<SpawnJarOfPicklesForAll>(d20Pool);
            EventRegistry.RegisterEvent<WeatherCreditsEvent>(d20Pool);
            
            EventRegistry.RegisterEvent<WeatherCreditsEvent2>(d20Pool);
            EventRegistry.RegisterEvent<SpawnGoldBarForAll>(d20Pool);
            EventRegistry.RegisterEvent<WeatherCreditsEvent3>(d20Pool);
            // todo: register give special disco monkey to everyone
        }
    }
}