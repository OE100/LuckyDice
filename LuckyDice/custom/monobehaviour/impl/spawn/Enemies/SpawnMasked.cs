using LuckyDice.custom.monobehaviour.def;

namespace LuckyDice.custom.monobehaviour.impl.spawn.Enemies
{
    public class SpawnMasked : BaseSpawnEnemyEvent
    {
        protected override string Name()
        {
            return Patches.Enemies.MaskedPlayerEnemy;
        }

        protected override int AmountPerStack()
        {
            return 1;
        }
    }
}