using System;

namespace LuckyDice.custom.network
{
    [Serializable]
    public enum Event
    {
        Bleed = 0,
        SpawnCoilhead,
        SpawnFlowerman,
        SpawnMasked,
        SpawnJester,
        SpawnCentipede
    }
}