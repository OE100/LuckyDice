using LuckyDice.custom.monobehaviour.def;

namespace LuckyDice.custom.monobehaviour.impl.spawn.Items
{
    public class SpawnMetalSheet : BaseSpawnItemEvent
    {
        protected override int AmountPerStack()
        {
            return 1;
        }

        protected override int ItemId()
        {
            return 39;
        }

        protected override int ItemValue()
        {
            return 100;
        }
    }
}