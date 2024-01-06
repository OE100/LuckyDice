using LuckyDice.custom.monobehaviour.def;

namespace LuckyDice.custom.monobehaviour.impl.spawn.Enemies.all
{
    public class SpawnJesterForAll : BaseSpawnEnemyForAll
    {
        protected override string Name()
        {
            return Patches.Enemies.Jester;
        }

        protected override int AmountPerStack()
        {
            return 1;
        }
    }
}