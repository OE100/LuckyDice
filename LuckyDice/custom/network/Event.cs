#region

using System;

#endregion

namespace LuckyDice.custom.network
{
    [Serializable]
    public enum Event
    {
        Bleed = 0,
        RandomizeLocks,
        SpawnCoilhead,
        SpawnFlowerman,
        SpawnMasked,
        SpawnJester,
        SpawnCentipede,
        SpawnClownHorn,
        SpawnGoldBar,
        SpawnPickleJar,
        SpawnGoldBarForAll,
        HolyJihad,
        MaskedChaos,
        StormyWeather,
        ExplodeLandmines
    }
}