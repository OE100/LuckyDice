using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace LuckyDice.Patches
{
    [HarmonyPatch(typeof(JesterAI))]
    public class JesterAIPatch
    {
        // Outside jester fix
        [HarmonyPatch(nameof(JesterAI.Update)), HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Stloc_1)
                {
                    codes[i - 1].opcode = OpCodes.Nop;
                    codes.Insert(i, new CodeInstruction(OpCodes.Ldc_I4_1));
                    break;
                }
            }

            return codes.AsEnumerable();
        }
    }
}