#region

using HarmonyLib;
using LuckyDice.custom.events.implementation;
using LuckyDice.custom.events.implementation.map;

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