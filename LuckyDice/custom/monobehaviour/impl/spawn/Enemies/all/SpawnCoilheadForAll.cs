using LuckyDice.custom.monobehaviour.def;

namespace LuckyDice.custom.monobehaviour.impl.spawn.Enemies.all
{
    public class SpawnCoilheadForAll : BaseSpawnEnemyForAll
    {
        protected override string Name()
        {
            return Patches.Enemies.SpringMan;
        }

        protected override int AmountPerStack()
        {
            return 1;
        }
    }
}