#region

using HarmonyLib;
using LuckyDice.custom.monobehaviour.impl;

#endregion

namespace LuckyDice.Patches
{
    [HarmonyPatch(typeof(DoorLock))]
    public class DoorLockPatch
    {
        [HarmonyPatch("Awake"), HarmonyPostfix]
        private static void PatchAwake(DoorLock __instance)
        {
            if (StartOfRound.Instance.IsHost || StartOfRound.Instance.IsServer)
                RandomizeLocks.doors.Add(__instance);
        }
    }
}