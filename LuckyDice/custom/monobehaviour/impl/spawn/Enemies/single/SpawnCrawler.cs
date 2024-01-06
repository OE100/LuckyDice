using LuckyDice.custom.monobehaviour.def;

namespace LuckyDice.custom.monobehaviour.impl.spawn.Enemies.single
{
    public class SpawnCrawler : BaseSpawnEnemyEvent
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