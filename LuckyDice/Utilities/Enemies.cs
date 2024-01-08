#region

using System.Collections.Generic;

#endregion

namespace LuckyDice.Utilities
{
    public static class Enemies
    {
        public static readonly string Centipede = "Centipede";
        public static readonly string SandSpider = "SandSpider";
        public static readonly string HoarderBug = "HoarderBug";
        public static readonly string Flowerman = "Flowerman";
        public static readonly string Crawler = "Crawler";
        public static readonly string Blob = "Blob";
        public static readonly string DressGirl = "DressGirl";
        public static readonly string Puffer = "Puffer";
        public static readonly string Nutcracker = "Nutcracker";
        public static readonly string SpringMan = "SpringMan";
        public static readonly string Jester = "Jester";
        public static readonly string MaskedPlayerEnemy = "MaskedPlayerEnemy";
        
        public static readonly List<string> EnemiesList = new List<string>()
        {
            Centipede,
            SandSpider,
            HoarderBug,
            Flowerman,
            Crawler,
            Blob,
            DressGirl,
            Puffer,
            Nutcracker,
            SpringMan,
            Jester,
            MaskedPlayerEnemy
        };
    }
}