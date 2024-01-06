using LuckyDice.custom.monobehaviour.def;

namespace LuckyDice.custom.monobehaviour.impl.spawn.Items.single
{
    public class SpawnGoldBar : BaseSpawnItemEvent
    {
        protected override int AmountPerStack()
        {
            return 1;
        }

        protected override int ItemId()
        {
            return 36;
        }

        protected override int ItemValue()
        {
            return 200;
        }
    }
}