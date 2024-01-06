using LuckyDice.custom.monobehaviour.def;

namespace LuckyDice.custom.monobehaviour.impl.spawn.Enemies.all
{
    public class SpawnDressGirlForAll : BaseSpawnEnemyForAll
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