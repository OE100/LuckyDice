using LuckyDice.custom.monobehaviour.def;

namespace LuckyDice.custom.monobehaviour.impl.spawn.Enemies.single
{
    public class SpawnNutcracker : BaseSpawnEnemyEvent
    {
        protected override string Name()
        {
            return Patches.Enemies.Nutcracker;
        }

        protected override int AmountPerStack()
        {
            return 1;
        }
    }
}