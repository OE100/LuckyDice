using BepInEx.Configuration;

namespace LuckyDice
{
    internal static class ModConfig
    {
        // General settings
        public static ConfigEntry<bool> EnableDiceSpawning;
        public static ConfigEntry<bool> RegisterDiceToEventPools;
        
        // Items
        public static ConfigEntry<int> D4Rarity;
        public static ConfigEntry<int> D6Rarity;
        public static ConfigEntry<int> D8Rarity;
        public static ConfigEntry<int> D12Rarity;
        public static ConfigEntry<int> D20Rarity;
        
        // Weather
        public static ConfigEntry<bool> EnableWeatherChangingCommands;
        public static ConfigEntry<float> SecondsPerWarningPeriod;
        
        // TTT
        public static ConfigEntry<float> TTTTerroristChance;
        public static ConfigEntry<int> TTTMinTimeToBlow;
        public static ConfigEntry<int> TTTMaxTimeToBlow;
        
        internal static void Init(ConfigFile config)
        {
            // General settings
            EnableDiceSpawning = config.Bind("General", "EnableDiceSpawning", true, 
                "Enable spawning of dice in the world.");
            RegisterDiceToEventPools = config.Bind("General", "RegisterDiceToEventPools", true, 
                "Register dice to event pools. (if false dice will act like regular scrap)");
            
            // Items
            D4Rarity = config.Bind("Items", "D4Rarity", 25, 
                "Rarity of the D4 item. (0 = Can't spawn, 100 = Extremely common");
            D6Rarity = config.Bind("Items", "D6Rarity", 20, 
                "Rarity of the D6 item. (0 = Can't spawn, 100 = Extremely common");
            D8Rarity = config.Bind("Items", "D8Rarity", 15, 
                "Rarity of the D8 item. (0 = Can't spawn, 100 = Extremely common");
            D12Rarity = config.Bind("Items", "D12Rarity", 10, 
                "Rarity of the D12 item. (0 = Can't spawn, 100 = Extremely common");
            D20Rarity = config.Bind("Items", "D20Rarity", 5, 
                "Rarity of the D20 item. (0 = Can't spawn, 100 = Extremely common");
            
            // Commands
            EnableWeatherChangingCommands = config.Bind("Commands", "EnableWeatherChangingCommands", true, 
                "Enable weather changing commands.");
            SecondsPerWarningPeriod = config.Bind("Commands", "SecondsPerWarningPeriod", 10f, 
                "Seconds per warning period.");
            
            // TTT
            TTTTerroristChance = config.Bind("TTT", "TTTTerroristChance", 0.5f,
                "Chance to become a terrorist in the TTT event.");
            TTTMinTimeToBlow = config.Bind("TTT", "TTTMinTimeToBlow", 60, 
                "Minimum time to blow up in the TTT event.");
            TTTMaxTimeToBlow = config.Bind("TTT", "TTTMaxTimeToBlow", 120,
                "Maximum time to blow up in the TTT event.");
        }
    }
}