using LuckyDice.custom.monobehaviour.def;

namespace LuckyDice.custom.monobehaviour.impl.spawn.Enemies
{
    public class SpawnCoilhead : BaseSpawnEnemyEvent
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