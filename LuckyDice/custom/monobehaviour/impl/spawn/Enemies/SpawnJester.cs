using LuckyDice.custom.monobehaviour.def;

namespace LuckyDice.custom.monobehaviour.impl.spawn.Enemies
{
    public class SpawnJester : BaseSpawnEnemyEvent
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