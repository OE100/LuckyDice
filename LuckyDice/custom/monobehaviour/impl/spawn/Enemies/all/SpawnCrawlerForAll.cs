using LuckyDice.custom.monobehaviour.def;

namespace LuckyDice.custom.monobehaviour.impl.spawn.Enemies.all
{
    public class SpawnCrawlerForAll : BaseSpawnEnemyForAll
    {
        protected override string Name()
        {
            return Patches.Enemies.Crawler;
        }

        protected override int AmountPerStack()
        {
            return 1;
        }
    }
}