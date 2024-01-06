﻿using LuckyDice.custom.monobehaviour.def;

namespace LuckyDice.custom.monobehaviour.impl.spawn.Items.all
{
    public class SpawnJarOfPicklesForAll : BaseSpawnItemForAllEvent
    {
        protected override int AmountPerStack()
        {
            return 10;
        }

        protected override int ItemId()
        {
            return 44;
        }

        protected override int ItemValue()
        {
            return 25;
        }
    }
}