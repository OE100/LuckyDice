using LuckyDice.custom.monobehaviour.def;

namespace LuckyDice.custom.monobehaviour.impl.spawn.Enemies
{
    public class SpawnCentipede : BaseSpawnEnemyEvent
    {
        protected override string Name()
        {
            return Patches.Enemies.Centipede;
        }

        protected override int AmountPerStack()
        {
            return 4;
        }
    }
}