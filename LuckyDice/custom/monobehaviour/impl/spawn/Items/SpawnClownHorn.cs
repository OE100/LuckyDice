using LuckyDice.custom.monobehaviour.def;

namespace LuckyDice.custom.monobehaviour.impl.spawn.Items
{
    public class SpawnClownHorn : BaseSpawnItemEvent
    {
        protected override int AmountPerStack()
        {
            return 2;
        }

        protected override int ItemId()
        {
            return 25;
        }

        protected override int ItemValue()
        {
            return 75;
        }
    }
}