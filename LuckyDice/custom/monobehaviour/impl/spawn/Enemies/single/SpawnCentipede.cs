using LuckyDice.custom.monobehaviour.def;

namespace LuckyDice.custom.monobehaviour.impl.spawn.Enemies.single
{
    public class SpawnCentipede : BaseSpawnEnemyEvent<CentipedeAI>
    {
        protected override int AmountPerStack() => 4;
    }
}