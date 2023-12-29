#region

using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LethalLib.Modules;
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
            RegisterItems();
            
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
        
        private void RegisterItems()
        {
            Item d20 = ab.LoadAsset<Item>("assets/custom/luckydice/scrap/d20/D20.asset");
            Items.RegisterScrap(d20, 100, Levels.LevelTypes.All);
            Item d4 = ab.LoadAsset<Item>("assets/custom/luckydice/scrap/d4/D4.asset");
            Items.RegisterScrap(d4, 100, Levels.LevelTypes.All);
        }
    }
}