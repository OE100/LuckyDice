using LuckyDice.custom.monobehaviour.def;

namespace LuckyDice.custom.monobehaviour.impl.spawn.Enemies
{
    public class SpawnHoarderbug : BaseSpawnEnemyEvent
    {
        protected override string Name()
        {
            return Patches.Enemies.HoarderBug;
        }

        protected override int AmountPerStack()
        {
            return 3;
        }
    }
}