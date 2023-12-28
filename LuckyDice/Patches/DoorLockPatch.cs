using HarmonyLib;
using LuckyDice.custom.events.implementation;

namespace LuckyDice.Patches
{
    [HarmonyPatch(typeof(DoorLock))]
    public class DoorLockPatch
    {
        [HarmonyPatch("Awake"), HarmonyPostfix]
        private static void AwakePostfix(DoorLock __instance)
        {
            RandomizeLocks.DoorLocks.Add(__instance.NetworkObjectId, __instance);
        }
        
        [HarmonyPatch("OnDestroy"), HarmonyPostfix]
        private static void OnDestroyPostfix(DoorLock __instance)
        {
            RandomizeLocks.DoorLocks.Remove(__instance.NetworkObjectId);
        }
    }
}