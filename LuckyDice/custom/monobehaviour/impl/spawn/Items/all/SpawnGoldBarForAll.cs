using LuckyDice.custom.monobehaviour.def;

namespace LuckyDice.custom.monobehaviour.impl.spawn.Items.all
{
    public class SpawnGoldBarForAll : BaseSpawnItemForAllEvent
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