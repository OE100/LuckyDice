using LuckyDice.custom.monobehaviour.def;

namespace LuckyDice.custom.monobehaviour.impl.spawn.Enemies.single
{
    public class SpawnFlowerman : BaseSpawnEnemyEvent
    {
        protected override string Name()
        {
            return Patches.Enemies.Flowerman;
        }

        protected override int AmountPerStack()
        {
            return 1;
        }
    }
}