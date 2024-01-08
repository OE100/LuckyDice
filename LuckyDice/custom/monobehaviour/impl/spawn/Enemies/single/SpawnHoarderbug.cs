using LuckyDice.custom.monobehaviour.def;

namespace LuckyDice.custom.monobehaviour.impl.spawn.Enemies.single
{
    public class SpawnHoarderbug : BaseSpawnEnemyEvent<HoarderBugAI>
    {
        protected override int AmountPerStack() => 3;
    }
}