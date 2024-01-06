using LuckyDice.custom.monobehaviour.def;

namespace LuckyDice.custom.monobehaviour.impl.spawn.Enemies.single
{
    public class SpawnDressGirl : BaseSpawnEnemyEvent
    {
        protected override string Name()
        {
            return Patches.Enemies.DressGirl;
        }

        protected override int AmountPerStack()
        {
            return 1;
        }
    }
}